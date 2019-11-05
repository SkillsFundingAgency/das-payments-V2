using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;

namespace SFA.DAS.Payments.Audit.Application.Data
{
    public interface IDataLockEventRepository
    {
        Task DeleteEventsPriorToSubmission(long ukprn, short academicYear, byte collectionPeriod, DateTime ilrSubmissionDateTime);
        Task DeleteEventsOfSubmission(long ukprn, short academicYear, byte collectionPeriod, DateTime ilrSubmissionDateTime);
    }

    public class DataLockEventRepository : IDataLockEventRepository
    {
        private readonly IPaymentsDataContext dataContext;

        public DataLockEventRepository(IPaymentsDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task DeleteEventsPriorToSubmission(long ukprn, short academicYear, byte collectionPeriod, DateTime ilrSubmissionDateTime)
        {
            await dataContext.Database.ExecuteSqlCommandAsync($@"
                delete 
                    from [Payments2].[DataLockEvent] 
                where 
                    Ukprn = {ukprn}
                    and AcademicYear = {academicYear}
                    and CollectionPeriod = {collectionPeriod}
                    and IlrSubmissionDateTime < {ilrSubmissionDateTime}
            ").ConfigureAwait(false);
        }

        public async Task DeleteEventsOfSubmission(long ukprn, short academicYear, byte collectionPeriod, DateTime ilrSubmissionDateTime)
        {
            await dataContext.Database.ExecuteSqlCommandAsync($@"
                delete 
                    from [Payments2].[DataLockEvent] 
                where 
                    Ukprn = {ukprn}
                    and AcademicYear = {academicYear}
                    and CollectionPeriod = {collectionPeriod}
                    and IlrSubmissionDateTime = {ilrSubmissionDateTime}
            ").ConfigureAwait(false);
        }
    }
}
