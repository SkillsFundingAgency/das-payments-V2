using System;

namespace SFA.DAS.Payments.PeriodEnd.Application.Infrastructure
{
    public interface IPeriodEndConfiguration
    {
        TimeSpan TimeToPauseBetweenChecks { get; }
        TimeSpan TimeToWaitForJobToComplete { get; }
    }

    public class PeriodEndConfiguration: IPeriodEndConfiguration
    {
        public TimeSpan TimeToPauseBetweenChecks { get; set; }
        public TimeSpan TimeToWaitForJobToComplete { get; set; }
    }
}