namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedModelDetailDto
    {
        public int ImportModelId { get; set; }

        public int ImportBatchId { get; set; }

        public string BrandName { get; set; } = string.Empty;

        public string ModelName { get; set; } = string.Empty;

        public string? Category { get; set; }

        public string? BodyType { get; set; }

        public string? ModelImageUrl { get; set; }

        public StagedModelDimensionsDto Dimensions { get; set; } = new();

        public List<StagedVariantParentDto> ParentVariants { get; set; } = [];

        public List<StagedWarrantyDto> ModelWarranties { get; set; } = [];

        public List<StagedWarningDto> Warnings { get; set; } = [];
    }
}
