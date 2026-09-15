namespace CarSpecAPI.Data.Models.RequestModel
{
    public class BrochurePageSelectionRequest
    {
        public int ImportDocumentId { get; set; }
        public List<int> SelectedPages { get; set; } = new();
    }
}
