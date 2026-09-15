namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class UpdateStagedFeatureDto
    {
        public bool? Available { get; set; }
        public string? Value { get; set; }
        public List<int> ValueOptionIds { get; set; } = [];
        public string? SourceColumn { get; set; }
        public int? PageNumber { get; set; }
        public string? Evidence { get; set; }
        public decimal? Confidence { get; set; }
    }
}
