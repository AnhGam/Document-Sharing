namespace document_sharing_manager.Core.Configurations
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public double DurationInMinutes { get; set; } = 15;
        public int RefreshTokenDurationInDays { get; set; } = 7;
    }

    public class SecuritySettings
    {
        public int BCryptWorkFactor { get; set; } = 12;
    }
}
