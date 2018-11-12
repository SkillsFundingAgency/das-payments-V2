using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Messages;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class ProviderPaymentEventMatcher
    {
        public static Tuple<bool, string> MatchPayments(List<ProviderPayment> expectedPayments, long ukprn, string learnerReference, long jobId, CalendarPeriod collectionPeriod)
        {
            expectedPayments = expectedPayments
                .Where(p => p.CollectionPeriod.ToDate().ToCalendarPeriod().Name == collectionPeriod.Name)
                .ToList();
            var receivedEvents = ProviderPaymentEventHandler.ReceivedEvents
                .Where(ev =>
                    ev.Ukprn == ukprn &&
                    ev.Learner.ReferenceNumber == learnerReference &&
                    ev.JobId == jobId)
                .ToList();

            if (!expectedPayments
                .Where(expected => expected.SfaCoFundedPayments != 0)
                .All(expected => receivedEvents.Any(receivedEvent =>
                    receivedEvent is SfaCoInvestedProviderPaymentEvent &&
                    receivedEvent.TransactionType == expected.TransactionType &&
                    receivedEvent.AmountDue == expected.SfaCoFundedPayments &&
                    receivedEvent.CollectionPeriod.Name == expected.CollectionPeriod.ToDate().ToCalendarPeriod().Name &&
                    receivedEvent.DeliveryPeriod.Period == expected.DeliveryPeriod.ToDate().ToCalendarPeriod().Period)))
            {
                return new Tuple<bool, string>(false, "Failed to find all sfa co-funded payments.");
            }

            if (!expectedPayments
                .Where(expected => expected.EmployerCoFundedPayments != 0)
                .All(expected => receivedEvents.Any(receivedEvent =>
                    receivedEvent is EmployerCoInvestedProviderPaymentEvent &&
                    receivedEvent.TransactionType == expected.TransactionType &&
                    receivedEvent.AmountDue == expected.EmployerCoFundedPayments &&
                    receivedEvent.CollectionPeriod.Name == expected.CollectionPeriod.ToDate().ToCalendarPeriod().Name &&
                    receivedEvent.DeliveryPeriod.Period == expected.DeliveryPeriod.ToDate().ToCalendarPeriod().Period)))
            {
                return new Tuple<bool, string>(false, "Failed to find all employer co-funded payments.");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        public static Tuple<bool, string> MatchRecordedPayments(IPaymentsDataContext dataContext, List<ProviderPayment> expectedPaymentInfo, TestSession testSession, List<Training> currentIlr, CalendarPeriod currentCollectionPeriod)
        {
            var learnerLearnRefNumber = testSession.Learner.LearnRefNumber;
            var name = currentCollectionPeriod.Name;

            var actualPayments = dataContext.Payment.Where(p => p.JobId == testSession.JobId &&
                                                          p.LearnerReferenceNumber == learnerLearnRefNumber &&
                                                          p.CollectionPeriod.Name == name).ToList();

            var expectedPayments = new List<PaymentModel>();
            foreach (var paymentInfo in expectedPaymentInfo)
            {
                var coFundedSfa = ToPaymentModel(paymentInfo, testSession.Ukprn, currentIlr.First().ContractType);
                coFundedSfa.Amount = paymentInfo.SfaCoFundedPayments;
                coFundedSfa.FundingSource = FundingSourceType.CoInvestedSfa;

                var coFundedEmp = ToPaymentModel(paymentInfo, testSession.Ukprn, currentIlr.First().ContractType);
                coFundedEmp.Amount = paymentInfo.EmployerCoFundedPayments;
                coFundedEmp.FundingSource = FundingSourceType.CoInvestedEmployer;
            
                expectedPayments.Add(coFundedSfa);
                expectedPayments.Add(coFundedEmp);
            }

            var matchedPayments = expectedPayments
                .Where(expected => actualPayments.Any(p => expected.CollectionPeriod == p.CollectionPeriod &&
                                                           expected.TransactionType == p.TransactionType &&
                                                           expected.ContractType == p.ContractType &&
                                                           expected.FundingSource == p.FundingSource &&
                                                           expected.Amount == p.Amount))
                .ToList();

            var errors = new List<string>();

            if (matchedPayments.Count < expectedPayments.Count)
                errors.Add($"{expectedPayments.Count - matchedPayments.Count} out of {expectedPayments.Count} were not found");

            if (matchedPayments.Count > expectedPayments.Count)
                errors.Add($"found {matchedPayments.Count - expectedPayments.Count} unexpected payments");

            return new Tuple<bool, string>(errors.Count == 0, string.Join(", ", errors));
        }

        private static PaymentModel ToPaymentModel(ProviderPayment paymentInfo, long ukprn, ContractType contractType)
        {
            return new PaymentModel
            {
                CollectionPeriod = paymentInfo.CollectionPeriod.ToCalendarPeriod(),
                Ukprn = ukprn,
                DeliveryPeriod = paymentInfo.DeliveryPeriod.ToCalendarPeriod(),
                TransactionType = paymentInfo.TransactionType,
                ContractType = contractType,
                Amount = paymentInfo.EmployerCoFundedPayments,
                FundingSource = FundingSourceType.CoInvestedEmployer
            };
        }
    }
}