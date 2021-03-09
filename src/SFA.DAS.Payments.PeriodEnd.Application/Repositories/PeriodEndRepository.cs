using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.PeriodEnd.Application.Data;

namespace SFA.DAS.Payments.PeriodEnd.Application.Repositories
{
    public interface IPeriodEndRepository
    {
        Task RemoveUkrpnFromReprocessingList(long ukprn);
    }

    public class PeriodEndRepository : IPeriodEndRepository
    {
        private readonly IPeriodEndDataContext dataContext;
        private readonly IPaymentLogger logger;

        public PeriodEndRepository(IPeriodEndDataContext dataContext, IPaymentLogger logger)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RemoveUkrpnFromReprocessingList(long ukprn)
        {
            logger.LogDebug($"Removing UKPRN from ProviderRequiringReprocessing for ukprn: {ukprn}");
            var record = await dataContext.ProvidersRequiringReprocessing
                .FirstOrDefaultAsync(x => x.Ukprn == ukprn);
            if (record != null)
                dataContext.ProvidersRequiringReprocessing.Remove(record);
            await dataContext.SaveChanges();

            //dataContext.ProvidersRequiringReprocessing.FromSql($"DELETE [Payments2].[ProviderRequiringReprocessing] WHERE Ukprn = {ukprn}");
            logger.LogInfo($"Removed UKPRN from ProviderRequiringReprocessing for ukprn: {ukprn}");
        }
    }
}
