using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public class DcNullHelper : IDcHelper
    {
        public Task SendIlrSubmission(List<FM36Learner> learners, long ukprn, short collectionYear, byte collectionPeriod, long jobId) => Task.CompletedTask;
        public Task SendPeriodEndTask(short collectionYear, byte collectionPeriod, long jobId, string taskName) => Task.CompletedTask;
    }
}
