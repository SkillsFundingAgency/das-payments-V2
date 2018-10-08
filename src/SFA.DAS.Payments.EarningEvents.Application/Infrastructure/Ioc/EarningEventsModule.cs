using Autofac;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Application.Services;

namespace SFA.DAS.Payments.EarningEvents.Application.Infrastructure.Ioc
{
    public class EarningEventsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new ApprenticeshipContractType2EarningEventsService
            (
                c.Resolve<IPaymentLogger>(),
                c.Resolve<IEarningEventMapper>()
            )).As<IEarningEventsProcessingService>();
        }
    }
}