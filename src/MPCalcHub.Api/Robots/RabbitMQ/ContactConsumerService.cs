using MPCalcHub.Domain.Constants;
using MPCalcHub.Application.Interfaces;
using MPCalcHub.Domain.Settings;
using Microsoft.Extensions.Options;

namespace MPCalcHub.Api.Robots.RabbitMQ
{
    public class ContactConsumerService : BaseRabbitMQConsumerService
    {
        public override string Queue => "mpcalchub.contact";

        public override string RoutingKey => "contact.*";

        public ContactConsumerService(IServiceProvider serviceProvider, IOptions<MPCalcHubSettings> settings,  ILogger<ContactConsumerService> logger)
            : base(serviceProvider, settings, logger)
        {
            ServiceMaps.Add(AppConstants.Routes.RabbitMQ.ContactInsert, sp => serviceProvider.GetService<IContactApplicationService>());
            ServiceMaps.Add(AppConstants.Routes.RabbitMQ.ContactUpdate, sp => serviceProvider.GetService<IContactApplicationService>());
        }
    }
}