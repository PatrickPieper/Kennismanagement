using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.IO;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace La_Game.Pdf
{
    public class ComparePdf
    {
        static void Main(string[] args)
        {
            //nieuwe pdf doc aanmaken
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Compare lessons";

            //lege page maken
            PdfPage page = document.AddPage();

            //get XGraphics for drawing
            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont font = new XFont("Arial", 20, XFontStyle.Bold);

            //text drawen
            gfx.DrawString("Compared Lessons Statistics", font, XBrushes.Black,
                new XRect(0, 0, page.Width, page.Height),
                XStringFormats.Center);

            //save doc
            const string filename = "ComparedLessons.pdf";
            document.Save(filename);

            //start viewer
            Process.Start(filename);
        }

    }
}