using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Tests.Core.Builders;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class ProviderPaymentModelMatcher : BaseMatcher<PaymentModel>
    {
        private readonly Provider provider;
        private readonly IPaymentsDataContext dataContext;
        private readonly TestSession testSession;
        private readonly CollectionPeriod currentCollectionPeriod;
        private readonly List<ProviderPayment> expectedPaymentInfo;
        private readonly ContractType contractType;

        public ProviderPaymentModelMatcher(Provider provider,
            IPaymentsDataContext dataContext,
            TestSession testSession,
            CollectionPeriod currentCollectionPeriod,
            List<ProviderPayment> expectedPaymentInfo = null,
            ContractType contractType = default(ContractType))
        {
            this.provider = provider;
            this.dataContext = dataContext;
            this.testSession = testSession;
            this.currentCollectionPeriod = currentCollectionPeriod;
            this.expectedPaymentInfo = expectedPaymentInfo;
            this.contractType = contractType;
        }

        protected override IList<PaymentModel> GetActualEvents()
        {
            return dataContext.Payment
                .Where(p => p.JobId == provider.JobId &&
                            p.CollectionPeriod.Period == currentCollectionPeriod.Period &&
                            p.CollectionPeriod.AcademicYear == currentCollectionPeriod.AcademicYear &&
                            p.Ukprn == provider.Ukprn)
                .ToList();
        }

        protected override IList<PaymentModel> GetExpectedEvents()
        {
            var expectedPayments = new List<PaymentModel>();

            foreach (var paymentInfo in expectedPaymentInfo)
            {
                if (paymentInfo.SfaCoFundedPayments != 0)
                {
                    var coFundedSfa = ToPaymentModel(paymentInfo, provider.Ukprn, FundingSourceType.CoInvestedSfa,
                        paymentInfo.SfaCoFundedPayments, testSession.JobId);
                    expectedPayments.Add(coFundedSfa);
                }

                if (paymentInfo.EmployerCoFundedPayments != 0)
                {
                    var coFundedEmp = ToPaymentModel(paymentInfo, provider.Ukprn,
                        FundingSourceType.CoInvestedEmployer, paymentInfo.EmployerCoFundedPayments, testSession.JobId);
                    expectedPayments.Add(coFundedEmp);
                }

                if (paymentInfo.SfaFullyFundedPayments != 0)
                {
                    var fullyFundedSfa = ToPaymentModel(paymentInfo, provider.Ukprn,
                        FundingSourceType.FullyFundedSfa, paymentInfo.SfaFullyFundedPayments, testSession.JobId);
                    expectedPayments.Add(fullyFundedSfa);
                }

                if (paymentInfo.LevyPayments != 0)
                {
                    var levyPayments = ToPaymentModel(paymentInfo, provider.Ukprn, FundingSourceType.Levy,
                        paymentInfo.LevyPayments, testSession.JobId);
                    expectedPayments.Add(levyPayments);
                }
            }
            return expectedPayments;
        }

        protected override bool Match(PaymentModel expected, PaymentModel actual)
        {
            return expected.CollectionPeriod.Period == actual.CollectionPeriod.Period &&
                   expected.CollectionPeriod.AcademicYear == actual.CollectionPeriod.AcademicYear &&
                   expected.DeliveryPeriod == actual.DeliveryPeriod &&
                   expected.TransactionType == actual.TransactionType &&
                   expected.ContractType == actual.ContractType &&
                   expected.FundingSource == actual.FundingSource &&
                   expected.Amount == actual.Amount &&
                   expected.LearnerReferenceNumber == actual.LearnerReferenceNumber &&
                   expected.Ukprn == actual.Ukprn &&
                   expected.JobId == actual.JobId &&
                   expected.LearningAimStandardCode == actual.LearningAimStandardCode;

        }

        private PaymentModel ToPaymentModel(
            ProviderPayment paymentInfo,
            long ukprn,
            FundingSourceType fundingSource,
            decimal amount,
            long jobId)
        {
            var learner = testSession.GetLearner(ukprn, paymentInfo.LearnerId);

            var standardCode = paymentInfo.StandardCode;

            if (!standardCode.HasValue)
            {
                var aim = learner.Aims.FirstOrDefault(a =>
                    AimPeriodMatcher.IsStartDateValidForCollectionPeriod(a.StartDate, currentCollectionPeriod,
                        a.PlannedDurationAsTimespan, a.ActualDurationAsTimespan, a.CompletionStatus,
                        a.AimReference));

                standardCode = aim?.StandardCode ?? 0;
            }

            return new PaymentModel
            {
                CollectionPeriod = new CollectionPeriodBuilder().WithSpecDate(paymentInfo.CollectionPeriod).Build(),
                Ukprn = ukprn,
                DeliveryPeriod = new DeliveryPeriodBuilder().WithSpecDate(paymentInfo.DeliveryPeriod).Build(),
                TransactionType = paymentInfo.TransactionType,
                ContractType = contractType,
                Amount = amount,
                FundingSource = fundingSource,
                LearnerReferenceNumber = learner.LearnRefNumber,
                JobId = jobId,
                LearningAimStandardCode = standardCode.Value
            };
        }
    }
}