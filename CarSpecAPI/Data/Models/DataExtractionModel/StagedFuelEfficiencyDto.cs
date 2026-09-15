namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedFuelEfficiencyDto
    {
        public int ImportFuelEfficiencyId { get; set; }

        public decimal? FuelEfficiency { get; set; }

        public string? FuelEfficiencyUnit { get; set; }

        public string? SourceColumn { get; set; }

        public int? PageNumber { get; set; }

        public string? Evidence { get; set; }

        public decimal? Confidence { get; set; }
    }
}
