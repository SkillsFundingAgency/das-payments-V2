using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using NServiceBus;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public class DcNullHelper : IDcHelper
    {
        public Task SendIlrSubmission(List<FM36Learner> learners, long ukprn, short collectionYear, byte collectionPeriod, long jobId) => Task.CompletedTask;

        public Task SendIlrSubmissionEvent(long ukprn, short collectionYear, byte collectionPeriod, long jobId,
            bool success) => Task.CompletedTask;

        public Task SendPeriodEndTask(short collectionYear, byte collectionPeriod, long jobId, string taskName) => Task.CompletedTask;

        public Task SendLevyMonthEndForEmployers(long monthEndJobId, IEnumerable<long> employerAccountIds, short academicYear,
            byte collectionPeriod, IMessageSession messageSession) => Task.CompletedTask;

        public int GetPaymentsCount(long ukprn, CollectionPeriod collectionPeriod) => 0;
    }
}
