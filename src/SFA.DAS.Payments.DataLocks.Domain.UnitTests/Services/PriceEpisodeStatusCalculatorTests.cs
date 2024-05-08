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

            calc.DetermineStatus(2324, priceEpisode, earnings, new List<PriceEpisodeStatusChange>()).Should().Be(PriceEpisodeStatus.New);
        }

        [Test]
        public void Price_Episode_Not_Found_In_Previous_Price_Episodes_Returns_New()
        {
            var calc = mocker.Create<PriceEpisodeStatusCalculator>();

            var priceEpisode = new PriceEpisode
            {
                Identifier = "1234",
                AgreedPrice = 1000, 
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

            var previousPriceEpisodeStatuses = new List<PriceEpisodeStatusChange>
            {
                new PriceEpisodeStatusChange
                {
                    DataLock = new LegacyDataLockEvent{ PriceEpisodeIdentifier = "4321" , AcademicYear = "2324" }
                }
            };

            calc.DetermineStatus(2324,priceEpisode, earnings, previousPriceEpisodeStatuses).Should().Be(PriceEpisodeStatus.New);
        }

        [Test]
        public void Price_Episode_Not_Found_In_Current_Year_Returns_New()
        {
            var calc = mocker.Create<PriceEpisodeStatusCalculator>();

            var priceEpisode = new PriceEpisode
            {
                Identifier = "1234",
                AgreedPrice = 1000,
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

            var previousPriceEpisodeStatuses = new List<PriceEpisodeStatusChange>
            {
                new PriceEpisodeStatusChange
                {
                    DataLock = new LegacyDataLockEvent{ PriceEpisodeIdentifier = "1234" , AcademicYear = "2223" }, 
                    AgreedPrice = 1000
                }
            };

            calc.DetermineStatus(2324, priceEpisode, earnings, previousPriceEpisodeStatuses).Should().Be(PriceEpisodeStatus.New);
        }
    }
}