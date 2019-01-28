using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Repositories
{
    public interface IPaymentsDataContext
    {
        DbSet<LevyAccountModel> LevyAccount { get; }
        DbSet<PaymentModel> Payment { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        int SaveChanges();
    }
}