namespace CarSpecAPI.Data.Models.ResponseModel
{
    public class TransmissionDto
    {
        public int TransmissionId { get; set; }
        public string TransmissionType { get; set; } = null!;
        public byte NumberOfGears { get; set; }
    }
}
