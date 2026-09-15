namespace CarSpecAPI.Data.Models.RequestModel
{
    using System.ComponentModel.DataAnnotations;

    public class CreateVariantDto
    {
        [Required]
        public string VariantName { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int PowertrainId { get; set; }

        [Range(1, int.MaxValue)]
        public int TransmissionId { get; set; }

        [Range(1, int.MaxValue)]
        public int DrivetrainId { get; set; }

        public decimal? ExShowroomPrice { get; set; }

        public string? VariantImageUrl { get; set; }
    }
}
