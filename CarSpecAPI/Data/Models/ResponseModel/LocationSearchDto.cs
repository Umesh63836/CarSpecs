namespace CarSpecAPI.Data.Models.ResponseModel
{
    public class LocationSearchDto
    {
        public string Type { get; set; } = string.Empty;

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int StateCode { get; set; }

        public string StateName { get; set; } = string.Empty;

        public int? DistrictCode { get; set; }

        public string? DistrictName { get; set; }
    }
}
