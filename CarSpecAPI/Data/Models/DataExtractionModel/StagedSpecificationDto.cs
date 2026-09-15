namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedSpecificationDto
    {
        public int ImportSpecificationVariantId { get; set; }

        public int ImportSpecificationId { get; set; }

        public int? SpecificationId { get; set; }

        public string SpecificationCode { get; set; } = string.Empty;

        public string? SpecificationName { get; set; }

        public string? DataType { get; set; }

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
