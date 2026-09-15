namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedEngineDto
    {
        public bool Exists { get; set; }

        public int? ImportEngineId { get; set; }

        public string? EngineRef { get; set; }

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
