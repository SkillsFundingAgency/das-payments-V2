using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Messages;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers
{
    public class  EarningFailedDataLockMatchingHandler : IHandleMessages<EarningFailedDataLockMatching>
    {
        public static ConcurrentBag<EarningFailedDataLockMatching> ReceivedEvents { get; } = new ConcurrentBag<EarningFailedDataLockMatching>();

        public Task Handle(EarningFailedDataLockMatching message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}