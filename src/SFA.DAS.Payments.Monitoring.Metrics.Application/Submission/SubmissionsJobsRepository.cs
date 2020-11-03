using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Data;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.Submission
{
    public interface ISubmissionsJobsRepository
    {
        Task<List<LatestSuccessfulJobModel>> GetLatestSuccessfulJobsForCollectionPeriod(short academicYear, byte collectionPeriod);
    }

    public class SubmissionsJobsRepository : ISubmissionsJobsRepository
    {
        private readonly ISubmissionJobsDataContext dataContext;

        public SubmissionsJobsRepository(ISubmissionJobsDataContext dataContext)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public Task<List<LatestSuccessfulJobModel>> GetLatestSuccessfulJobsForCollectionPeriod(short academicYear, byte collectionPeriod)
        {
            return dataContext.LatestSuccessfulJobs
                .Where(x => x.AcademicYear == academicYear &&
                            x.CollectionPeriod == collectionPeriod)
                .ToListAsync();
        }
    }
}
