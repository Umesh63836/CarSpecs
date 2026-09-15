namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedFeatureDto
    {
        public int ImportFeatureVariantId { get; set; }

        public int ImportFeatureId { get; set; }

        public int? FeatureId { get; set; }

        public string FeatureCode { get; set; } = string.Empty;

        public string? FeatureName { get; set; }

        public string? ValueType { get; set; }

        public bool IsMultiValue { get; set; }

        public bool? Available { get; set; }

        public string? Value { get; set; }

        public List<StagedFeatureValueOptionDto> ValueOptions { get; set; } = [];

        public string? SourceColumn { get; set; }

        public int? PageNumber { get; set; }

        public string? Evidence { get; set; }

        public decimal? Confidence { get; set; }
    }
}
