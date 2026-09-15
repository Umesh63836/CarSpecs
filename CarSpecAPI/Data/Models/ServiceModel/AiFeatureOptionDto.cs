namespace CarSpecAPI.Data.Models.ServiceModel
{
    public sealed class AiFeatureOptionDto
    {
        public int FeatureValueOptionId { get; set; }

        public string Value { get; set; } = "";

        public string DisplayName { get; set; } = "";
    }
}
