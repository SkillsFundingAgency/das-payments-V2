using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Repositories
{
    public interface IPaymentHistoryRepository : IDisposable
    {
        Task<List<PaymentHistoryEntity>> GetPaymentHistory(ApprenticeshipKey apprenticeshipKey, byte currentCollectionPeriod, CancellationToken cancellationToken = default(CancellationToken));

        Task<decimal> GetEmployerCoInvestedPaymentHistoryTotal(ApprenticeshipKey apprenticeshipKey, CancellationToken cancellationToken = default(CancellationToken));
        
        Task<List<IdentifiedRemovedLearningAim>> IdentifyRemovedLearnerAims(short academicYear, byte collectionPeriod, long ukprn, DateTime ilrSubmissionDateTime, CancellationToken cancellationToken);
    }

    public class PaymentHistoryRepository : IPaymentHistoryRepository
    {
        private readonly IPaymentsDataContext dataContext;

        public PaymentHistoryRepository(IPaymentsDataContext dataContext)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public async Task<List<PaymentHistoryEntity>> GetPaymentHistory(ApprenticeshipKey apprenticeshipKey, byte currentCollectionPeriod, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await dataContext.Payment
                .Where(payment => apprenticeshipKey.Ukprn == payment.Ukprn &&
                            apprenticeshipKey.FrameworkCode == payment.LearningAimFrameworkCode &&
                            apprenticeshipKey.LearnAimRef == payment.LearningAimReference &&
                            apprenticeshipKey.LearnerReferenceNumber == payment.LearnerReferenceNumber &&
                            apprenticeshipKey.PathwayCode == payment.LearningAimPathwayCode &&
                            (int)apprenticeshipKey.ProgrammeType == payment.LearningAimProgrammeType &&
                            apprenticeshipKey.StandardCode == payment.LearningAimStandardCode &&
                            apprenticeshipKey.AcademicYear == payment.CollectionPeriod.AcademicYear &&
                            apprenticeshipKey.ContractType == payment.ContractType &&
                            payment.CollectionPeriod.Period < currentCollectionPeriod)
                .Select(payment => new PaymentHistoryEntity
                {
                    ExternalId = payment.EventId,
                    AccountId = payment.AccountId,
                    TransferSenderAccountId = payment.TransferSenderAccountId,
                    ContractType = payment.ContractType,
                    Ukprn = payment.Ukprn,
                    LearnerReferenceNumber = payment.LearnerReferenceNumber,
                    LearnerUln = payment.LearnerUln,
                    LearnAimReference = payment.LearningAimReference,
                    LearningAimFundingLineType = payment.LearningAimFundingLineType,
                    TransactionType = (int)payment.TransactionType,
                    PriceEpisodeIdentifier = payment.PriceEpisodeIdentifier,
                    DeliveryPeriod = payment.DeliveryPeriod,
                    CollectionPeriod = payment.CollectionPeriod,
                    Amount = payment.Amount,
                    FundingSource = payment.FundingSource,
                    SfaContributionPercentage = payment.SfaContributionPercentage,
                    StartDate = payment.StartDate,
                    PlannedEndDate = payment.PlannedEndDate,
                    ActualEndDate = payment.ActualEndDate,
                    CompletionStatus = payment.CompletionStatus ?? 0,
                    CompletionAmount = payment.CompletionAmount ?? 0,
                    InstalmentAmount = payment.InstalmentAmount ?? 0,
                    NumberOfInstalments = payment.NumberOfInstalments ?? 0,
                    ApprenticeshipId = payment.ApprenticeshipId,
                    ApprenticeshipPriceEpisodeId = payment.ApprenticeshipPriceEpisodeId,
                    ApprenticeshipEmployerType = payment.ApprenticeshipEmployerType,
                    LearningStartDate = payment.LearningStartDate,
                })
            .ToListAsync(cancellationToken);
        }

        public async Task<decimal> GetEmployerCoInvestedPaymentHistoryTotal(ApprenticeshipKey apprenticeshipKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await dataContext.Payment
                .Where(payment => apprenticeshipKey.Ukprn == payment.Ukprn &&
                                  apprenticeshipKey.FrameworkCode == payment.LearningAimFrameworkCode &&
                                  apprenticeshipKey.LearnAimRef == payment.LearningAimReference &&
                                  apprenticeshipKey.LearnerReferenceNumber == payment.LearnerReferenceNumber &&
                                  apprenticeshipKey.PathwayCode == payment.LearningAimPathwayCode &&
                                  apprenticeshipKey.ProgrammeType == payment.LearningAimProgrammeType &&
                                  apprenticeshipKey.StandardCode == payment.LearningAimStandardCode &&
                                  apprenticeshipKey.ContractType == payment.ContractType &&
                                  payment.TransactionType == TransactionType.Learning &&
                                  payment.FundingSource == FundingSourceType.CoInvestedEmployer
                                  )
                //NOTE: do not remove cast to (int) this is used to remove decimal places from expectedContribution Amount i.e. 123.9997 becomes 123
                .Select(payment => (int)payment.Amount)
                .DefaultIfEmpty(0)
                .SumAsync(cancellationToken);
        }

        public async Task<List<IdentifiedRemovedLearningAim>> IdentifyRemovedLearnerAims(short academicYear, byte collectionPeriod, long ukprn, DateTime ilrSubmissionDateTime, CancellationToken cancellationToken)
        {
            var aims = await dataContext.SubmittedLearnerAim.FromSql($@"
                select
                    newid() Id,
                    {ukprn} Ukprn,
                    {collectionPeriod} CollectionPeriod,
                    {academicYear} AcademicYear,
                    {ilrSubmissionDateTime} IlrSubmissionDateTime,
                    {ukprn} JobId,
                    LearnerReferenceNumber,
                    LearningAimReference,
                    LearningAimFrameworkCode,
                    LearningAimPathwayCode,
                    LearningAimProgrammeType,
                    LearningAimStandardCode,
                    ContractType
                from (
                    select
                        p.LearnerReferenceNumber,
                        p.LearningAimReference,
                        p.LearningAimFrameworkCode,
                        p.LearningAimPathwayCode,
                        p.LearningAimProgrammeType,
                        p.LearningAimStandardCode,
                        p.ContractType
                    from [Payments2].[Payment] p
                    left outer join [Payments2].[SubmittedLearnerAim] sla
	           		on p.LearnerReferenceNumber = sla.LearnerReferenceNumber
                       and p.LearningAimReference = sla.LearningAimReference
                       and p.LearningAimFrameworkCode = sla.LearningAimFrameworkCode
                       and p.LearningAimPathwayCode = sla.LearningAimPathwayCode
                       and p.LearningAimProgrammeType = sla.LearningAimProgrammeType
                       and p.LearningAimStandardCode = sla.LearningAimStandardCode
                       and p.ContractType = sla.ContractType
	           		   and P.Ukprn = SLA.Ukprn
                       and p.AcademicYear = sla.AcademicYear
                       and sla.CollectionPeriod = {collectionPeriod}
                    where
	                    p.AcademicYear = {academicYear}
	                    and p.Ukprn = {ukprn}
	                	and sla.id is null
                ) as a
                group by
                    LearnerReferenceNumber,
                    LearningAimReference,
                    LearningAimFrameworkCode,
                    LearningAimPathwayCode,
                    LearningAimProgrammeType,
                    LearningAimStandardCode,
                    ContractType
"
            ).AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false);

            return aims.Select(p => new IdentifiedRemovedLearningAim
            {
                CollectionPeriod = new CollectionPeriod { AcademicYear = academicYear, Period = collectionPeriod },
                Ukprn = ukprn,
                EventId = Guid.NewGuid(),
                EventTime = DateTimeOffset.UtcNow,
                IlrSubmissionDateTime = ilrSubmissionDateTime,
                Learner = new Learner
                {
                    ReferenceNumber = p.LearnerReferenceNumber,
                },
                LearningAim = new LearningAim
                {
                    FrameworkCode = p.LearningAimFrameworkCode,
                    Reference = p.LearningAimReference,
                    PathwayCode = p.LearningAimPathwayCode,
                    StandardCode = p.LearningAimStandardCode,
                    ProgrammeType = p.LearningAimProgrammeType
                },
                ContractType = p.ContractType
            })
                .ToList();
        }

        public void Dispose()
        {
            (dataContext as PaymentsDataContext)?.Dispose();
        }
    }
}