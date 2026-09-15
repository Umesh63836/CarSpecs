namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class CreateStagedFuelEfficiencyDto
    {
        public int ImportModelId { get; set; }

        public decimal FuelEfficiency { get; set; }
        public string FuelEfficiencyUnit { get; set; } = null!;

        public string? SourceColumn { get; set; }
        public int? PageNumber { get; set; }
        public string? Evidence { get; set; }
        public decimal? Confidence { get; set; }

        public List<int> ImportVariantIds { get; set; } = [];
    }
}
