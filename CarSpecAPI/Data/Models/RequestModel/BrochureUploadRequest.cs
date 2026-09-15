using Microsoft.OpenApi;

namespace CarSpecAPI.Data.Models.RequestModel
{
    public class BrochureUploadRequest
    {
        public IFormFile File { get; set; } = null!;

        public string SourceName { get; set; } = null!;

        public string? SourceUrl { get; set; }

        public string? Publisher { get; set; }

        public DateOnly? PublishedDate { get; set; }

        public string? Notes { get; set; }
    }
}

//IOpenApiExample:
//File:
//Swift_Brochure_2026.pdf

//SourceName:
//Maruti Suzuki Swift 2026 Official Brochure

//SourceUrl:
//https://www.marutisuzuki.com/...

//Publisher:
//Maruti Suzuki India Limited

//PublishedDate:
//2026 - 08 - 15