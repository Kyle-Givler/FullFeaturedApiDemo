﻿namespace TodoApi.Options;

public class JwtOptions
{
    public const string Jwt = "Jwt";

    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; }
    public int RefreshExpirationDays { get; set; }
}
