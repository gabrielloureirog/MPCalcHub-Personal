using MPCalcHub.Application.DataTransferObjects;
using MPCalcHub.Application.Interfaces.RabbitMQ;

namespace MPCalcHub.Application.Interfaces;

public interface IContactApplicationService : IBaseRabbitMQConsumer
{   
    Task<IEnumerable<Contact>> FindByDDD(int ddd);
    Task Remove(Guid id);
    Task<Contact> GetById(Guid id);
}