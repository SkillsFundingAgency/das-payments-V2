using System.Threading.Tasks;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.UnitTests.MonthEnd
{
    public class StopPeriodEnd
    {
#if DEBUG
        [Test]
        public async Task SendPeriodEndStoppedEvent()
        {
            var messageSession = await MessageSessionBuilder.BuildAsync();

            await messageSession.Publish<PeriodEndStoppedEvent>(m => {}, new PublishOptions());
        }
#endif
    }
}
