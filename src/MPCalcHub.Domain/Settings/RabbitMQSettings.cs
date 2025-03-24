namespace MPCalcHub.Domain.Settings;

public class RabbitMQSettings
{
    public string HostName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string VirtualHost { get; set; }
    public int Port { get; set; }
    public string DefaultExchangeName { get; set; }
    public string DefaultExchangeType { get; set; }
}
