using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Query;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers.PeriodEnd
{
    class LogActors : IHandleMessages<RecordPeriodEndStartJob>
    {
        private readonly IPaymentLogger logger;

        public LogActors(IPaymentLogger logger)
        {
            this.logger = logger;
        }

        public async Task Handle(RecordPeriodEndStartJob message, IMessageHandlerContext context)
        {
            
        }
    }
}
