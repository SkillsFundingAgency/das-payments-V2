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
        Task<List<LatestSuccessfulJobModel>> GetLatestSuccessfulJobs(short academicYear, byte collectionPeriod);
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
