using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.AcceptanceTests.Handlers
{
    public class RecordPeriodEndStartSucceededHandler : IHandleMessages<RecordPeriodEndRunStartJobSucceeded>
    {
    public static ConcurrentBag<RecordPeriodEndRunStartJobSucceeded> ReceivedEvents { get; } = new ConcurrentBag<RecordPeriodEndRunStartJobSucceeded>();

    public Task Handle(RecordPeriodEndRunStartJobSucceeded message, IMessageHandlerContext context)
    {

        //This should be using a custom event rather than command. Need to create the event objects
        // and also check where these will be handled

        return Task.CompletedTask;
    }
    }
}