using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.Data.EarningEvent
{
    public interface IEarningEventRepository
    {
        Task RemovePriorEvents(long ukprn, short academicYear, byte collectionPeriod, DateTime latestIlrSubmissionTime, CancellationToken cancellationToken);
        Task RemoveFailedSubmissionEvents(long jobId, CancellationToken cancellationToken);
        Task SaveEarningEvents(List<EarningEventModel> earningEvents, CancellationToken cancellationToken);

        Task<List<EarningEventModel>> GetDuplicateEarnings(List<EarningEventModel> earnings,
            CancellationToken cancellationToken);
    }

    public class EarningEventRepository: IEarningEventRepository
    {
        private readonly IPaymentsDataContext dataContext;

        public EarningEventRepository(IPaymentsDataContext dataContext)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public async Task RemovePriorEvents(long ukprn, short academicYear, byte collectionPeriod, DateTime latestIlrSubmissionTime, CancellationToken cancellationToken)
        {
            await dataContext.Database.ExecuteSqlCommandAsync($@"
                    Delete 
                        From [Payments2].[EarningEvent] 
                    Where 
                        Ukprn = {ukprn}
                        And AcademicYear = {academicYear}
                        And CollectionPeriod = {collectionPeriod}
                        And IlrSubmissionDateTime < {latestIlrSubmissionTime}", cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task RemoveFailedSubmissionEvents(long jobId, CancellationToken cancellationToken)
        {
            await dataContext.Database.ExecuteSqlCommandAsync($@"
                    Delete 
                        From [Payments2].[EarningEvent] 
                    Where 
                        JobId = {jobId}", cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task SaveEarningEvents(List<EarningEventModel> earningEvents, CancellationToken cancellationToken)
        {
            await dataContext.EarningEvent.AddRangeAsync(earningEvents, cancellationToken).ConfigureAwait(false);
            await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<List<EarningEventModel>> GetDuplicateEarnings(List<EarningEventModel> earnings, CancellationToken cancellationToken)
        {
            var minEventTime = earnings.Min(earningEvent => earningEvent.EventTime).AddMinutes(-10);
            return await dataContext.EarningEvent
                .AsNoTracking()
                .Where(earningEvent => earningEvent.EventTime > minEventTime)
                .Join(earnings,
                    storedEarning => new
                    {
                        storedEarning.Ukprn, storedEarning.ContractType, storedEarning.CollectionPeriod,
                        storedEarning.AcademicYear, storedEarning.LearnerReferenceNumber, storedEarning.LearnerUln,
                        storedEarning.LearningAimReference, storedEarning.LearningAimProgrammeType,
                        storedEarning.LearningAimStandardCode, storedEarning.LearningAimFrameworkCode,
                        storedEarning.LearningAimPathwayCode, storedEarning.LearningAimFundingLineType,
                        storedEarning.LearningStartDate, storedEarning.JobId, storedEarning.LearningAimSequenceNumber,
                        storedEarning.EventType
                    },
                    receivedEarning => new
                    {
                        receivedEarning.Ukprn, receivedEarning.ContractType, receivedEarning.CollectionPeriod,
                        receivedEarning.AcademicYear, receivedEarning.LearnerReferenceNumber,
                        receivedEarning.LearnerUln, receivedEarning.LearningAimReference,
                        receivedEarning.LearningAimProgrammeType, receivedEarning.LearningAimStandardCode,
                        receivedEarning.LearningAimFrameworkCode, receivedEarning.LearningAimPathwayCode,
                        receivedEarning.LearningAimFundingLineType, receivedEarning.LearningStartDate,
                        receivedEarning.JobId, receivedEarning.LearningAimSequenceNumber, receivedEarning.EventType
                    }, (storedEarning,receivedEarning) => receivedEarning)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}