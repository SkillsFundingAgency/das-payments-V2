using System;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Tests.Core;
using SFA.DAS.Payments.Tests.Core.Builders;
using System.Collections.Generic;
using System.Linq;

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
                .Where(p =>
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
                    var coFundedSfa = ToPaymentModel(paymentInfo, provider.Ukprn, FundingSourceType.CoInvestedSfa, paymentInfo.SfaCoFundedPayments, provider.JobId, paymentInfo.AccountId);
                    expectedPayments.Add(coFundedSfa);
                }

                if (paymentInfo.EmployerCoFundedPayments != 0)
                {
                    var coFundedEmp = ToPaymentModel(paymentInfo, provider.Ukprn, FundingSourceType.CoInvestedEmployer, paymentInfo.EmployerCoFundedPayments, provider.JobId, paymentInfo.AccountId);
                    expectedPayments.Add(coFundedEmp);
                }

                if (paymentInfo.SfaFullyFundedPayments != 0)
                {
                    var fullyFundedSfa = ToPaymentModel(paymentInfo, provider.Ukprn, FundingSourceType.FullyFundedSfa, paymentInfo.SfaFullyFundedPayments, provider.JobId, paymentInfo.AccountId);
                    expectedPayments.Add(fullyFundedSfa);
                }

                if (paymentInfo.LevyPayments != 0)
                {
                    var levyPayments = ToPaymentModel(paymentInfo, provider.Ukprn, FundingSourceType.Levy, paymentInfo.LevyPayments, provider.JobId, paymentInfo.AccountId);
                    expectedPayments.Add(levyPayments);
                }
            }
            return expectedPayments;
        }

        protected override bool Match(PaymentModel expected, PaymentModel actual)
        {
            if (expected.CollectionPeriod.Period == actual.CollectionPeriod.Period &&
                expected.CollectionPeriod.AcademicYear == actual.CollectionPeriod.AcademicYear &&
                expected.DeliveryPeriod == actual.DeliveryPeriod &&
                expected.TransactionType == actual.TransactionType &&
                expected.ContractType == actual.ContractType &&
                expected.FundingSource == actual.FundingSource &&
                expected.Amount == actual.Amount &&
                expected.LearnerReferenceNumber == actual.LearnerReferenceNumber &&
                expected.Ukprn == actual.Ukprn &&
                expected.LearningAimStandardCode == actual.LearningAimStandardCode)
            {
                if (actual.LearningAimReference.Equals("ZPROG001", StringComparison.OrdinalIgnoreCase) && EnumHelper.IsOnProgType(actual.TransactionType))  //TODO: check with PO if this is ok
                {
                    return expected.AccountId == actual.AccountId;
                }

                return true;
            }

            return false;
        }

        private PaymentModel ToPaymentModel(
            ProviderPayment paymentInfo,
            long ukprn,
            FundingSourceType fundingSource,
            decimal amount,
            long jobId,
            long? employerAccountId)
        {
            var learner = testSession.GetLearner(ukprn, paymentInfo.LearnerId);

            var standardCode = paymentInfo.StandardCode;

            if (!standardCode.HasValue)
            {
                var aim = learner.Aims.FirstOrDefault(a =>
                {
                    var aimStartDate = a.StartDate.ToDate();
                    var aimStartPeriod = new CollectionPeriodBuilder().WithDate(aimStartDate).Build();
                    var aimDuration = string.IsNullOrEmpty(a.ActualDuration) ? a.PlannedDuration : a.ActualDuration;

                    var aimEndPeriod = AimPeriodMatcher.GetEndPeriodForAim(aimStartPeriod, aimDuration);
                    var aimFinishedInPreviousPeriod = aimEndPeriod.FinishesBefore(currentCollectionPeriod);
                    if (!aimFinishedInPreviousPeriod)
                    {
                        return true;
                    }

                    if (a.CompletionStatus == CompletionStatus.Withdrawn && amount >= 0M)
                    {
                        return false;
                    }

                    return a.AimReference == "ZPROG001" && (a.CompletionStatus == CompletionStatus.Completed || a.CompletionStatus == CompletionStatus.Withdrawn);
                });

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
                AccountId = employerAccountId,
                LearningAimStandardCode = standardCode.Value
            };
        }
    }
}