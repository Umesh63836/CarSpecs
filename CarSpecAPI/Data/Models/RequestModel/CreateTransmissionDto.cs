namespace CarSpecAPI.Data.Models.RequestModel
{
    using System.ComponentModel.DataAnnotations;

    public class CreateTransmissionDto
    {
        [Required]
        public string TransmissionType { get; set; } = null!;

        [Range(1, 20)]
        public byte NumberOfGears { get; set; }
    }
}
