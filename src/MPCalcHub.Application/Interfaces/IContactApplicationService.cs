using MPCalcHub.Application.DataTransferObjects;
using MPCalcHub.Application.Interfaces.RabbitMQ;

namespace MPCalcHub.Application.Interfaces;

public interface IContactApplicationService : IBaseRabbitMQConsumer
{   
    Task<IEnumerable<Contact>> FindByDDD(int ddd);
    Task<Contact> GetById(Guid id);

    Task<Contact> Add(BasicContact model);
    Task<Contact> Update(Contact model);
}