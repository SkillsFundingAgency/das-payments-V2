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
    public interface IProvidersRequiringReprocessingRepository
    {
        Task Remove(long ukprn);
        
        /// <summary>
        /// This method will add the UKPRN to the table if it does not exist and ignore the new record if
        ///     an existing record already exists with the same UKPRN. The unique index has IGNORE_DUP_KEY
        ///     set
        /// </summary>
        Task Add(long ukprn);

        Task<List<long>> GetAll();

        Task<List<LatestSuccessfulJobModel>> GetLatestSuccessfulJobs(short academicYear, byte collectionPeriod);
    }

    public class ProvidersRequiringReprocessingRepository : IProvidersRequiringReprocessingRepository
    {
        private readonly IPeriodEndDataContext dataContext;
        private readonly IPaymentLogger logger;

        public ProvidersRequiringReprocessingRepository(IPeriodEndDataContext dataContext, IPaymentLogger logger)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Remove(long ukprn)
        {
            logger.LogDebug($"Removing UKPRN from ProviderRequiringReprocessing for ukprn: {ukprn}");
            var record = await dataContext.ProvidersRequiringReprocessing
                .FirstOrDefaultAsync(x => x.Ukprn == ukprn);
            if (record != null)
                dataContext.ProvidersRequiringReprocessing.Remove(record);
            await dataContext.SaveChangesAsync();

            logger.LogInfo($"Removed UKPRN from ProviderRequiringReprocessing for ukprn: {ukprn}");
        }

        public async Task Add(long ukprn)
        {
            logger.LogDebug($"Adding ProviderRequiringReprocessing entity for provider: {ukprn}");

            var isUkprnAlreadyAdded = await dataContext.ProvidersRequiringReprocessing.AnyAsync(x => x.Ukprn == ukprn);

            if (isUkprnAlreadyAdded) return;
            
            var record = new ProviderRequiringReprocessingEntity
            {
                Ukprn = ukprn,
            };
            await dataContext.ProvidersRequiringReprocessing.AddAsync(record);
            await dataContext.SaveChangesAsync();
            logger.LogInfo($"Finished adding ProviderRequiringReprocessing entity for Ukprn: {ukprn}");
        }

        public async Task<List<long>> GetAll()
        {
            logger.LogDebug("Getting all providers that need reprocessing");
            var providers = await dataContext.ProvidersRequiringReprocessing
                .Select(x => x.Ukprn)
                .ToListAsync();
            logger.LogInfo("Finished getting all providers that need reprocessing");
            return providers;
        }

        public async Task<List<LatestSuccessfulJobModel>> GetLatestSuccessfulJobs(short academicYear, byte collectionPeriod)
        {
            logger.LogDebug($"Getting latest successful jobs for Academic Year: {academicYear} and Collection Period: {collectionPeriod}");
            
            logger.LogInfo("Finished getting latest successful jobs");
            return await dataContext.LatestSuccessfulJobs
                .Where(x =>
                    x.AcademicYear == academicYear &&
                    x.CollectionPeriod == collectionPeriod)
                .ToListAsync();
        }
    }
}
