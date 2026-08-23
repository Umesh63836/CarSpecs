namespace CarSpecAPI.Data.Models.ResponseModel
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public int ExpiresIn { get; set; } = 0;
    }
}
