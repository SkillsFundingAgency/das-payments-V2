using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd
{
    public interface ISubmissionWindowValidationClient
    {
        Task<bool> Validate(long jobId, short academicYear, byte collectionPeriod);
    }
}