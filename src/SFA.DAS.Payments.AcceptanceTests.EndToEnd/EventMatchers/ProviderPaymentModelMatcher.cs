using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Tests.Core.Builders;
using Learner = SFA.DAS.Payments.AcceptanceTests.Core.Data.Learner;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class ProviderPaymentModelMatcher : BaseMatcher<PaymentModel>
    {
        private readonly Provider provider;
        private readonly IPaymentsDataContext dataContext;
        private readonly TestSession testSession;
        private readonly CollectionPeriod currentCollectionPeriod;
        private readonly List<ProviderPayment> expectedPaymentInfo;
        private readonly List<Training> providerCurrentIlr;

        public ProviderPaymentModelMatcher(Provider provider,
            IPaymentsDataContext dataContext,
            TestSession testSession,
            CollectionPeriod currentCollectionPeriod,
            List<ProviderPayment> expectedPaymentInfo = null,
            List<Training> providerCurrentIlr = null)
        {
            this.provider = provider;
            this.dataContext = dataContext;
            this.testSession = testSession;
            this.currentCollectionPeriod = currentCollectionPeriod;
            this.expectedPaymentInfo = expectedPaymentInfo;
            this.providerCurrentIlr = providerCurrentIlr;
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
                var learner = testSession.GetLearner(provider.Ukprn, paymentInfo.LearnerId);

                var currentIlrForLearner =
                    providerCurrentIlr?.Where(x => x.LearnerId == paymentInfo.LearnerId).ToList();
                
                var contractType = GetContractType(currentIlrForLearner, learner, paymentInfo.TransactionType);

                if (paymentInfo.SfaCoFundedPayments != 0)
                {
                    var coFundedSfa = ToPaymentModel(paymentInfo, provider.Ukprn, FundingSourceType.CoInvestedSfa,
                        paymentInfo.SfaCoFundedPayments, provider.JobId, contractType, learner);
                    expectedPayments.Add(coFundedSfa);
                }

                if (paymentInfo.EmployerCoFundedPayments != 0)
                {
                    var coFundedEmp = ToPaymentModel(paymentInfo, provider.Ukprn,
                        FundingSourceType.CoInvestedEmployer, paymentInfo.EmployerCoFundedPayments, provider.JobId, contractType, learner);
                    expectedPayments.Add(coFundedEmp);
                }

                if (paymentInfo.SfaFullyFundedPayments != 0)
                {
                    var fullyFundedSfa = ToPaymentModel(paymentInfo, provider.Ukprn,
                        FundingSourceType.FullyFundedSfa, paymentInfo.SfaFullyFundedPayments, provider.JobId, contractType, learner);
                    expectedPayments.Add(fullyFundedSfa);
                }

                if (paymentInfo.LevyPayments != 0)
                {
                    var levyPayments = ToPaymentModel(paymentInfo, provider.Ukprn, FundingSourceType.Levy,
                        paymentInfo.LevyPayments, provider.JobId, contractType, learner);
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
                   expected.JobId == actual.JobId;
        }

        private PaymentModel ToPaymentModel(ProviderPayment paymentInfo,
            long ukprn,
            FundingSourceType fundingSource,
            decimal amount,
            long jobId,
            ContractType contractType,
            Learner learner)
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
                LearnerReferenceNumber = learner.LearnRefNumber,
                JobId = jobId
            };
        }

        private static ContractType GetContractType(List<Training> currentIlr, Learner learner, TransactionType transactionType)
        {
            if (currentIlr!= null && currentIlr.Any())
            {
                var ilrZProgAim = currentIlr.FirstOrDefault(x => x.AimReference == "ZPROG001");
                var ilrOtherAim = currentIlr.FirstOrDefault(x => x.AimReference != "ZPROG001");

                if (transactionType == TransactionType.OnProgrammeMathsAndEnglish || transactionType == TransactionType.BalancingMathsAndEnglish)
                {
                    return ilrOtherAim.ContractType;
                }

                return ilrZProgAim.ContractType;
            }

            var learnerZProgAim = learner.Aims.FirstOrDefault(x => x.AimReference == "ZPROG001");
            var learnerOtherAim = learner.Aims.FirstOrDefault(x => x.AimReference != "ZPROG001");

            var zProgAimContractType = learnerZProgAim?.PriceEpisodes.First().ContractType; // there could be multiple AIMs of the same type so this may not end up being correct
            var otherAimContractType = learnerOtherAim?.PriceEpisodes.First().ContractType;


            if (transactionType == TransactionType.OnProgrammeMathsAndEnglish || transactionType == TransactionType.BalancingMathsAndEnglish)
            {
                return otherAimContractType.Value;
            }

            return zProgAimContractType.Value;
        }
    }
}