using System;
using System.Collections.Generic;
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


        [TestCase("Stop date from a prior period should produce DLOCK_10", TransactionType.Completion, "2020/07/01", "2019/07/31")]
        [TestCase("Stop date from a prior period should produce DLOCK_10", TransactionType.Balancing, "2020/07/01", "2019/07/31")]
        [TestCase("Stop date is before end date and should produce DLOCK_10", TransactionType.Completion, "2019/08/25", "2019/08/20")]
        [TestCase("Stop date is before end date and should produce DLOCK_10", TransactionType.Balancing, "2019/08/25", "2019/08/20")]
        public void ScenariosThatCreateDlockErrors(string errorMessage, TransactionType transactionType, DateTime ilrDate, DateTime commitmentDate)
        {
            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode
                {
                    Identifier = PriceEpisodeIdentifier,
                    ActualEndDate = ilrDate,
                },
                EarningPeriod = period,
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1 ,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 100,
                        },
                    },
                    StopDate = commitmentDate, // Last day of 1819
                    Status = ApprenticeshipStatus.Stopped,
                },
                AcademicYear = 1920,
                TransactionType = transactionType,
            };

            var validator = new CompletionStoppedValidator();
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_10, errorMessage);
            result.ApprenticeshipPriceEpisodes.Should().BeEmpty(errorMessage);
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
            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode
                {
                    Identifier = PriceEpisodeIdentifier,
                    ActualEndDate = ilrDate,
                },
                EarningPeriod = period,
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1,
                    Status = ApprenticeshipStatus.Stopped,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 100,
                        },
                    },
                    StopDate = commitmentDate, 
                },
                AcademicYear = 1920,
                TransactionType = transactionType,
            };

            var validator = new CompletionStoppedValidator();
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().BeNull();
            result.ApprenticeshipPriceEpisodes.Should().HaveCount(1);
            result.ApprenticeshipPriceEpisodes[0].Id.Should().Be(100);
        }
    }
}
