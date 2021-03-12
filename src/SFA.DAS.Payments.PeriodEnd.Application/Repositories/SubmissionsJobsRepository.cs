using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.PeriodEnd.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.PeriodEnd.Application.Repositories
{
    public interface ISubmissionJobsRepository
    {
        Task<List<LatestSuccessfulJobModel>> GetLatestSuccessfulJobs();
    }

    public class SubmissionJobsRepository : ISubmissionJobsRepository
    {
        private readonly IPeriodEndDataContext dataContext;

        public SubmissionJobsRepository(IPeriodEndDataContext dataContext)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
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