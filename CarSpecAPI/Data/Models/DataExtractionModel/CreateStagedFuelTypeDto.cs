namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class CreateStagedFuelTypeDto
    {
        public int ImportBatchId { get; set; }

        public string FuelTypeName { get; set; } = string.Empty;

        public int? ProductionFuelTypeId { get; set; }
    }
}
