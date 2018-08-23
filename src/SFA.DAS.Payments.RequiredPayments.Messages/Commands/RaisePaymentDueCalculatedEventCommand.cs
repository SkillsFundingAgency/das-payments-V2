using System.Threading.Tasks;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Messages.Core.Commands;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Commands
{
    public class RaisePaymentDueCalculatedEventCommand : PaymentsCommand
    {
        // TODO: NSB gateway through constructor

        Task Execute(ICalculatedPaymentDueEvent calculatedPaymentDueEvent)
        {
            // TODO: call NSB gateway

            return Task.FromResult(0);
        }
    }
}
