namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class FuelEfficiencyDto
    {
        public int IndexId { get; set; }
        public decimal? FuelEfficiency { get; set; }
        public string? FuelEfficiencyUnit { get; set; }
        public List<string> AppliesToVariants { get; set; } = [];
        public string? SourceColumn { get; set; }
        public int? PageNumber { get; set; }
        public string? Evidence { get; set; }
        public decimal? Confidence { get; set; }
    }
}
