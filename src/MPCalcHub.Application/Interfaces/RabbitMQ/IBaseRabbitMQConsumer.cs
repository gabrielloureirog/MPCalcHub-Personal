namespace MPCalcHub.Application.Interfaces.RabbitMQ
{
    public interface IBaseRabbitMQConsumer : IDisposable
    {
        Task Consumer(string message, string rountingKey);
    }
}