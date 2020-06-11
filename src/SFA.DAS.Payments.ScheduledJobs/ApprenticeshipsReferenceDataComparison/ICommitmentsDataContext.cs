using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Payments.ScheduledJobs.ApprenticeshipsReferenceDataComparison
{
    public interface ICommitmentsDataContext
    {
        DbSet<ApprenticeshipModel> Apprenticeship { get; }
    }
}
