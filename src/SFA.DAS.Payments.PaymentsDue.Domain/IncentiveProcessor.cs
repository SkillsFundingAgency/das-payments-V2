﻿using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Domain
{
    public class IncentiveProcessor : IIncentiveProcessor
    {
        public IncentivePaymentDueEvent[] HandleIncentiveEarnings(long ukprn, long jobId,
            IncentiveEarning incentiveEarning, CalendarPeriod collectionPeriod, Learner learner,
            LearningAim learningAim,
            decimal sfaContributionPercentage, DateTime ilrSubmissionDate, ContractType contractType)
        {
            if (incentiveEarning == null)
                throw new ArgumentNullException(nameof(incentiveEarning));

            if (collectionPeriod == null)
                throw new ArgumentNullException(nameof(collectionPeriod));

            if (learner == null)
                throw new ArgumentNullException(nameof(learner));

            if (learningAim == null)
                throw new ArgumentNullException(nameof(learningAim));

            var paymentsDue = new List<IncentivePaymentDueEvent>();

            foreach (var period in incentiveEarning.Periods.Where(earning => earning.Period <= collectionPeriod.Period))
            {
                paymentsDue.Add(new IncentivePaymentDueEvent
                {
                    JobId = jobId,
                    Ukprn = ukprn,
                    DeliveryPeriod =
                        new CalendarPeriod(collectionPeriod.Name.Split('-').FirstOrDefault(),
                            period.Period),
                    LearningAim = learningAim.Clone(),
                    Learner = learner.Clone(),
                    AmountDue = period.Amount,
                    CollectionPeriod = collectionPeriod,
                    EventTime = DateTimeOffset.UtcNow,
                    PriceEpisodeIdentifier = period.PriceEpisodeIdentifier,
                    IlrSubmissionDateTime = ilrSubmissionDate,
                    Type = incentiveEarning.Type,
                    ContractType = contractType
                });
            }

            return paymentsDue.ToArray();
        }
    }
}