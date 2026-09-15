namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedSpecificationCatalogDto
    {
        public int ImportSpecificationId { get; set; }
        public string SpecificationCode { get; set; } = null!;
        public string? SpecificationName { get; set; }
    }
}
