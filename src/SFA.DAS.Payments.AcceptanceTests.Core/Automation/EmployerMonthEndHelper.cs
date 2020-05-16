using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.FundingSource.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public static class EmployerMonthEndHelper
    {
        public static async Task SendLevyMonthEndForEmployers(long monthEndJobId, IEnumerable<long> employerAccountIds, short academicYear, byte collectionPeriod, IMessageSession messageSession)
        {
            Console.WriteLine($"Month end job id: {monthEndJobId}");

            foreach (var employerAccountId in employerAccountIds)
            {
                var processLevyFundsAtMonthEndCommand = new ProcessLevyPaymentsOnMonthEndCommand
                {
                    JobId = monthEndJobId,
                    CollectionPeriod = new CollectionPeriod { AcademicYear = academicYear, Period = collectionPeriod },
                    RequestTime = DateTime.Now,
                    SubmissionDate = DateTime.UtcNow,
                    AccountId = employerAccountId,
                };

                await messageSession.Send(processLevyFundsAtMonthEndCommand).ConfigureAwait(false);
            }
        }
    }
}