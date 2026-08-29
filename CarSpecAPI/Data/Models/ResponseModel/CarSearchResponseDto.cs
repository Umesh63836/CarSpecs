namespace CarSpecAPI.Data.Models.ResponseModel
{
    public class CarSearchResponseDto
    {
        public int TotalModels { get; set; }
        public int TotalVariants { get; set; }
        public List<CarModelSearchResponseDto> Models { get; set; } = [];
    }
}
