using System;
using System.Collections.Generic;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.PaymentsDue.Domain.Entities;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Domain
{
    public class ApprenticeshipContractType2EarningProcessor : IApprenticeshipContractType2EarningProcessor
    {
        public ApprenticeshipContractType2PaymentDueEvent[] HandleOnProgrammeEarning(OnProgrammeEarning onProgEarning, CalendarPeriod collectionPeriod, Learner learner, LearningAim learningAim, decimal sfaContributionPercentage)
        {
            if (onProgEarning == null)
                throw new ArgumentNullException(nameof(onProgEarning));

            if (collectionPeriod == null)
                throw new ArgumentNullException(nameof(collectionPeriod));

            if (learner == null)
                throw new ArgumentNullException(nameof(learner));

            if (learningAim == null)
                throw new ArgumentNullException(nameof(learningAim));

            var paymentsDue = new List<ApprenticeshipContractType2PaymentDueEvent>();

                foreach (var period in onProgEarning.Periods)
                {
                    if (period.Period.Year > collectionPeriod.Year
                        || (period.Period.Year == collectionPeriod.Year
                            && period.Period.Month > collectionPeriod.Month)
                        || period.Amount == 0)
                        continue; // cut future and empty periods off
                    
                    paymentsDue.Add(new ApprenticeshipContractType2PaymentDueEvent
                    {
                        Ukprn = learner.Ukprn,
                        DeliveryPeriod = period.Period,
                        LearningAim = learningAim.Clone(),
                        Learner = learner.Clone(),
                        Type = onProgEarning.Type,
                        SfaContributionPercentage = sfaContributionPercentage,
                        AmountDue = period.Amount,
                        CollectionPeriod = collectionPeriod,
                        EventTime = DateTimeOffset.UtcNow,
                        PriceEpisodeIdentifier = period.PriceEpisodeIdentifier
                    });
                }

            return paymentsDue.ToArray();
        }
    }
}
