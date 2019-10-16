using System;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Repositories;

namespace SFA.DAS.Payments.Audit.Application.Data.EarningEvent
{
    public interface IEarningEventRepository
    {
        Task RemovePriorEvents(long ukprn, DateTime latestIlrSubmission);
    }

    public class EarningEventRepository: IEarningEventRepository
    {
        private readonly IPaymentsDataContext dataContext;

        public EarningEventRepository(IPaymentsDataContext dataContext)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public async Task RemovePriorEvents(long ukprn, DateTime latestIlrSubmission)
        {
            throw new NotImplementedException();
        }
    }
}