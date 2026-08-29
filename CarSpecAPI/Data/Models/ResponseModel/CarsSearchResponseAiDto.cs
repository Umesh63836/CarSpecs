namespace CarSpecAPI.Data.Models.ResponseModel
{
    public class CarSearchResponseAiDto
    {
        public int TotalModels { get; set; }
        public int TotalVariants { get; set; }
        public List<CarModelSearchResponseAiDto> Models { get; set; } = [];

    }
}
