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
        public decimal? SafetyRating { get; set; }
        public string? SafetyRatingSource { get; set; }
        public int? SafetyTestYear { get; set; }
        public List<decimal?> EngineCC { get; set; } = [];
        public List<string> FuelTypes { get; set; } = [];
        public decimal? FuelEfficiency { get; set; }
        public string? FuelEfficiencyUnit { get; set; }
        public string? FuelEfficiencySource { get; set; }
        public string? ModelImageUrl { get; set; }
    }
}
