using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using RazorEngine;
using Urdms.DocumentBuilderService.Database.Entities;
//using WebSupergoo.ABCpdf8;
//using WebSupergoo.ABCpdf8.Internal;

namespace Urdms.DocumentBuilderService.Helpers
{
	// TODO: We used ABC pdf to generate our documents, you can reuse this or use another library

	public interface IPdfHelper
	{
	//    IDoc GeneratePdf<T>(string templateName, T model) where T : PdfModel;
	}

	public class PdfHelper : IPdfHelper
	{
	//    public IDoc GeneratePdf<T>(string templateName, T model) where T : PdfModel
	//    {
	//        var htmlView = GenerateDmpPdfFromTemplate(templateName, model);

	//        var pdfDoc = (IDoc)new Doc();
	//        pdfDoc.MediaBox.String = "A4";
	//        pdfDoc.AddPage();
	//        var imageId = pdfDoc.AddImageHtml(htmlView, true);
            
	//        while (true)
	//        {
	//            if (!pdfDoc.Chainable(imageId))
	//                break;
	//            pdfDoc.Page = pdfDoc.AddPage();
	//            imageId = pdfDoc.AddImageToChain(imageId);
	//        }
           
	//        return pdfDoc;
	//    }

	//    private string GenerateDmpPdfFromTemplate<T>(string templateName, T model) where T : PdfModel
	//    {
	//        var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

	//        Debug.Assert(assemblyDirectory != null, "directoryName in GenerateDmpPdfFromTemplate was null");

	//        var path = Path.Combine(assemblyDirectory.Replace("file:\\", String.Empty), 
	//                                "PdfTemplates", 
	//                                templateName + ".cshtml");

	//        var razorTemplate = File.ReadAllText(path);

	//        return Razor.Parse(razorTemplate, model);
	//    }
	}
}