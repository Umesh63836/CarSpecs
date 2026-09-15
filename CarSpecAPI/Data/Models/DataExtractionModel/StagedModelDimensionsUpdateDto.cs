namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedModelDimensionsUpdateDto
    {
        public decimal? LengthMm { get; set; }

        public decimal? WidthMm { get; set; }

        public decimal? HeightMm { get; set; }

        public decimal? WheelbaseMm { get; set; }

        public decimal? GroundClearanceMm { get; set; }

        public decimal? BootSpaceLitres { get; set; }

        public decimal? FuelTankCapacityLitres { get; set; }

        public int? SourceId { get; set; }

        public string? EvidenceText { get; set; }

        public int? PageNumber { get; set; }

        public decimal? Confidence { get; set; }
    }
}
