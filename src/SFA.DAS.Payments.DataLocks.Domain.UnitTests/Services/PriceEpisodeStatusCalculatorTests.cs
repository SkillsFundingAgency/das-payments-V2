using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autofac.Extras.Moq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpisodeChanges;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services
{
    [TestFixture]
    public class PriceEpisodeStatusCalculatorTests
    {
        private AutoMock mocker;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
        }


        [Test]
        public void No_Previous_Price_Episode_Returns_New()
        {
            var calc = mocker.Create<PriceEpisodeStatusCalculator>();

            var priceEpisode = new PriceEpisode
            {
                Identifier = "1234",
                AgreedPrice = 1000
            };

            var earnings = new List<OnProgrammeEarning>
            {
                new OnProgrammeEarning
                {
                    Type = OnProgrammeEarningType.Learning,
                    Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                    {
                        new EarningPeriod
                        {
                            AccountId = 12, Amount = 100, ApprenticeshipId = 1, Period = 1,
                            DataLockFailures = new List<DataLockFailure>()
                        }
                    })
                }

            };

            calc.DetermineStatus(priceEpisode, earnings, new List<PriceEpisodeStatusChange>()).Should().Be(PriceEpisodeStatus.New);
        }
    }
}