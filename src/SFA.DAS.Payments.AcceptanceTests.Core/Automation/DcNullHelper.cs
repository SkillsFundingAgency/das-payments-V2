using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using NServiceBus;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public class DcNullHelper : IDcHelper
    {
        public Task SendIlrSubmission(List<FM36Learner> learners, long ukprn, short collectionYear, byte collectionPeriod, long jobId) => Task.CompletedTask;

        public Task SendCustomFm36File(IMessageSession messageSession, byte collectionPeriod)
        {
            return Task.CompletedTask;
        }
    }
}
