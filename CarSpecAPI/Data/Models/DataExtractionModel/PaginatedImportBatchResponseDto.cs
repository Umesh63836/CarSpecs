namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public class PaginatedImportBatchResponseDto
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }

        public bool HasPreviousPage { get; set; }

        public bool HasNextPage { get; set; }

        public List<ImportBatchListDto> Batches { get; set; } = new();
    }
}
