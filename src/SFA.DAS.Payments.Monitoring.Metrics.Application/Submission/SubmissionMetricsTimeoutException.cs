using System;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.Submission
{
    public class SubmissionMetricsTimeoutException : Exception
    {
        public SubmissionMetricsTimeoutException(string message) : base(message)
        {
        }
    }
}
