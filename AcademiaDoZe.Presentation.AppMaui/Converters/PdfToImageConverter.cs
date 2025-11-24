using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Converters
{
    public static class PdfToImageConverter
    {
        public static ImageSource? Convert(byte[]? pdfBytes)
        {
            if (pdfBytes == null || pdfBytes.Length == 0)
                return null;

            try
            {
             
                return ImageSource.FromFile("pdf_preview.png");
            }
            catch
            {
                return null;
            }
        }
    }
}
