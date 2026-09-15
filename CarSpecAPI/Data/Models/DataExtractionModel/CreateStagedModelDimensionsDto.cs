namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class CreateStagedModelDimensionsDto
    {
        public int ImportModelId { get; set; }

        public decimal? LengthMM { get; set; }
        public decimal? WidthMM { get; set; }
        public decimal? HeightMM { get; set; }
        public decimal? WheelbaseMM { get; set; }
        public decimal? GroundClearanceMM { get; set; }
        public decimal? BootSpaceLitres { get; set; }
        public decimal? FuelTankCapacityLitres { get; set; }

        public int? SourceId { get; set; }
        public string? EvidenceText { get; set; }
        public int? PageNumber { get; set; }
        public decimal? Confidence { get; set; }
    }
}
