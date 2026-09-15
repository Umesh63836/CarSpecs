namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class UpdateStagedSpecificationDto
    {
        public decimal? NumericValue { get; set; }
        public string? TextValue { get; set; }
        public bool? BooleanValue { get; set; }
        public string? Unit { get; set; }
        public string? SourceColumn { get; set; }
        public int? PageNumber { get; set; }
        public string? Evidence { get; set; }
        public decimal? Confidence { get; set; }
    }
}
