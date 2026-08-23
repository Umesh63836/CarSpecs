namespace CarSpecAPI.Data.Models.ResponseModel
{
    public class SelectTransmissionDto
    {
        public int TransmissionId { get; set; }
        public string TransmissionType { get; set; } = null!;
        public byte NumberOfGears { get; set; } = 0;
    }
}
