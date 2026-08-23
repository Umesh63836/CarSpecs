namespace CarSpecAPI.Data.Models.ResponseModel
{
    public class VariantModelResponseDto
    {
        public int ModelId { get; set; }

        public string BrandName { get; set; } = string.Empty;

        public string ModelName { get; set; } = string.Empty;

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string? ModelImageUrl { get; set; }
    }
}
