using CarSpecAPI.Data.Models.ResponseModel;

namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class BrochureExtractionDto
    {
        public DocumentDto? Document { get; set; }

        public VehicleDto? Vehicle { get; set; }

        public List<PowertrainDto> Powertrains { get; set; } = [];
        public List<EngineDto> Engines { get; set; } = [];
        public List<TransmissionDto> Transmissions { get; set; } = [];
        public List<DrivetrainDto> Drivetrains { get; set; } = [];
        public List<VariantDto> Variants { get; set; } = [];

        public List<FeatureDto> Features { get; set; } = [];

        public List<SpecificationDto> Specifications { get; set; } = [];

        public List<FuelEfficiencyDto> FuelEfficiencies { get; set; } = [];

        public List<string> Warnings { get; set; } = [];
    }
}
