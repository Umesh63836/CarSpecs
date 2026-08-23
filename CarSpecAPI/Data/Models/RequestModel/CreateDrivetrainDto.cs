namespace CarSpecAPI.Data.Models.RequestModel
{
    using System.ComponentModel.DataAnnotations;

    public class CreateDrivetrainDto
    {
        [Required]
        public string DrivetrainType { get; set; } = null!;
    }
}
