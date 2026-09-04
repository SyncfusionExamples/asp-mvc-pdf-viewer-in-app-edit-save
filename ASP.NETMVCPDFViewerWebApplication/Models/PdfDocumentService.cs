using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ASP.NETMVCPDFViewerWebApplication.Models
{
    /// <summary>
    /// Handles PDF documents that are hosted on the IIS server.
    /// Documents are stored in "~/App_Data/PDFDocuments" so that they are never
    /// served directly by IIS static file handling, but always streamed through
    /// controller actions (which also allows saving changes back to the server).
    /// </summary>
    public static class PdfDocumentService
    {
        private const string PdfExtension = ".pdf";

        /// <summary>Physical folder on the server that hosts the PDF files.</summary>
        public static string DocumentsFolder
        {
            get
            {
                // App_Data is guaranteed to exist and is never served by IIS directly.
                string folder = HttpContext.Current.Server.MapPath("~/App_Data/PDFDocuments");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                return folder;
            }
        }

        /// <summary>
        /// Lists all PDF documents that are available on the server.
        /// </summary>
        public static List<PdfFileInfo> GetDocuments()
        {
            var directory = new DirectoryInfo(DocumentsFolder);
            return directory.Exists
                ? directory.GetFiles("*" + PdfExtension)
                            .OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
                            .Select(f => new PdfFileInfo
                            {
                                Name = f.Name,
                                SizeBytes = f.Length,
                                LastModified = f.LastWriteTime
                            })
                            .ToList()
                : new List<PdfFileInfo>();
        }

        /// <summary>
        /// Returns the physical path of an existing document, or null when the name
        /// is invalid (guards against path traversal such as "..\..\web.config").
        /// </summary>
        public static string GetSafeFullPath(string fileName)
        {
            string safeName = GetSafeFileName(fileName);
            if (safeName == null)
            {
                return null;
            }

            string combined = Path.GetFullPath(Path.Combine(DocumentsFolder, safeName));

            // The combined path must still live inside the documents folder
            // (compare including a separator so "docs" does not match "docs2").
            string folderRoot = Path.GetFullPath(DocumentsFolder).TrimEnd(Path.DirectorySeparatorChar)
                               + Path.DirectorySeparatorChar;
            if (!combined.StartsWith(folderRoot, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return File.Exists(combined) ? combined : null;
        }

        /// <summary>
        /// Saves (overwrites) the posted document on the server. The stream is
        /// written directly to disk, so large PDFs are never buffered in memory.
        /// Returns false for invalid names.
        /// </summary>
        public static bool SaveDocument(string fileName, Stream contents)
        {
            string safeName = GetSafeFileName(fileName);
            if (safeName == null)
            {
                return false;
            }

            string fullPath = Path.Combine(DocumentsFolder, safeName);
            using (var target = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                contents.CopyTo(target);
            }
            return true;
        }

        /// <summary>
        /// Reduces a user supplied value to a plain "*.pdf" file name (no folders,
        /// no traversal) or null when it is not a valid PDF file name.
        /// </summary>
        private static string GetSafeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            string safeName = Path.GetFileName(fileName);
            return !string.IsNullOrEmpty(safeName)
                   && safeName.EndsWith(PdfExtension, StringComparison.OrdinalIgnoreCase)
                ? safeName
                : null;
        }
    }

    /// <summary>Simple file descriptor returned to the browser.</summary>
    public class PdfFileInfo
    {
        public string Name { get; set; }
        public long SizeBytes { get; set; }
        public DateTime LastModified { get; set; }
    }
}
