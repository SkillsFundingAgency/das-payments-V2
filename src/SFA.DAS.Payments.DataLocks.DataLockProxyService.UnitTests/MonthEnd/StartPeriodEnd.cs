using System.Threading.Tasks;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.UnitTests.MonthEnd
{
    public class StartPeriodEnd
    {
#if DEBUG
        [Test]
        public async Task SendPeriodEndStartedEvent()
        {
            var messageSession = await MessageSessionBuilder.BuildAsync();

            await messageSession.Publish<PeriodEndStartedEvent>(m => { }, new PublishOptions());
        }
#endif
    }
}
