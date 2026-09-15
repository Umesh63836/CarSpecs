using UglyToad.PdfPig;

namespace CarSpecAPI.Services
{
    public class PdfTextExtractor : IPdfTextExtractor
    {
        public Task<string> ExtractTextAsync(Stream pdfStream, CancellationToken cancellationToken = default)
        {
            if (pdfStream == null)
                throw new ArgumentNullException(nameof(pdfStream));

            using var document = PdfDocument.Open(pdfStream);
            var pages = new List<string>();
            foreach (var page in document.GetPages())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var text = page.Text;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    pages.Add(text);
                }
            }
            var extractedText = string.Join(Environment.NewLine + Environment.NewLine, pages);

            return Task.FromResult(extractedText);
        }

        public Task<string> ExtractPageTextAsync(Stream pdfStream, int pageNumber, CancellationToken cancellationToken = default)
        {
            using var document = PdfDocument.Open(pdfStream);
            if (pageNumber < 1 || pageNumber > document.NumberOfPages)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(pageNumber));
            }
            var page = document.GetPage(pageNumber);
            return Task.FromResult(page.Text);
        }
    }
}
