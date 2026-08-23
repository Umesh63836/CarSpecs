namespace CarSpecAPI.Data.Models.RequestModel
{
    public class CreateVariantDto
    {
        public string VariantName { get; set; } = null!;

        public int EngineId { get; set; }
        public int TransmissionId { get; set; }
        public int DrivetrainId { get; set; }

        public decimal? ExShowroomPrice { get; set; }

        public string? VariantImageUrl { get; set; }
    }
}
