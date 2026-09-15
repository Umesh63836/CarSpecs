namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class UpdateStagedEngineDto
    {
        public string? EngineName { get; set; }
        public int? NumberOfCylinders { get; set; }
        public int? NumberOfValves { get; set; }
        public decimal? Displacement { get; set; }
        public bool? IsTurbocharged { get; set; }
        public string? EmissionStandard { get; set; }
        public string? Aspiration { get; set; }
        public string? EngineType { get; set; }
    }
}
