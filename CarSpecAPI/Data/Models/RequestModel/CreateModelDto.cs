using System.ComponentModel.DataAnnotations;

namespace CarSpecAPI.Data.Models.RequestModel
{
    public class CreateModelDto
    {
        public string ModelName { get; set; } = null!;
        public int BrandId { get; set; }
        public bool IsActive { get; set; } = true;
        [Range(1950, 2026)]
        public short? LaunchYear { get; set; }
        [Range(1990, 2026)]
        public short? DiscontinuedYear { get; set; } = null;
        public string? ModelImageUrl { get; set; }
    }
}
