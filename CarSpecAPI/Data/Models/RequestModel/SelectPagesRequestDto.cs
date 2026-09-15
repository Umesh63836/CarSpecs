namespace CarSpecAPI.Data.Models.RequestModel
{
    public class SelectPagesRequestDto
    {
        public int ImportDocumentId { get; set; }
        public List<int> SelectedPages { get; set; } = [];
        public string FileName { get; set; } = "";
    }
}
