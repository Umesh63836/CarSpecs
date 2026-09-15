namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public class AuditChangesResponseDto
    {
        public List<AuditChangeDto> Changes { get; set; } = new();
    }
}
