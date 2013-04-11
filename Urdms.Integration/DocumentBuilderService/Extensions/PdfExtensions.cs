//using WebSupergoo.ABCpdf8.Internal;

namespace Urdms.DocumentBuilderService.Extensions
{
	// TODO: We used ABC pdf to generate our documents, you can reuse this or use another library

	//public static class PdfExtensions
	//{
	//    public static void RenderAsLandscape(this IDoc pdfDoc, string pageSize)
	//    {
	//        //Set page size to A4
	//        pdfDoc.MediaBox.String = pageSize;
	//        const int bottomMargin = 10;

	//        //Rotate writing page rectangle to landscape
	//        var pageWidth = pdfDoc.MediaBox.Width;
	//        var pageHeight = pdfDoc.MediaBox.Height;
	//        var pageLeft = pdfDoc.MediaBox.Left;
	//        var pageBottom = pdfDoc.MediaBox.Bottom;
	//        pdfDoc.Transform.Rotate(90, pageLeft, pageBottom);
	//        pdfDoc.Transform.Translate(pageWidth, 0);

	//        pdfDoc.Rect.Width = pageHeight;
	//        pdfDoc.Rect.Height = pageWidth;
	//        pdfDoc.Rect.Bottom = bottomMargin;
	//        pdfDoc.Rect.Inset(10, 10);

	//        //Rotate whole PDF doc to landscape 
	//        pdfDoc.SetInfo(pdfDoc.GetInfoInt(pdfDoc.Root, "Pages"), "/Rotate", "90");
	//    }

	//    public static void AddFooter(this IDoc pdfDoc, int bottomMargin, string format)
	//    {
	//        var pageCount = pdfDoc.PageCount;

	//        pdfDoc.Rect.Bottom = bottomMargin;
	//        pdfDoc.HPos = 1.0;
	//        pdfDoc.VPos = 1.0;
	//        pdfDoc.FontSize = 10;
	//        for (int i = 1; i <= pageCount; i++)
	//        {
	//            pdfDoc.PageNumber = i;
	//            pdfDoc.AddText(string.Format(format, i, pageCount));
	//        }
	//    }
	//}
}
