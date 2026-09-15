namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedModelListDto
    {
        public int ImportModelId { get; set; }

        public string BrandName { get; set; } = string.Empty;

        public string ModelName { get; set; } = string.Empty;

        public string? Category { get; set; }

        public string? BodyType { get; set; }

        public string? ModelImageUrl { get; set; }

        public int ParentVariantCount { get; set; }

        public int VariantCount { get; set; }

        public int WarningCount { get; set; }

        public int ImportBatchId { get; set; }

        public DateTime? StagedAt { get; set; }
    }
}
