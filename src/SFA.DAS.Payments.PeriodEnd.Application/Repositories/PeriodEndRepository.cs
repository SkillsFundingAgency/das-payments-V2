using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.PeriodEnd.Data;
using SFA.DAS.Payments.PeriodEnd.Model;

namespace SFA.DAS.Payments.PeriodEnd.Application.Repositories
{
    public interface IPeriodEndRepository
    {
        Task RemoveUkrpnFromReprocessingList(long ukprn);
        Task<ProviderRequiringReprocessingEntity> RecordForProviderThatRequiresReprocessing(long ukprn);
        Task AddProviderThatRequiredReprocessing(long ukprn);
        Task<List<LatestSuccessfulJobModel>> GetLatestSuccessfulJobs();
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

        public async Task<ProviderRequiringReprocessingEntity> RecordForProviderThatRequiresReprocessing(long ukprn)
        {
            logger.LogDebug($"Getting ProviderRequiringReprocessing entity for Ukprn: {ukprn}");
            var record = await dataContext.ProvidersRequiringReprocessing.FirstOrDefaultAsync(x => x.Ukprn == ukprn);
            logger.LogInfo($"Finished getting ProviderRequiringReprocessing entity for Ukprn: {ukprn}");
            return record;
        }

        public async Task AddProviderThatRequiredReprocessing(long ukprn)
        {
            logger.LogDebug($"Adding ProviderRequiringReprocessing entity for provider: {ukprn}");
            var record = new ProviderRequiringReprocessingEntity
            {
                Ukprn = ukprn,
            };
            await dataContext.ProvidersRequiringReprocessing.AddAsync(record);
            await dataContext.SaveChanges();
            logger.LogInfo($"Finished adding ProviderRequiringReprocessing entity for Ukprn: {ukprn}");
        }


        public async Task<List<LatestSuccessfulJobModel>> GetLatestSuccessfulJobs()
        {
            var latestCollectionPeriod = await dataContext.LatestSuccessfulJobs
                .OrderByDescending(x => x.AcademicYear)
                .ThenByDescending(x => x.CollectionPeriod)
                .Select(x => new { x.AcademicYear, x.CollectionPeriod })
                .FirstOrDefaultAsync();

            if (latestCollectionPeriod == null)
            {
                return new List<LatestSuccessfulJobModel>();
            }

            return await dataContext.LatestSuccessfulJobs
                .Where(x =>
                    x.AcademicYear == latestCollectionPeriod.AcademicYear &&
                    x.CollectionPeriod == latestCollectionPeriod.CollectionPeriod)
                .ToListAsync();
        }
    }
}
