namespace MPCalcHub.Domain.Options;

public class TokenSettings
{
    public string Key { get; set; }
    public int ExpirationTimeHour { get; set; }
    public int IncreaseExpirationTimeMinutes { get; set; }
}