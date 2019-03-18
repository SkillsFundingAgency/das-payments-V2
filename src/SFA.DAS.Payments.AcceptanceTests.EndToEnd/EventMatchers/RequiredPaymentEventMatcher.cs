using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.Tests.Core.Builders;
using Payment = SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data.Payment;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class RequiredPaymentEventMatcher : BaseMatcher<RequiredPaymentEvent>
    {
        private readonly Provider provider;
        private readonly CollectionPeriod collectionPeriod;
        private readonly List<Payment> paymentSpec;
        private readonly List<Training> currentIlr;
        private readonly List<Price> currentPriceEpisodes;

        public RequiredPaymentEventMatcher(Provider provider , CollectionPeriod collectionPeriod)
        {
            this.provider = provider;
            this.collectionPeriod = collectionPeriod;
        }

        public RequiredPaymentEventMatcher(Provider provider,
            CollectionPeriod collectionPeriod,
            List<Payment> paymentSpec,
            List<Training> currentIlr, 
            List<Price> currentPriceEpisodes) : this(provider, collectionPeriod)
        {
            this.paymentSpec = paymentSpec;
            this.currentIlr = currentIlr;
            this.currentPriceEpisodes = currentPriceEpisodes;
        }

        protected override IList<RequiredPaymentEvent> GetActualEvents()
        {
            var events = RequiredPaymentEventHandler.ReceivedEvents
                .Where(e => e.Ukprn == provider.Ukprn && 
                            e.CollectionPeriod.Period== collectionPeriod.Period &&
                            e.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear &&
                            e.JobId == provider.JobId).ToList();

            var results = new List<RequiredPaymentEvent>();

            var aggregatedCoinvestmentEvents = events
                .Select(x => x as CalculatedRequiredCoInvestedAmount)
                .Where(x => x != null)
                .GroupBy(x => new
                {
                    x.CollectionPeriod,
                    x.DeliveryPeriod,
                    x.OnProgrammeEarningType,
                    x.ContractType,
                });
            foreach (var aggregatedEvent in aggregatedCoinvestmentEvents)
            {
                results.Add(new CalculatedRequiredCoInvestedAmount
                {
                    AmountDue = aggregatedEvent.Sum(x => x.AmountDue),
                    CollectionPeriod = aggregatedEvent.Key.CollectionPeriod,
                    DeliveryPeriod = aggregatedEvent.Key.DeliveryPeriod,
                    OnProgrammeEarningType = aggregatedEvent.Key.OnProgrammeEarningType,
                    ContractType = aggregatedEvent.Key.ContractType,
                });
            }

            var aggregatedLevyEvents = events
                .Select(x => x as CalculatedRequiredLevyAmount)
                .Where(x => x != null)
                .GroupBy(x => new
                {
                    x.CollectionPeriod,
                    x.DeliveryPeriod,
                    x.OnProgrammeEarningType,
                    x.ContractType,
                });
            foreach (var aggregatedEvent in aggregatedLevyEvents)
            {
                results.Add(new CalculatedRequiredLevyAmount
                {
                    AmountDue = aggregatedEvent.Sum(x => x.AmountDue),
                    CollectionPeriod = aggregatedEvent.Key.CollectionPeriod,
                    DeliveryPeriod = aggregatedEvent.Key.DeliveryPeriod,
                    OnProgrammeEarningType = aggregatedEvent.Key.OnProgrammeEarningType,
                    ContractType = aggregatedEvent.Key.ContractType,
                });
            }

            var aggregatedIncentiveEvents = events
                .Select(x => x as CalculatedRequiredIncentiveAmount)
                .Where(x => x != null)
                .GroupBy(x => new
                {
                    x.CollectionPeriod,
                    x.DeliveryPeriod,
                    x.Type,
                    x.ContractType,
                });
            foreach (var aggregatedEvent in aggregatedIncentiveEvents)
            {
                results.Add(new CalculatedRequiredIncentiveAmount
                {
                    AmountDue = aggregatedEvent.Sum(x => x.AmountDue),
                    CollectionPeriod = aggregatedEvent.Key.CollectionPeriod,
                    DeliveryPeriod = aggregatedEvent.Key.DeliveryPeriod,
                    Type = aggregatedEvent.Key.Type,
                    ContractType = aggregatedEvent.Key.ContractType,
                });
            }

            return results;
        }

        protected override IList<RequiredPaymentEvent> GetExpectedEvents()
        {
            var expectedPayments = new List<RequiredPaymentEvent>();

            var paymentsToValidate =
                paymentSpec
                    .Where(e => e.ParsedCollectionPeriod.AcademicYear == collectionPeriod.AcademicYear &&
                                e.ParsedCollectionPeriod.Period == collectionPeriod.Period )
                    .ToList();

            foreach (var payment in paymentsToValidate)
            {
                AddOnProgPayment(payment, expectedPayments, payment.OnProgramme, OnProgrammeEarningType.Learning);
                AddOnProgPayment(payment, expectedPayments, payment.Balancing, OnProgrammeEarningType.Balancing);
                AddOnProgPayment(payment, expectedPayments, payment.Completion, OnProgrammeEarningType.Completion);

                foreach (var incentiveTypeKey in payment.IncentiveValues.Keys)
                {
                    var amount = payment.IncentiveValues[incentiveTypeKey];

                    if (amount != 0)
                        expectedPayments.Add(new CalculatedRequiredIncentiveAmount
                        {
                            AmountDue = amount,
                            Type = incentiveTypeKey,
                            DeliveryPeriod = new DeliveryPeriodBuilder().WithSpecDate(payment.DeliveryPeriod).Build(),
                        });

                }    
            }

            return expectedPayments;
        }

        private void AddOnProgPayment(Payment paymentToValidate, List<RequiredPaymentEvent> expectedPayments, decimal amountDue, OnProgrammeEarningType type)
        {
            var deliveryPeriod = new DeliveryPeriodBuilder().WithSpecDate(paymentToValidate.DeliveryPeriod).Build();
            var payment = CreateContractTypeRequiredPaymentEvent(amountDue, type, deliveryPeriod);
            
            if (payment.AmountDue != 0)
                expectedPayments.Add(payment);
        }

        protected override bool Match(RequiredPaymentEvent expected, RequiredPaymentEvent actual)
        {
            if (expected.GetType() != actual.GetType())
                return false;

            return expected.DeliveryPeriod == actual.DeliveryPeriod &&
                   expected.AmountDue == actual.AmountDue &&
                   MatchAct(expected as CalculatedRequiredOnProgrammeAmount, actual as CalculatedRequiredOnProgrammeAmount) &&
                   MatchIncentive(expected as CalculatedRequiredIncentiveAmount, actual as CalculatedRequiredIncentiveAmount);
        }

        private bool MatchAct(CalculatedRequiredOnProgrammeAmount expected, CalculatedRequiredOnProgrammeAmount actual)
        {
            if (expected == null)
                return true;

            return expected.OnProgrammeEarningType == actual.OnProgrammeEarningType;
        }

        private bool MatchIncentive(CalculatedRequiredIncentiveAmount expected, CalculatedRequiredIncentiveAmount actual)
        {
            if (expected == null)
                return true;

            return expected.Type == actual.Type;
        }

        private RequiredPaymentEvent CreateContractTypeRequiredPaymentEvent(decimal amountDue, OnProgrammeEarningType onProgrammeEarningType, byte deliveryPeriod)
        {
            var contractType = EnumHelper.GetContractType(currentIlr, currentPriceEpisodes);

            switch (contractType)
            {
                case ContractType.Act1:
                    return new CalculatedRequiredLevyAmount
                    {
                        AmountDue = amountDue,
                        OnProgrammeEarningType = onProgrammeEarningType,
                        DeliveryPeriod = deliveryPeriod
                    };

                case ContractType.Act2:
                    return new CalculatedRequiredCoInvestedAmount
                    {
                        AmountDue = amountDue,
                        OnProgrammeEarningType = onProgrammeEarningType,
                        DeliveryPeriod = deliveryPeriod
                    };

                default:
                    throw new InvalidOperationException("Cannot create the RequiredPaymentMatcher invalid contract type ");
            }
        }
    }
}