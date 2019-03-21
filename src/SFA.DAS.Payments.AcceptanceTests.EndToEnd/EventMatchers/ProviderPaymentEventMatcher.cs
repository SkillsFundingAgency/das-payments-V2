using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Messages;
using SFA.DAS.Payments.Tests.Core;
using SFA.DAS.Payments.Tests.Core.Builders;
using Learner = SFA.DAS.Payments.Model.Core.Learner;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class ProviderPaymentEventMatcher : BaseMatcher<ProviderPaymentEvent>
    {
        private readonly List<ProviderPayment> paymentSpec;
        private readonly TestSession testSession;
        private readonly Provider provider;
        private readonly CollectionPeriod collectionPeriod;

        public ProviderPaymentEventMatcher(Provider provider, CollectionPeriod collectionPeriod, TestSession testSession, List<ProviderPayment> paymentSpec = null)
        {
            this.paymentSpec = paymentSpec;
            this.provider = provider;
            this.collectionPeriod = collectionPeriod;
            this.testSession = testSession;
        }

        protected override IList<ProviderPaymentEvent> GetActualEvents()
        {
            return ProviderPaymentEventHandler.ReceivedEvents
                .Where(ev =>
                    ev.Ukprn == provider.Ukprn &&
                    ev.JobId == provider.JobId &&
                    ev.CollectionPeriod.Period == collectionPeriod.Period && 
                    ev.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear)
                .ToList();
        }

        protected override IList<ProviderPaymentEvent> GetExpectedEvents()
        {
            var expectedPayments = new List<ProviderPaymentEvent>();
            var payments = paymentSpec.Where(p => p.ParsedCollectionPeriod.Period == collectionPeriod.Period
                                                  && p.ParsedCollectionPeriod.AcademicYear == collectionPeriod.AcademicYear);

            foreach (var providerPayment in payments)
            {
                var eventCollectionPeriod = new CollectionPeriodBuilder().WithSpecDate(providerPayment.CollectionPeriod).Build();
                var deliveryPeriod = new DeliveryPeriodBuilder().WithSpecDate(providerPayment.DeliveryPeriod).Build(); 
                var testLearner = testSession.GetLearner(provider.Ukprn,providerPayment.LearnerId);

                var learner = new Learner
                {
                    ReferenceNumber = testLearner.LearnRefNumber,
                    Uln = testLearner.Uln,
                };

                var standardCode = providerPayment.StandardCode;

                if (!standardCode.HasValue)
                {
                    var aim = testLearner.Aims.FirstOrDefault(a =>
                    {
                        var aimStartDate = a.StartDate.ToDate();
                        var aimStartPeriod = new CollectionPeriodBuilder().WithDate(aimStartDate).Build();
                        var aimDuration = string.IsNullOrEmpty(a.ActualDuration) ? a.PlannedDuration : a.ActualDuration;

                        var aimEndPeriod = AimPeriodMatcher.GetEndPeriodForAim(aimStartPeriod, aimDuration);
                        var aimFinishedInPreviousPeriod = aimEndPeriod.FinishesBefore(collectionPeriod);
                        if (!aimFinishedInPreviousPeriod)
                        {
                            return true;
                        }

                        // withdrawal but payments made during period active
                        if (a.CompletionStatus == CompletionStatus.Withdrawn && providerPayment.LevyPayments >= 0M && providerPayment.SfaCoFundedPayments >= 0M && providerPayment.EmployerCoFundedPayments >= 0M && providerPayment.SfaFullyFundedPayments >= 0M)
                        {
                            return false;
                        }

                        // retrospective withdrawal
                        return a.AimReference == "ZPROG001" && (a.CompletionStatus == CompletionStatus.Completed || a.CompletionStatus == CompletionStatus.Withdrawn);
                    });

                    standardCode = aim?.StandardCode ?? 0;
                }

                if (providerPayment.SfaCoFundedPayments != 0)
                {
                    var coFundedSfa = new SfaCoInvestedProviderPaymentEvent
                    {
                        TransactionType = providerPayment.TransactionType,
                        AmountDue = providerPayment.SfaCoFundedPayments,
                        CollectionPeriod = eventCollectionPeriod,
                        DeliveryPeriod = deliveryPeriod,
                        Learner = learner,
                        FundingSourceType = FundingSourceType.CoInvestedSfa,
                        LearningAim = new LearningAim { StandardCode = standardCode.Value}
                        EmployerAccountId = providerPayment.AccountId
                    };
                    expectedPayments.Add(coFundedSfa);
                }

                if (providerPayment.EmployerCoFundedPayments != 0)
                {
                    var coFundedEmp = new EmployerCoInvestedProviderPaymentEvent
                    {
                        TransactionType = providerPayment.TransactionType,
                        AmountDue = providerPayment.EmployerCoFundedPayments,
                        CollectionPeriod = eventCollectionPeriod,
                        DeliveryPeriod = deliveryPeriod,
                        Learner = learner,
                        FundingSourceType = FundingSourceType.CoInvestedEmployer,
                        EmployerAccountId = providerPayment.AccountId,
                        LearningAim = new LearningAim { StandardCode = standardCode.Value }
                    };
                    expectedPayments.Add(coFundedEmp);
                }

                if (providerPayment.SfaFullyFundedPayments != 0)
                {
                    var fullyFundedSfa = new SfaFullyFundedProviderPaymentEvent
                    {
                        TransactionType = providerPayment.TransactionType,
                        AmountDue = providerPayment.SfaFullyFundedPayments,
                        CollectionPeriod = eventCollectionPeriod,
                        DeliveryPeriod = deliveryPeriod,
                        Learner = learner,
                        EmployerAccountId = providerPayment.AccountId,
                        LearningAim = new LearningAim { StandardCode = standardCode.Value }
                    };
                    expectedPayments.Add(fullyFundedSfa);
                }

                if (providerPayment.LevyPayments != 0)
                {
                    var levyFunded = new LevyProviderPaymentEvent
                    {
                        TransactionType = providerPayment.TransactionType,
                        AmountDue = providerPayment.LevyPayments,
                        CollectionPeriod = eventCollectionPeriod,
                        DeliveryPeriod = deliveryPeriod,
                        Learner = learner,
                        EmployerAccountId = providerPayment.AccountId,
                        LearningAim = new LearningAim { StandardCode = standardCode.Value }
                    };
                    expectedPayments.Add(levyFunded);
                }
            }

            return expectedPayments;
        }

        protected override bool Match(ProviderPaymentEvent expected, ProviderPaymentEvent actual)
        {
            return expected.GetType() == actual.GetType() &&
                   expected.TransactionType == actual.TransactionType &&
                   expected.AmountDue == actual.AmountDue &&
                   expected.CollectionPeriod.Period == actual.CollectionPeriod.Period &&
                   expected.CollectionPeriod.AcademicYear == actual.CollectionPeriod.AcademicYear &&
                   expected.DeliveryPeriod == actual.DeliveryPeriod &&
                   expected.Learner.ReferenceNumber == actual.Learner.ReferenceNumber &&
                   expected.Learner.Uln == actual.Learner.Uln &&
                   expected.EmployerAccountId == actual.EmployerAccountId &&
                   expected.LearningAim.StandardCode == actual.LearningAim.StandardCode;
        }
        
    }
}