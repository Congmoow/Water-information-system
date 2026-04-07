namespace WaterInfoSystem.Infrastructure.Identity;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "WaterInfoSystem";

    public string Audience { get; set; } = "WaterInfoSystemClient";

    public string SigningKey { get; set; } = "WaterInfoSystemSigningKey1234567890";

    public int ExpiryMinutes { get; set; } = 480;
}
