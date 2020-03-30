using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
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

    public class EarningEventRepository : IEarningEventRepository
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
            //EF Core 2.2 produces very inefficient sql for joins between in-memory collection and sql table
            var sqlWhereClause = earnings.Aggregate(string.Empty, (currentSql, model) => $"Or (JobId = {model.JobId} And Ukprn = {model.Ukprn} and AcademicYear = {model.AcademicYear} and CollectionPeriod = {model.CollectionPeriod} and ContractType = {model} and LearnerUln = {model.LearnerUln} and LearnerReferenceNumber = '{model.LearnerReferenceNumber}' and LearningAimReference = '{model.LearningAimReference}' and LearningAimProgrammeType = {model.LearningAimProgrammeType} and LearningAimStandardCode = {model.LearningAimStandardCode} and LearningAimFrameworkCode = {model.LearningAimFrameworkCode} and LearningAimPathwayCode = {model.LearningAimPathwayCode} and LearningAimFundingLineType = {model.LearningAimFundingLineType} and LearningAimSequenceNumber = {model.LearningAimSequenceNumber} and LearningStartDate = '{model.StartDate:yyyy-MM-dd hh:mm:ss}' and EventType = '{model.GetType().FullName}')\n\r");
            var sql = $@"Select [Id]
                    ,[EventId]
                ,[Ukprn]
                ,[ContractType]
                ,[CollectionPeriod]
                ,[AcademicYear]
                ,[LearnerReferenceNumber]
                ,[LearnerUln]
                ,[LearningAimReference]
                ,[LearningAimProgrammeType]
                ,[LearningAimStandardCode]
                ,[LearningAimFrameworkCode]
                ,[LearningAimPathwayCode]
                ,[LearningAimFundingLineType]
                ,[LearningStartDate]
                ,[AgreementId]
                ,[IlrSubmissionDateTime]
                ,[JobId]
                ,[EventTime]
                ,[CreationDate]
                ,[LearningAimSequenceNumber]
                ,[SfaContributionPercentage]
                ,[IlrFileName]
                ,[EventType] 
                From Payments2.Payment
                Where 1=0
                {sqlWhereClause}";
            cancellationToken.ThrowIfCancellationRequested();
            return await dataContext.EarningEvent
                .AsNoTracking()
                .FromSql(sql)
                .ToListAsync(cancellationToken);
        }
    }
}