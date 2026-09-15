namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedFeatureSpecificationCatalogDto
    {
        public List<StagedFeatureCatalogDto> Features { get; set; } = [];
        public List<StagedSpecificationCatalogDto> Specifications { get; set; } = [];
    }
}
