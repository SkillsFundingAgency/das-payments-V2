using SFA.DAS.Payments.FundingSource.Model.Enum;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Messages;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public class MonthEndEventHandlerService : IMonthEndEventHandlerService
    {
        private readonly IProviderPaymentsRepository providerPaymentsRepository;

        public MonthEndEventHandlerService(IProviderPaymentsRepository providerPaymentsRepository)
        {
            this.providerPaymentsRepository = providerPaymentsRepository;
        }

        public async Task<List<ProviderPaymentEvent>> GetMonthEndPaymentsAsync(short collectionYear, byte collectionPeriod, long ukprn, CancellationToken cancellationToken = default(CancellationToken))
        {
            var payments = await providerPaymentsRepository.GetMonthEndPaymentsAsync(collectionYear, collectionPeriod, ukprn, cancellationToken);
            return payments.Select(MapToPeriodicPayment).ToList();
        }

        private ProviderPaymentEvent MapToPeriodicPayment(PaymentDataEntity payment)
        {
            return new ProviderPaymentEvent
            {
                Ukprn = payment.Ukprn,
                FundingSourceType = (FundingSourceType)payment.FundingSource,
                AmountDue = payment.Amount,
                CollectionPeriod = new CalendarPeriod((short)payment.CollectionPeriodYear, (byte)payment.CollectionPeriodMonth),
                DeliveryPeriod = new CalendarPeriod((short)payment.DeliveryPeriodYear, (byte)payment.DeliveryPeriodMonth),
                ContractType = (byte)payment.ContractType,
                SfaContributionPercentage = payment.SfaContributionPercentage,
                JobId = payment.JobId,
                LearningAim = new LearningAim
                {
                    ProgrammeType = payment.LearningAimProgrammeType,
                    FrameworkCode = payment.LearningAimFrameworkCode,
                    PathwayCode = payment.LearningAimPathwayCode,
                    StandardCode = payment.LearningAimStandardCode,
                    AgreedPrice = payment.LearningAimAgreedPrice,
                    FundingLineType = payment.LearningAimFundingLineType,
                    Reference = payment.LearningAimReference
                },
                IlrSubmissionDateTime = payment.IlrSubmissionDateTime,
                OnProgrammeEarningType = (OnProgrammeEarningType)payment.TransactionType,
                Learner = new Learner
                {
                    Ukprn = payment.Ukprn,
                    ReferenceNumber = payment.LearnerReferenceNumber,
                    Uln = payment.LearnerUln
                },
                PriceEpisodeIdentifier = payment.PriceEpisodeIdentifier
            };

        }
    }
}
