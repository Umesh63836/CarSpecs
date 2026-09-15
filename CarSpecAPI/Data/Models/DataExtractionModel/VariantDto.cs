namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class VariantDto
    {
        public int IndexId { get; set; }
        public string VariantName { get; set; } = null!;

        public string VariantType { get; set; } = null!;

        public string? ParentVariantName { get; set; }
        public string? BaseVariantName { get; set; }

        public string? PowertrainRef { get; set; }
        public string? TransmissionRef { get; set; }
        public string? DrivetrainRef { get; set; }

        public decimal? ExShowroomPrice { get; set; }

        public int? KerbWeight { get; set; }
        public int? SeatingCapacity { get; set; }
    }
}
