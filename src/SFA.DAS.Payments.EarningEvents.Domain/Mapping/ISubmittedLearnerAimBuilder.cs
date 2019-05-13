using System.Collections.Generic;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.EarningEvents.Domain.Mapping
{
    public interface ISubmittedLearnerAimBuilder
    {
        IList<SubmittedLearnerAimModel> Build(ProcessLearnerCommand processLearnerCommand);
    }
}
