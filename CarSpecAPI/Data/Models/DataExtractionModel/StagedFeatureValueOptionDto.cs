namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedFeatureValueOptionDto
    {
        public int ValueOptionId { get; set; }

        public string Value { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
