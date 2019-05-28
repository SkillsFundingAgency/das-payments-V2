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
    public class PaymentHistoryRepository : IPaymentHistoryRepository
    {
        private readonly IPaymentsDataContext dataContext;

        public PaymentHistoryRepository(IPaymentsDataContext dataContext)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public async Task<List<PaymentHistoryEntity>> GetPaymentHistory(ApprenticeshipKey apprenticeshipKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await dataContext.Payment
                .Where(payment => apprenticeshipKey.Ukprn == payment.Ukprn &&
                            apprenticeshipKey.FrameworkCode == payment.LearningAimFrameworkCode &&
                            apprenticeshipKey.LearnAimRef == payment.LearningAimReference &&
                            apprenticeshipKey.LearnerReferenceNumber == payment.LearnerReferenceNumber &&
                            apprenticeshipKey.PathwayCode == payment.LearningAimPathwayCode &&
                            (int)apprenticeshipKey.ProgrammeType == payment.LearningAimProgrammeType &&
                            apprenticeshipKey.StandardCode == payment.LearningAimStandardCode &&
                            apprenticeshipKey.AcademicYear == payment.CollectionPeriod.AcademicYear)
                .Select(payment => new PaymentHistoryEntity
                {
                    ExternalId = payment.EventId,
                    AccountId = payment.AccountId,
                    TransferSenderAccountId = payment.TransferSenderAccountId,
                    ContractType = payment.ContractType,
                    Ukprn = payment.Ukprn,
                    LearnerReferenceNumber = payment.LearnerReferenceNumber,
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
                    CompletionStatus = payment.CompletionStatus,
                    CompletionAmount = payment.CompletionAmount,
                    InstalmentAmount = payment.InstalmentAmount,
                    NumberOfInstalments = payment.NumberOfInstalments
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
                                  payment.FundingSource == FundingSourceType.CoInvestedEmployer)
                .Select(payment => payment.Amount)
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
                    LearnerUln,
                    LearningAimReference,
                    LearningAimFrameworkCode,
                    LearningAimPathwayCode,
                    LearningAimProgrammeType,
                    LearningAimStandardCode
                from (
                    select
                        LearnerReferenceNumber,
                        LearnerUln,
                        LearningAimReference,
                        LearningAimFrameworkCode,
                        LearningAimPathwayCode,
                        LearningAimProgrammeType,
                        LearningAimStandardCode
                    from
                        [Payments2].[Payment]
                    where
					    AcademicYear = {academicYear}
                        and Ukprn = {ukprn}
                    except
				    select
					    LearnerReferenceNumber,
					    LearnerUln,
					    LearningAimReference,
					    LearningAimFrameworkCode,
					    LearningAimPathwayCode,
					    LearningAimProgrammeType,
					    LearningAimStandardCode
				    from
					    [Payments2].[SubmittedLearnerAim]
				    where
					    AcademicYear = {academicYear}
					    and Ukprn = {ukprn}
					    and CollectionPeriod = {collectionPeriod}
                ) as a
                group by
                    LearnerReferenceNumber,
                    LearnerUln,
                    LearningAimReference,
                    LearningAimFrameworkCode,
                    LearningAimPathwayCode,
                    LearningAimProgrammeType,
                    LearningAimStandardCode
"
            ).AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false);

            return aims.Select(p => new IdentifiedRemovedLearningAim
                {
                    CollectionPeriod = new CollectionPeriod {AcademicYear = academicYear, Period = collectionPeriod},
                    Ukprn = ukprn,
                    EventId = Guid.NewGuid(),
                    EventTime = DateTimeOffset.UtcNow,
                    IlrSubmissionDateTime = ilrSubmissionDateTime,
                    Learner = new Learner
                    {
                        ReferenceNumber = p.LearnerReferenceNumber,
                        Uln = p.LearnerUln
                    },
                    LearningAim = new LearningAim
                    {
                        FrameworkCode = p.LearningAimFrameworkCode,
                        Reference = p.LearningAimReference,
                        PathwayCode = p.LearningAimPathwayCode,
                        StandardCode = p.LearningAimStandardCode,
                        ProgrammeType = p.LearningAimProgrammeType
                    }
                })
                .ToList();
        }
    }
}