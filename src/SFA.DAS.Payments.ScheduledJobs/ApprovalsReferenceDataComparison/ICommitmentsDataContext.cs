using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Payments.ScheduledJobs.ApprovalsReferenceDataComparison
{
    public interface ICommitmentsDataContext
    {
        DbSet<ApprenticeshipModel> Apprenticeships { get; }
    }
}
