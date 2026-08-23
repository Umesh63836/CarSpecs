namespace CarSpecAPI.Data.Models.RequestModel
{
    public class CreateBrandDto
    {
        public string BrandName { get; set; } = null!;
        public string? LogoUrl { get; set; }
    }
}
