using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.CourseValidation
{
    [TestFixture]
    public class CompletionAndBalancingStoppedValidatorTests
    {
        private const string PriceEpisodeIdentifier = "pe-1";
        private EarningPeriod period;

        [OneTimeSetUp]
        public void Setup()
        {
            period = new EarningPeriod
            {
                Period = 1,
            };
        }


        [AutoData]
        public void ReturnsOnlyCommitmentsThatAreNotStopped(ApprenticeshipModel apprenticeshipA, ApprenticeshipModel apprenticeshipB, PriceEpisode priceEpisode)
        {
            priceEpisode.EffectiveTotalNegotiatedPriceStartDate = new DateTime(2018, 8, 1);
            priceEpisode.ActualEndDate = new DateTime(2020, 8, 30);

            apprenticeshipA.StopDate = new DateTime(2019, 8, 1);
            apprenticeshipA.Status = ApprenticeshipStatus.Stopped;
            apprenticeshipA.ApprenticeshipPriceEpisodes[0].StartDate = new DateTime(2019, 8, 1);
            apprenticeshipA.ApprenticeshipPriceEpisodes[0].EndDate = new DateTime(2020, 8, 30);

            apprenticeshipB.Status = ApprenticeshipStatus.Active;
            apprenticeshipB.ApprenticeshipPriceEpisodes[0].StartDate = new DateTime(2018, 8, 1);
            apprenticeshipB.ApprenticeshipPriceEpisodes[0].EndDate = new DateTime(2019, 8, 30);

            var apprenticeships = new List<ApprenticeshipModel> { apprenticeshipA, apprenticeshipB };

            var validator = new CompletionStoppedValidator();
            (List<ApprenticeshipModel> validApprenticeships, List<DataLockFailure> dataLockFailures) result = validator.Validate(priceEpisode, apprenticeships, TransactionType.Completion);

            result.dataLockFailures.Should().BeEmpty();
            result.validApprenticeships.Should().NotBeNull();
            result.validApprenticeships.All(x => x.Id == apprenticeshipB.Id).Should().BeTrue();
        }

        [TestCase("Stop date from a prior period should produce DLOCK_10", TransactionType.Completion, "2020/07/01", "2019/07/31")]
        [TestCase("Stop date from a prior period should produce DLOCK_10", TransactionType.Balancing, "2020/07/01", "2019/07/31")]
        [TestCase("Stop date is before end date and should produce DLOCK_10", TransactionType.Completion, "2019/08/25", "2019/08/20")]
        [TestCase("Stop date is before end date and should produce DLOCK_10", TransactionType.Balancing, "2019/08/25", "2019/08/20")]
        public void ScenariosThatCreateDlockErrors(string errorMessage, TransactionType transactionType, DateTime ilrDate, DateTime commitmentDate)
        {
            var priceEpisode = new PriceEpisode
            {
                Identifier = PriceEpisodeIdentifier,
                ActualEndDate = ilrDate,
            };
            var apprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    Id = 1,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 100,
                        },
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 200,
                            Removed = true
                        },
                    },
                    StopDate = commitmentDate, // Last day of 1819
                    Status = ApprenticeshipStatus.Stopped,
                }
            };
        
            var validator = new CompletionStoppedValidator();
            var result = validator.Validate(priceEpisode,apprenticeships,transactionType);
            result.validApprenticeships.Should().BeEmpty(errorMessage);
            result.dataLockFailures.Should().HaveCount(1);
            result.dataLockFailures[0].DataLockError.Should().Be(DataLockErrorCode.DLOCK_10);
            result.dataLockFailures[0].ApprenticeshipId.Should().Be(1);

            result.dataLockFailures[0].ApprenticeshipPriceEpisodeIds.Should().HaveCount(1);
            result.dataLockFailures[0].ApprenticeshipPriceEpisodeIds[0].Should().Be(100);

        }

        [TestCase("When commitment stop date is after the end date then no datalock expected", TransactionType.Completion, "2019/08/15", "2019/08/31")]
        [TestCase("When commitment stop date is after the end date then no datalock expected", TransactionType.Balancing, "2019/08/15", "2019/08/31")]
        [TestCase("When commitment stop date is the same as the end date then no datalock expected", TransactionType.Completion, "2019/08/15", "2019/08/15")]
        [TestCase("When commitment stop date is the same as the end date then no datalock expected", TransactionType.Balancing, "2019/08/15", "2019/08/15")]
        [TestCase("When commitment stop date is in a future period then no datalock expected", TransactionType.Completion, "2020/07/01", "2020/08/01")]
        [TestCase("When commitment stop date is in a future period then no datalock expected", TransactionType.Balancing, "2020/07/01", "2020/08/01")]
        [TestCase("Stop date is before end date but transaction type is LearningSupport then no datalock expected", TransactionType.LearningSupport, "2019/08/25", "2019/08/20")]
        [TestCase("Stop date is before end date, but transaction type is First16To18ProviderIncentive then no datalock expected", TransactionType.First16To18ProviderIncentive, "2019/08/25", "2019/08/20")]
        public void ScenariosThatDoNotCreateDlockErrors(string errorMessage, TransactionType transactionType, DateTime ilrDate, DateTime commitmentDate)
        {
            var priceEpisode = new PriceEpisode
            {
                Identifier = PriceEpisodeIdentifier,
                ActualEndDate = ilrDate,
            };
            var apprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    Id = 2,
                    Status = ApprenticeshipStatus.Stopped,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 200,
                        }
                    },
                    StopDate = commitmentDate,
                },
            };

            var validator = new CompletionStoppedValidator();
            var result = validator.Validate(priceEpisode, apprenticeships, transactionType);
            result.dataLockFailures.Should().BeEmpty();
            result.validApprenticeships[0].Id.Should().Be(2);
            result.validApprenticeships[0].ApprenticeshipPriceEpisodes[0].Id.Should().Be(200);
        }
    }
}
