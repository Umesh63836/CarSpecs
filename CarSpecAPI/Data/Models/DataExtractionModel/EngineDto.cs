namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class EngineDto
    {
        public int IndexId { get; set; }
        public string EngineRef { get; set; } = null!;
        public string EngineName { get; set; } = null!;

        public int? NumberOfCylinders { get; set; }
        public int? NumberOfValves { get; set; }
        public decimal? Displacement { get; set; }

        public bool? IsTurbocharged { get; set; }

        public string? EmissionStandard { get; set; }
        public string? Aspiration { get; set; }
        public string? EngineType { get; set; }

        public List<EnginePerformanceDto> EnginePerformances { get; set; } = [];
    }
}
