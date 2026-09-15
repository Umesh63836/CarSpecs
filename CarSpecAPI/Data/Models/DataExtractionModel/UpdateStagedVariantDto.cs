namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class UpdateStagedVariantDto
    {
        public string? VariantName { get; set; }

        public string? BaseVariantName { get; set; }

        public decimal? ExShowroomPrice { get; set; }

        public int? KerbWeight { get; set; }

        public int? SeatingCapacity { get; set; }

        public string? PowertrainRef { get; set; }

        public string? TransmissionRef { get; set; }

        public string? DrivetrainRef { get; set; }
    }
}
