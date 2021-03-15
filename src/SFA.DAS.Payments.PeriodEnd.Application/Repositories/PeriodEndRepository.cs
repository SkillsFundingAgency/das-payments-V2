using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.PeriodEnd.Data;

namespace SFA.DAS.Payments.PeriodEnd.Application.Repositories
{
    public interface IPeriodEndRepository
    {
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
