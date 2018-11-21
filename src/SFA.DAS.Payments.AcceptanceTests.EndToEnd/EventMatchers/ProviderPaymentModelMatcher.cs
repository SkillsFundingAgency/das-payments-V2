using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class ProviderPaymentModelMatcher : BaseMatcher<PaymentModel>
    {
        private readonly IPaymentsDataContext dataContext;
        private readonly TestSession testSession;
        private readonly string currentCollectionPeriodName;
        private readonly List<ProviderPayment> expectedPaymentInfo;
        private readonly ContractType contractType;


        public ProviderPaymentModelMatcher(IPaymentsDataContext dataContext, TestSession testSession, string currentCollectionPeriodName)
        {
            this.dataContext = dataContext;
            this.testSession = testSession;
            this.currentCollectionPeriodName = currentCollectionPeriodName;
        }

        public ProviderPaymentModelMatcher(IPaymentsDataContext dataContext, TestSession testSession, string currentCollectionPeriodName, List<ProviderPayment> expectedPaymentInfo, ContractType contractType)
            :this(dataContext,testSession, currentCollectionPeriodName)
        {
            this.expectedPaymentInfo = expectedPaymentInfo;
            this.contractType = contractType;
        }

        protected override IList<PaymentModel> GetActualEvents()
        {
            return dataContext.Payment.Where(p => p.JobId == testSession.JobId &&
                                                  p.CollectionPeriod.Name == currentCollectionPeriodName).ToList();
        }

        protected override IList<PaymentModel> GetExpectedEvents()
        {
            var expectedPayments = new List<PaymentModel>();

            foreach (var paymentInfo in expectedPaymentInfo)
            {
                var coFundedSfa = ToPaymentModel(paymentInfo, testSession.Ukprn);
                coFundedSfa.Amount = paymentInfo.SfaCoFundedPayments;
                coFundedSfa.FundingSource = FundingSourceType.CoInvestedSfa;

                var coFundedEmp = ToPaymentModel(paymentInfo, testSession.Ukprn);
                coFundedEmp.Amount = paymentInfo.EmployerCoFundedPayments;
                coFundedEmp.FundingSource = FundingSourceType.CoInvestedEmployer;
            
                expectedPayments.Add(coFundedSfa);
                expectedPayments.Add(coFundedEmp);
            }

            return expectedPayments;
        }

        protected override bool Match(PaymentModel expected, PaymentModel actual)
        {
            return expected.CollectionPeriod == actual.CollectionPeriod &&
                   expected.TransactionType == actual.TransactionType &&
                   expected.ContractType == actual.ContractType &&
                   expected.FundingSource == actual.FundingSource &&
                   expected.Amount == actual.Amount &&
                   expected.LearnerReferenceNumber == actual.LearnerReferenceNumber;
        }

        private PaymentModel ToPaymentModel(ProviderPayment paymentInfo, long ukprn)
        {
            return new PaymentModel
            {
                CollectionPeriod = paymentInfo.CollectionPeriod.ToCalendarPeriod(),
                Ukprn = ukprn,
                DeliveryPeriod = paymentInfo.DeliveryPeriod.ToCalendarPeriod(),
                TransactionType = paymentInfo.TransactionType,
                ContractType = contractType,
                Amount = paymentInfo.EmployerCoFundedPayments,
                FundingSource = FundingSourceType.CoInvestedEmployer,
                LearnerReferenceNumber = testSession.GetLearner(paymentInfo.LearnerId).LearnRefNumber
            };
        }
    }
}