namespace CarSpecAPI.Data.Models.ResponseModel
{
    public class SearchResultDto
    {
        public string ResultType { get; set; } = string.Empty;

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? BrandName { get; set; }

        public string? ModelName { get; set; }
    }
}
