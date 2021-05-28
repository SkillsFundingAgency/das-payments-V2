using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.ClawbackRemovedLearnerAimPaymentsService.Handlers
{
    public class IdentifiedRemovedLearningAimEventHandler : IHandleMessages<IdentifiedRemovedLearningAim>
    {
        public IdentifiedRemovedLearningAimEventHandler()
        {
        }

        public Task Handle(IdentifiedRemovedLearningAim message, IMessageHandlerContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}