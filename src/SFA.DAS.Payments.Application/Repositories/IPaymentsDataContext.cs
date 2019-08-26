using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Repositories
{
    public interface IPaymentsDataContext
    {
        DbSet<LevyAccountModel> LevyAccount { get; }
        DbSet<PaymentModel> Payment { get; }
        DbSet<ApprenticeshipModel> Apprenticeship { get; }
        DbSet<ApprenticeshipPriceEpisodeModel> ApprenticeshipPriceEpisode { get; }
        DbSet<SubmittedLearnerAimModel> SubmittedLearnerAim { get; }
        DbSet<ApprenticeshipDuplicateModel> ApprenticeshipDuplicate { get; }
        DbSet<DataLockFailureModel> DataLockFailure { get; }
        DbSet<EmployerProviderPriorityModel> EmployerProviderPriority { get; }
        DbSet<ApprenticeshipPauseModel> ApprenticeshipPause { get; }
        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        int SaveChanges();
        
    }
}