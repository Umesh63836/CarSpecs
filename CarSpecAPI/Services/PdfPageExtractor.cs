using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace CarSpecAPI.Services
{
    public class PdfPageExtractor : IPdfPageExtractor
    {
        public async Task<byte[]> CreateSelectedPagesPdfAsync(Stream sourcePdf, IReadOnlyCollection<int> selectedPages, CancellationToken cancellationToken = default)
        {
            if (selectedPages == null || selectedPages.Count == 0)
            {
                throw new ArgumentException("At least one page must be selected.", nameof(selectedPages));
            }

            cancellationToken.ThrowIfCancellationRequested();

            // PDFsharp needs a seekable stream.
            await using var inputMemoryStream = new MemoryStream();

            await sourcePdf.CopyToAsync(inputMemoryStream, cancellationToken);

            inputMemoryStream.Position = 0;

            using var sourceDocument = PdfReader.Open(inputMemoryStream, PdfDocumentOpenMode.Import);

            using var outputDocument = new PdfDocument();

            foreach (var pageNumber in selectedPages.Distinct().OrderBy(x => x))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (pageNumber < 1 || pageNumber > sourceDocument.PageCount)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(selectedPages),
                        $"Page {pageNumber} does not exist. " +
                        $"PDF contains {sourceDocument.PageCount} pages.");
                }

                var sourcePage = sourceDocument.Pages[pageNumber - 1];

                outputDocument.AddPage(sourcePage);
            }

            await using var outputStream = new MemoryStream();

            outputDocument.Save(outputStream, false);

            return outputStream.ToArray();
        }
    }
}
