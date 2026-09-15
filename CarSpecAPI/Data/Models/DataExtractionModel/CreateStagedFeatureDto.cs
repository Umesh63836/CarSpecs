namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class CreateStagedFeatureDto
    {
        public int ImportModelId { get; set; }

        public int ImportVariantId { get; set; }

        public int? ImportFeatureId { get; set; }

        public string FeatureCode { get; set; } = string.Empty;

        public bool? Available { get; set; }

        public string? Value { get; set; }

        public List<int> ValueOptionIds { get; set; } = [];

        public string? SourceColumn { get; set; }

        public int? PageNumber { get; set; }

        public string? Evidence { get; set; }

        public decimal? Confidence { get; set; }
    }
}
