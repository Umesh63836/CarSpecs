namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class SpecificationDto
    {
        public int IndexId { get; set; }
        public int SpecificationId { get; set; }
        public string SpecificationCode { get; set; } = null!;
        public decimal? NumericValue { get; set; }
        public string? TextValue { get; set; }
        public bool? BooleanValue { get; set; }

        public string? Unit { get; set; }

        public List<string> AppliesToVariants { get; set; } = [];

        public string? SourceColumn { get; set; }
        public int? PageNumber { get; set; }
        public string? Evidence { get; set; }
        public decimal? Confidence { get; set; }
    }
}
