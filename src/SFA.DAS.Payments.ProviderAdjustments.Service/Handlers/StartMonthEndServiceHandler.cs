using System;
using System.Threading.Tasks;
using NServiceBus;

namespace SFA.DAS.Payments.ProviderAdjustments.Service.Handlers
{
    class StartMonthEndServiceHandler : IHandleMessages<StartMonthEndServiceHandler>
    {
        public Task Handle(StartMonthEndServiceHandler message, IMessageHandlerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
