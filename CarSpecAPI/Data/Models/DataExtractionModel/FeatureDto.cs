namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class FeatureDto
    {
        public int IndexId { get; set; }
        public int FeatureId { get; set; }
        public string FeatureCode { get; set; } = null!;

        public bool? Available { get; set; }

        public List<int> ValueOptionIds { get; set; } = [];

        public string? Value { get; set; }

        public List<string> AppliesToVariants { get; set; } = [];

        public string? SourceColumn { get; set; }
        public int? PageNumber { get; set; }
        public string? Evidence { get; set; }
        public decimal? Confidence { get; set; }
    }
}
