namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class UpdateStagedModelDto
    {
        public string? BrandName { get; set; }

        public string? ModelName { get; set; }

        public string? Category { get; set; }

        public string? BodyType { get; set; }

        public string? ModelImageUrl { get; set; }

        public StagedModelDimensionsUpdateDto Dimensions { get; set; } = new();
    }
}
