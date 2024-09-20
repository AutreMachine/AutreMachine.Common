using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.Common.AI
{
    /// <summary>
    /// Tools for PDF manipulations
    /// </summary>
    public class PDFTools
    {
        public async Task<ServiceResponse<string>> ReadPDFFile(string path)
        {
            if (!File.Exists(path))
                return ServiceResponse<string>.Ko("File not found");

            var reader = new PdfReader(path);

            var doc = new PdfDocument(reader);
            var sb = new StringBuilder();
            for (int i = 1; i <= doc.GetNumberOfPages(); i++)
            {
                var page = doc.GetPage(i);
                string text = PdfTextExtractor.GetTextFromPage(page);
                sb.Append(text);

            }

            return ServiceResponse<string>.Ok(sb.ToString());
        }
    }
}
