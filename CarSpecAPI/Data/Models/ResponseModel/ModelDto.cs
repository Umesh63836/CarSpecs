namespace CarSpecAPI.Data.Models.ResponseModel
{
    public class ModelDto
    {
        public int ModelId { get; set; }

        public string ModelName { get; set; } = string.Empty;

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public decimal? MinPower { get; set; }

        public decimal? MaxPower { get; set; }

        public string? ModelImageUrl { get; set; }
    }
}
