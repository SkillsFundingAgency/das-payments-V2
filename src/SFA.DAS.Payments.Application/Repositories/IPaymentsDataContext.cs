using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        int SaveChanges();
    }
}