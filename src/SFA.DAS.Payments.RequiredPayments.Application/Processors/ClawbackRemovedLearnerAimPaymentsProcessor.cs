using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public interface IClawbackRemovedLearnerAimPaymentsProcessor
    {
        Task<IReadOnlyCollection<CalculatedRequiredLevyAmount>> GenerateClawbacksForRemovedLearnerAim(IdentifiedRemovedLearningAim identifiedRemovedLearningAim, CancellationToken cancellationToken);
    }

    public class ClawbackRemovedLearnerAimPaymentsProcessor : IClawbackRemovedLearnerAimPaymentsProcessor
    {
        public Task<IReadOnlyCollection<CalculatedRequiredLevyAmount>> GenerateClawbacksForRemovedLearnerAim(IdentifiedRemovedLearningAim identifiedRemovedLearningAim, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}