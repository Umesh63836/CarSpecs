namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StageBatchResultDto
    {
        public int ImportBatchId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ImportModelId { get; set; }

        public int PowertrainsInserted { get; set; }
        public int EnginesInserted { get; set; }
        public int EnginePerformancesInserted { get; set; }
        public int MotorPerformancesInserted { get; set; }
        public int TransmissionsInserted { get; set; }
        public int DrivetrainsInserted { get; set; }

        public int VariantParentsInserted { get; set; }
        public int VariantsInserted { get; set; }

        public int FeatureVariantsInserted { get; set; }
        public int SpecificationVariantsInserted { get; set; }

        public int FuelEfficienciesInserted { get; set; }
        public int FuelEfficiencyVariantsInserted { get; set; }

        public int WarningsInserted { get; set; }

        public List<string> Warnings { get; set; } = [];
    }
}
