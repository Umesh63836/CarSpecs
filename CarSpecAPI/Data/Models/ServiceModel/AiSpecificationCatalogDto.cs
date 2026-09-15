namespace CarSpecAPI.Data.Models.ServiceModel
{
    public sealed class AiSpecificationCatalogDto
    {
        public int SpecificationId { get; set; }

        public string? SpecificationCode { get; set; } = "";

        public string SpecificationName { get; set; } = "";

        public string DataType { get; set; } = "";

        public string? Unit { get; set; }

        public string? Description { get; set; }

        public string? ExtractionGuidance { get; set; }
    }
}
