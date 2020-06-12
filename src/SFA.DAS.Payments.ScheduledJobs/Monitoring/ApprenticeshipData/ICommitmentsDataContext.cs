using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Payments.ScheduledJobs.Monitoring.ApprenticeshipData
{
    public interface ICommitmentsDataContext
    {
        DbSet<ApprenticeshipModel> Apprenticeship { get; }
    }
}
