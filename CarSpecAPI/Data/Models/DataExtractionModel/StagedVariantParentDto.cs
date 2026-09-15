namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedVariantParentDto
    {
        public int ImportVariantParentId { get; set; }

        public string ParentVariantName { get; set; } = string.Empty;

        public List<StagedVariantDetailDto> SubVariants { get; set; } = [];
    }
}
