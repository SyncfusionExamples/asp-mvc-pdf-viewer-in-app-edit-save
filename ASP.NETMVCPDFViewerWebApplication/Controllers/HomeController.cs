using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ASP.NETMVCPDFViewerWebApplication.Models;

namespace ASP.NETMVCPDFViewerWebApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        #region PDF document handling

        /// <summary>
        /// Returns the list of PDF documents that are hosted on the IIS server.
        /// Dates are formatted as ISO strings because the default JsonResult
        /// serialises DateTime as "/Date(...)/" which the browser cannot parse.
        /// </summary>
        [HttpGet]
        public JsonResult GetDocumentList()
        {
            var documents = PdfDocumentService.GetDocuments().Select(d => new
            {
                name = d.Name,
                sizeBytes = d.SizeBytes,
                lastModified = d.LastModified.ToString("yyyy-MM-dd HH:mm:ss")
            });
            return Json(documents, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Streams a PDF document (with annotations) to the PDF Viewer.
        /// Called from the browser via viewer.load('/Home/OpenDocument?file=...').
        /// The file is streamed straight from disk - it is never fully buffered in memory.
        /// </summary>
        [HttpGet]
        public ActionResult OpenDocument(string file)
        {
            string path = PdfDocumentService.GetSafeFullPath(file);
            if (path == null)
            {
                return new HttpNotFoundResult("The requested PDF document was not found on the server.");
            }

            Response.AppendHeader("Content-Disposition", "inline; filename=\"" + Uri.EscapeDataString(file) + "\"");
            return File(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read),
                        "application/pdf");
        }

        /// <summary>
        /// Saves the annotated PDF document back to the IIS server,
        /// overwriting the hosted copy so every user loads the modified document.
        /// </summary>
        [HttpPost]
        public JsonResult SaveDocument()
        {
            try
            {
                if (Request.Files == null || Request.Files.Count == 0)
                {
                    return Json(new { success = false, message = "No PDF file was received." });
                }

                var postedFile = Request.Files[0];
                string fileName = Request.QueryString["file"] ?? Request.Form["file"];
                if (postedFile == null || postedFile.ContentLength == 0 || string.IsNullOrWhiteSpace(fileName))
                {
                    return Json(new { success = false, message = "Invalid request: missing files or file name." });
                }

                // Streamed straight to disk - never buffered fully in memory.
                bool saved = PdfDocumentService.SaveDocument(fileName, postedFile.InputStream);
                return saved
                    ? Json(new { success = true, message = "Changes saved to the server." })
                    : Json(new { success = false, message = "The file name is not valid." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Save failed: " + ex.Message });
            }
        }

        #endregion
    }
}