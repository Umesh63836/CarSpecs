namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedPowertrainDependenciesDto
    {
        public List<StagedEngineLookupDto> Engines { get; set; } = [];
        public List<StagedEnginePerformanceLookupDto> EnginePerformances { get; set; } = [];
        public List<StagedMotorPerformanceLookupDto> MotorPerformances { get; set; } = [];
    }
}
