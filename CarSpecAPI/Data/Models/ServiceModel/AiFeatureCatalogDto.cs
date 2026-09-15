namespace CarSpecAPI.Data.Models.ServiceModel
{
    public sealed class AiFeatureCatalogDto
    {
        public int FeatureId { get; set; }

        public string? FeatureCode { get; set; } = "";

        public string FeatureName { get; set; } = "";

        public string? ValueType { get; set; } = "";

        public bool IsMultiValue { get; set; }

        public string? Unit { get; set; }

        public List<AiFeatureOptionDto> Options { get; set; } = [];
    }
}
