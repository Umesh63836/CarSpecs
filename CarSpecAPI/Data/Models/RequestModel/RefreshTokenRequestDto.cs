namespace CarSpecAPI.Data.Models.RequestModel
{
    public class RefreshTokenRequestDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
