using BCrypt.Net;
using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Data.Models.ResponseModel;
using CarSpecAPI.Entities;
using CarSpecAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class AuthService : IAuthService
{
    private readonly CarsDbContext carsDbContext;
    private readonly IConfiguration configuration;

    public AuthService(CarsDbContext carsDbContext,IConfiguration configuration)
    {
        this.carsDbContext = carsDbContext;
        this.configuration = configuration;
    }

    public async Task<RegisterDto?> RegisterAsync(RegisterDto dto)
    {
        var admin = await carsDbContext.Admins
            .FirstOrDefaultAsync(x =>
                x.Username == dto.Username);

        if (admin != null)
            return null;

        var adminEntity = new Admin
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            IsActive = true
        };

        carsDbContext.Admins.Add(adminEntity);

        await carsDbContext.SaveChangesAsync();

        return new RegisterDto
        {
            Username = adminEntity.Username,
            Password = adminEntity.PasswordHash
        };
    }

    public async Task<LoginResponseDto?> LoginAsync(AdminLoginDto dto)
    {
        var admin = await carsDbContext.Admins
            .FirstOrDefaultAsync(x =>
                x.Username == dto.Username &&
                x.IsActive);

        if (admin == null)
            return null;

        // Verify password against stored hash
        bool passwordValid = BCrypt.Net.BCrypt.Verify(
            dto.Password,
            admin.PasswordHash);

        if (!passwordValid)
            return null;

        // Generate access JWT
        var accessToken = GenerateJwtToken(admin);

        // Generate refresh token
        var refreshToken = GenerateRefreshToken();

        var days = configuration.GetValue<int>("Jwt:RefreshTokenDays");

        Console.WriteLine(days);

        var refreshTokenEntity = new RefreshToken
        {
            AdminId = admin.AdminId,
            TokenHash = refreshToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(configuration.GetValue<int>("Jwt:RefreshTokenDays"))
        };

        carsDbContext.RefreshTokens.Add(refreshTokenEntity);

        await carsDbContext.SaveChangesAsync();

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 900
        };
    }


    // REFRESH TOKEN
    public async Task<LoginResponseDto?> RefreshTokenAsync(
        string refreshToken)
    {

        var storedToken = await carsDbContext.RefreshTokens
            .Include(x => x.Admin)
            .FirstOrDefaultAsync(x =>
                x.TokenHash == refreshToken);

        if (storedToken == null)
            return null;

        // Token already revoked
        if (storedToken.RevokedAt != null)
            return null;

        // Token expired
        if (storedToken.ExpiresAt <= DateTime.UtcNow)
            return null;

        // Admin disabled
        if (!storedToken.Admin.IsActive)
            return null;


        // Generate new jwt
        var newAccessToken = GenerateJwtToken(
            storedToken.Admin);

        return new LoginResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 900
        };
    }


    // LOGOUT
    public async Task LogoutAsync(string refreshToken)
    {
        var storedToken = await carsDbContext.RefreshTokens
            .FirstOrDefaultAsync(x =>
                x.TokenHash == refreshToken);

        if (storedToken == null)
            return;

        if (storedToken.RevokedAt == null)
        {
            storedToken.RevokedAt = DateTime.UtcNow;
            await carsDbContext.SaveChangesAsync();
        }
    }


    // GENERATE JWT
    private string GenerateJwtToken(Admin admin)
    {
        var jwtKey = configuration["Jwt:Key"];

        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                admin.AdminId.ToString()),

            new Claim(
                ClaimTypes.Name,
                admin.Username),

            new Claim(
                ClaimTypes.Role,
                "Admin")
        };


        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        var credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);


        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:AccessTokenMinutes")),
            signingCredentials: credentials);


        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }


    // GENERATE REFRESH TOKEN
    private string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }

}