# ASP.NET MVC PDF Viewer – Open, Annotate In-App, and Save Back to the Server

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-512BD4)
![ASP.NET MVC 5](https://img.shields.io/badge/ASP.NET%20MVC-5.2.9-5C2D91)
![Syncfusion EJ2](https://img.shields.io/badge/Syncfusion%20EJ2-34.2.6-FF6D00)
![C#](https://img.shields.io/badge/language-C%23-239120)

An ASP.NET MVC 5 application that demonstrates **opening PDF documents hosted on the server, annotating them entirely in the browser, and saving the changes back to the same server-side document — without ever downloading the file locally**. All operations (open → annotate → save) are performed inside the application itself.

Built with the [Syncfusion ASP.NET MVC PDF Viewer](https://www.syncfusion.com/aspnet-mvc-ui-controls/pdf-viewer) control running in **standalone (client-side) mode**, so PDF rendering happens in the browser and the annotated document is written straight back to the hosted file on the web server.

---

## ✨ Features

- **Open server-hosted PDFs** – A document browser modal lists every PDF stored in `~/App_Data/PDFDocuments` with name, size, and last-modified date.
- **In-app annotation** – Highlight, underline, strikethrough, sticky notes, shapes, ink (freehand), stamps, free text, measurements, and fillable form fields — all inside the browser.
- **Save back to the server** – *Save changes* flattens every annotation into the PDF and overwrites the same file on the server. The next time anyone opens it, the modified document (with stored annotations) is loaded. No local download round-trip.
- **Download (optional)** – Export the annotated PDF to your computer with a single click, using the original file name.
- **Secure file handling** – Documents live in `App_Data` (never served directly by IIS), are streamed through controller actions, and file-name input is guarded against path traversal.

## 🚀 Getting started

### Prerequisites

- **An IDE** supports the *ASP.NET and web development* workload
- **.NET Framework 4.8 Developer Pack**
- NuGet package restore enabled (packages restore automatically on first build)
- A [Syncfusion license key](https://www.syncfusion.com/sales/products) (or use the free trial — an unlicensed build shows a trial watermark)

### Steps

1. Clone this repository.
2. Open `ASP.NETMVCPDFViewerWebApplication.sln` in Visual Studio or any IDE.
3. Restore NuGet packages (right-click solution → *Restore NuGet Packages*, or let the build do it).
4. Copy a few sample PDF files into the project folder
   `ASP.NETMVCPDFViewerWebApplication/App_Data/PDFDocuments/`
   (the folder is created automatically at runtime if it doesn't exist).
5. Run the application and open it any browser.
6. In the app, click **Open**, pick a document, add annotations, then click **Save changes**. Reload the same document to confirm the annotations were persisted on the server.
