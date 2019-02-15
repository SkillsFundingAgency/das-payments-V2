using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.Repositories
{
    public interface ICommitmentRepository
    {
        Task<List<CommitmentModel>> CommitmentsForProvider(long ukprn);
    }

    public class CommitmentRepository : ICommitmentRepository
    {
        private readonly IPaymentsDataContext dataContext;

        public CommitmentRepository(IPaymentsDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<List<CommitmentModel>> CommitmentsForProvider(long ukprn)
        {
            var commitments = await dataContext.Commitment
                .Where(x => x.Ukprn == ukprn)
                .ToListAsync()
                .ConfigureAwait(false);

            return commitments;
        }
    }
}
