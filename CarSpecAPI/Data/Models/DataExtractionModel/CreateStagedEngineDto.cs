namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class CreateStagedEngineDto
    {
        public string EngineRef { get; set; } = null!;
        public string EngineName { get; set; } = null!;

        public byte? NumberOfCylinders { get; set; }
        public byte? NumberOfValves { get; set; }

        public decimal? Displacement { get; set; }

        public bool? IsTurbocharged { get; set; }

        public string? EmissionStandard { get; set; }
        public string? Aspiration { get; set; }
        public string? EngineType { get; set; }
    }
}
