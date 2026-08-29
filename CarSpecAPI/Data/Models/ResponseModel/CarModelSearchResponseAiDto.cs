namespace CarSpecAPI.Data.Models.ResponseModel
{
    public class CarModelSearchResponseAiDto
    {
        public int ModelId { get; set; }

        public string Brand { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public string? ModelImageUrl {  get; set; } = string.Empty;

        public List<CarVariantSearchResponseAiDto> Variants { get; set; } = [];
    }
}
