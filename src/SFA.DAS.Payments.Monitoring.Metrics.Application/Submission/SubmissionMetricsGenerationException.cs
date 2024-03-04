using System;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.Submission
{
    public class SubmissionMetricsGenerationException : Exception
    {
        public SubmissionMetricsGenerationException(string message) : base(message)
        {
        }
    }
}
