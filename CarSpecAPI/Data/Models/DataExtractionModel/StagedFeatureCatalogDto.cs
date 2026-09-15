namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedFeatureCatalogDto
    {
        public int ImportFeatureId { get; set; }
        public string FeatureCode { get; set; } = null!;
        public string? FeatureName { get; set; }
    }
}
