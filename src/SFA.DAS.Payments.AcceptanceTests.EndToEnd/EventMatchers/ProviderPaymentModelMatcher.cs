using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Tests.Core.Builders;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class ProviderPaymentModelMatcher : BaseMatcher<PaymentModel>
    {
        private readonly IPaymentsDataContext dataContext;
        private readonly TestSession testSession;
        private readonly string currentCollectionPeriodName;
        private readonly List<ProviderPayment> expectedPaymentInfo;
        private readonly ContractType contractType;


        public ProviderPaymentModelMatcher(
            IPaymentsDataContext dataContext, 
            TestSession testSession, 
            string currentCollectionPeriodName)
        {
            this.dataContext = dataContext;
            this.testSession = testSession;
            this.currentCollectionPeriodName = currentCollectionPeriodName;
        }

        public ProviderPaymentModelMatcher(
            IPaymentsDataContext dataContext, 
            TestSession testSession, 
            string currentCollectionPeriodName, 
            List<ProviderPayment> expectedPaymentInfo, 
            ContractType contractType)
            : this(dataContext, testSession, currentCollectionPeriodName)
        {
            this.expectedPaymentInfo = expectedPaymentInfo;
            this.contractType = contractType;
        }

        protected override IList<PaymentModel> GetActualEvents()
        {
            return dataContext.Payment.Where(p => p.JobId == testSession.JobId &&
                                                  p.CollectionPeriod.Name == currentCollectionPeriodName &&
                                                  p.Ukprn == testSession.Ukprn).ToList();
        }

        protected override IList<PaymentModel> GetExpectedEvents()
        {
            var expectedPayments = new List<PaymentModel>();

            foreach (var paymentInfo in expectedPaymentInfo)
            {
                if (paymentInfo.SfaCoFundedPayments != 0)
                {
                    var coFundedSfa = ToPaymentModel(paymentInfo, testSession.Ukprn, FundingSourceType.CoInvestedSfa,
                        paymentInfo.SfaCoFundedPayments, testSession.JobId);
                    expectedPayments.Add(coFundedSfa);
                }

                if (paymentInfo.EmployerCoFundedPayments != 0)
                {
                    var coFundedEmp = ToPaymentModel(paymentInfo, testSession.Ukprn,
                        FundingSourceType.CoInvestedEmployer, paymentInfo.EmployerCoFundedPayments, testSession.JobId);
                    expectedPayments.Add(coFundedEmp);
                }

                if (paymentInfo.SfaFullyFundedPayments != 0)
                {
                    var fullyFundedSfa = ToPaymentModel(paymentInfo, testSession.Ukprn,
                        FundingSourceType.FullyFundedSfa, paymentInfo.SfaFullyFundedPayments, testSession.JobId);
                    expectedPayments.Add(fullyFundedSfa);
                }

                if (paymentInfo.LevyPayments != 0)
                {
                    var levyPayments = ToPaymentModel(paymentInfo, testSession.Ukprn, FundingSourceType.Levy,
                        paymentInfo.LevyPayments, testSession.JobId);
                    expectedPayments.Add(levyPayments);
                }
            }
            return expectedPayments;
        }

        protected override bool Match(PaymentModel expected, PaymentModel actual)
        {
            return expected.CollectionPeriod.Name == actual.CollectionPeriod.Name &&
                   expected.DeliveryPeriod == actual.DeliveryPeriod &&
                   expected.TransactionType == actual.TransactionType &&
                   expected.ContractType == actual.ContractType &&
                   expected.FundingSource == actual.FundingSource &&
                   expected.Amount == actual.Amount &&
                   expected.LearnerReferenceNumber == actual.LearnerReferenceNumber &&
                   expected.Ukprn == actual.Ukprn &&
                   expected.JobId == actual.JobId;

        }

        private PaymentModel ToPaymentModel(
            ProviderPayment paymentInfo, 
            long ukprn, 
            FundingSourceType fundingSource, 
            decimal amount, 
            long jobId)
        {
            return new PaymentModel
            {
                CollectionPeriod = new CollectionPeriodBuilder().WithSpecDate(paymentInfo.CollectionPeriod).Build(),
                Ukprn = ukprn,
                DeliveryPeriod = new DeliveryPeriodBuilder().WithSpecDate(paymentInfo.DeliveryPeriod).Build(),
                TransactionType = paymentInfo.TransactionType,
                ContractType = contractType,
                Amount = amount,
                FundingSource = fundingSource,
                LearnerReferenceNumber = testSession.GetLearner(paymentInfo.LearnerId).LearnRefNumber,
                JobId = jobId
            };
        }
    }
}