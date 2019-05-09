using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class SubmittedLearnerAimBuilder : EarningEventBuilderBase
    {
        public IList<SubmittedLearnerAimModel> Build(ProcessLearnerCommand processLearnerCommand)
        {
            return null;
        }
    }
}
