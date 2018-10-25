using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using OnProgrammeEarning = SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data.OnProgrammeEarning;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public static class EarningEventMatcher
    {
        public static Tuple<bool, string> MatchEarnings(IList<OnProgrammeEarning> expectedEarnings, long ukprn)
        {
            var sessionEarnings = ApprenticeshipContractType2EarningEventHandler.ReceivedEvents.Where(e => e.Ukprn == ukprn).ToArray();
            var receivedPeriods = ConvertToOnProgEarning(sessionEarnings);

            var matchedEvents = receivedPeriods.Select(received => expectedEarnings.Any(expected =>
            {
                return expected.DeliveryCalendarPeriod.Name == received.Key
                       && expected.Balancing == received.Value.Balancing
                       && expected.Completion == received.Value.Completion
                       && expected.OnProgramme == received.Value.OnProgramme;
            })).ToArray();

            return new Tuple<bool, string>(matchedEvents.Length == receivedPeriods.Count, "");
        }

        private static Dictionary<string, OnProgrammeEarning> ConvertToOnProgEarning(ApprenticeshipContractType2EarningEvent[] sessionEarnings)
        {
            var receivedPeriods = sessionEarnings.SelectMany(e => e.OnProgrammeEarnings.SelectMany(pe => pe.Periods.Select(p => p.Period))).Distinct().ToDictionary(p => p.Name, p => new OnProgrammeEarning {DeliveryCalendarPeriod = p});

            foreach (var receivedEarning in sessionEarnings.SelectMany(e => e.OnProgrammeEarnings))
            {
                foreach (var period in receivedEarning.Periods)
                {
                    var onProg = receivedPeriods[period.Period.Name];
                    switch (receivedEarning.Type)
                    {
                        case OnProgrammeEarningType.Learning:
                            onProg.OnProgramme = period.Amount;
                            break;
                        case OnProgrammeEarningType.Balancing:
                            onProg.Balancing = period.Amount;
                            break;
                        case OnProgrammeEarningType.Completion:
                            onProg.Completion = period.Amount;
                            break;
                        default:
                            throw new NotSupportedException("Unknown earning type " + receivedEarning.Type);
                    }
                }
            }

            return receivedPeriods;
        }
    }
}
