using System.Threading.Tasks;

namespace SFA.DAS.Payments.ScheduledJobs.Monitoring.ApprenticeshipData
{
    public interface IApprenticeshipsDataService
    {
        Task ProcessComparison();
    }
}