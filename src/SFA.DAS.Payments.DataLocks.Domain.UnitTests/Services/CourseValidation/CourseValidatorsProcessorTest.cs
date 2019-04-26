using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.CourseValidation
{
    [TestFixture]
    public class CourseValidatorsProcessorTest
    {
        private AutoMock mocker;
        private List<ICourseValidator> courseValidators;
        private DataLockValidationModel dataLockValidationModel;

        [SetUp]
        public void Prepare()
        {
            mocker = AutoMock.GetStrict();
            var earningPeriod = new EarningPeriod
            {
                Period = 1
            };

            dataLockValidationModel = new DataLockValidationModel
            {
                EarningPeriod = earningPeriod,
                Uln = 100,
                PriceEpisode = new PriceEpisode(),
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{ Id  = 99, ApprenticeshipId = 1, Cost = 100, StartDate = DateTime.Today}
                    }
                }
            };

            courseValidators = new List<ICourseValidator>
            {
                mocker.Mock<IStartDateValidator>().Object,
                mocker.Mock<INegotiatedPriceValidator>().Object,
                mocker.Mock<IApprenticeshipPauseValidator>().Object,
            };
            mocker.Provide<IEnumerable<ICourseValidator>>(courseValidators);
        }

        [Test]
        public void UsesAllCourseValidators()
        {
            mocker.Mock<IStartDateValidator>()
                .Setup(validator => validator.Validate(It.IsAny<DataLockValidationModel>()))
                .Returns(() => new ValidationResult
                {
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 51},
                    }
                });
            mocker.Mock<INegotiatedPriceValidator>()
                .Setup(validator => validator.Validate(It.IsAny<DataLockValidationModel>()))
                .Returns(() => new ValidationResult
                {
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 52}
                    }
                });
            mocker.Mock<IApprenticeshipPauseValidator>()
                .Setup(validator => validator.Validate(It.IsAny<DataLockValidationModel>()))
                .Returns(() => new ValidationResult
                {
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 53}
                    }
                });

            var courseValidator = mocker.Create<CourseValidationProcessor>();
            courseValidator.ValidateCourse(dataLockValidationModel);
            mocker.Mock<IStartDateValidator>()
                .Verify(x => x.Validate(It.IsAny<DataLockValidationModel>()), Times.Once);
            mocker.Mock<INegotiatedPriceValidator>()
                .Verify(x => x.Validate(It.IsAny<DataLockValidationModel>()), Times.Once);
            mocker.Mock<IApprenticeshipPauseValidator>()
                .Verify(x => x.Validate(It.IsAny<DataLockValidationModel>()), Times.Once);
        }

        [Test]
        public void PassesAllApprenticeshipPriceEpisodesToEachValidator()
        {
            dataLockValidationModel.Apprenticeship = new ApprenticeshipModel
            {
                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel {Id = 50},
                    new ApprenticeshipPriceEpisodeModel {Id = 51},
                    new ApprenticeshipPriceEpisodeModel {Id = 52}
                }
            };

            mocker.Mock<IStartDateValidator>()
                .Setup(validator => validator.Validate(It.IsAny<DataLockValidationModel>()))
                .Returns(() => new ValidationResult
                {
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                            new ApprenticeshipPriceEpisodeModel{Id = 51},
                            new ApprenticeshipPriceEpisodeModel{Id = 52}
                    }
                });
            mocker.Mock<INegotiatedPriceValidator>()
                .Setup(validator => validator.Validate(It.IsAny<DataLockValidationModel>()))
                .Returns(() => new ValidationResult
                {
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                            new ApprenticeshipPriceEpisodeModel{Id = 52}
                    }
                });
            mocker.Mock<IApprenticeshipPauseValidator>()
                .Setup(validator => validator.Validate(It.IsAny<DataLockValidationModel>()))
                .Returns(() => new ValidationResult
                {
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                            new ApprenticeshipPriceEpisodeModel{Id = 52}
                    }
                });
            var courseValidator = mocker.Create<CourseValidationProcessor>();
            courseValidator.ValidateCourse(dataLockValidationModel);
            mocker.Mock<IStartDateValidator>()
                .Verify(validator => validator.Validate(It.Is<DataLockValidationModel>(model =>
                    model.Apprenticeship.ApprenticeshipPriceEpisodes.All(ape => ape.Id == 50 || ape.Id == 51 || ape.Id == 52) &&
                    model.Apprenticeship.ApprenticeshipPriceEpisodes.Count == 3)), Times.Once);

            mocker.Mock<INegotiatedPriceValidator>()
                .Verify(validator => validator.Validate(It.Is<DataLockValidationModel>(model =>
                    model.Apprenticeship.ApprenticeshipPriceEpisodes.All(ape => ape.Id == 50 || ape.Id == 51 || ape.Id == 52) &&
                    model.Apprenticeship.ApprenticeshipPriceEpisodes.Count == 3)), Times.Once);

            mocker.Mock<IApprenticeshipPauseValidator>()
                .Verify(validator => validator.Validate(It.Is<DataLockValidationModel>(model =>
                    model.Apprenticeship.ApprenticeshipPriceEpisodes.All(ape => ape.Id == 50 || ape.Id == 51 || ape.Id == 52) &&
                    model.Apprenticeship.ApprenticeshipPriceEpisodes.Count == 3)), Times.Once);

        }

        [Test]
        public void ReturnsMatchedApprenticeshipPriceEpisode()
        {
            dataLockValidationModel.Apprenticeship = new ApprenticeshipModel
            {
                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel {Id = 50},
                    new ApprenticeshipPriceEpisodeModel {Id = 51},
                    new ApprenticeshipPriceEpisodeModel {Id = 52},
                    new ApprenticeshipPriceEpisodeModel {Id = 53},
                    new ApprenticeshipPriceEpisodeModel {Id = 54},
                }
            };
            mocker.Mock<IStartDateValidator>()
                .Setup(validator => validator.Validate(It.IsAny<DataLockValidationModel>()))
                .Returns(() => new ValidationResult
                {
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 51},
                        new ApprenticeshipPriceEpisodeModel{Id = 52}
                    }
                });
            mocker.Mock<INegotiatedPriceValidator>()
                .Setup(validator => validator.Validate(It.IsAny<DataLockValidationModel>()))
                .Returns(() => new ValidationResult
                {
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 52},
                        new ApprenticeshipPriceEpisodeModel{Id = 53},
                    }
                });
            mocker.Mock<IApprenticeshipPauseValidator>()
                .Setup(validator => validator.Validate(It.IsAny<DataLockValidationModel>()))
                .Returns(() => new ValidationResult
                {
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 52},
                        new ApprenticeshipPriceEpisodeModel{Id = 54}
                    }
                });
            var courseValidator = mocker.Create<CourseValidationProcessor>();
            var result = courseValidator.ValidateCourse(dataLockValidationModel);

            mocker.Mock<IStartDateValidator>()
             .Verify(validator => validator.Validate(It.IsAny<DataLockValidationModel>()), Times.Once);

            mocker.Mock<INegotiatedPriceValidator>()
                 .Verify(validator => validator.Validate(It.IsAny<DataLockValidationModel>()), Times.Once);

            mocker.Mock<IApprenticeshipPauseValidator>()
                 .Verify(validator => validator.Validate(It.IsAny<DataLockValidationModel>()), Times.Once);


            result.MatchedPriceEpisode.Should().NotBeNull();
            result.MatchedPriceEpisode.Id.Should().Be(52);
        }

        [Test]
        public void ReturnsNoPriceEpisodeIfThereWereFailures()
        {
            dataLockValidationModel.Apprenticeship = new ApprenticeshipModel
            {
                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel {Id = 50},
                    new ApprenticeshipPriceEpisodeModel {Id = 51},
                    new ApprenticeshipPriceEpisodeModel {Id = 52},
                    new ApprenticeshipPriceEpisodeModel {Id = 53},
                    new ApprenticeshipPriceEpisodeModel {Id = 54},
                }
            };
            mocker.Mock<IStartDateValidator>()
                .Setup(validator => validator.Validate(It.IsAny<DataLockValidationModel>()))
                .Returns(() => new ValidationResult
                {
                    DataLockErrorCode = DataLockErrorCode.DLOCK_07,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                    }
                });
            mocker.Mock<INegotiatedPriceValidator>()
                .Setup(validator => validator.Validate(It.IsAny<DataLockValidationModel>()))
                .Returns(() => new ValidationResult
                {
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 52},
                    }
                });
            mocker.Mock<IApprenticeshipPauseValidator>()
                .Setup(validator => validator.Validate(It.IsAny<DataLockValidationModel>()))
                .Returns(() => new ValidationResult
                {
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 52},
                    }
                });
            var courseValidator = mocker.Create<CourseValidationProcessor>();
            var result = courseValidator.ValidateCourse(dataLockValidationModel);

            mocker.Mock<IStartDateValidator>()
               .Verify(validator => validator.Validate(It.IsAny<DataLockValidationModel>()), Times.Once);

            mocker.Mock<INegotiatedPriceValidator>()
                 .Verify(validator => validator.Validate(It.IsAny<DataLockValidationModel>()), Times.Once);

            mocker.Mock<IApprenticeshipPauseValidator>()
                 .Verify(validator => validator.Validate(It.IsAny<DataLockValidationModel>()), Times.Once);

            result.MatchedPriceEpisode.Should().BeNull();
        }

        [Test]
        public void ReturnsAllDataLockFailures()
        {
            dataLockValidationModel.Apprenticeship = new ApprenticeshipModel
            {
                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel {Id = 50},
                    new ApprenticeshipPriceEpisodeModel {Id = 51},
                    new ApprenticeshipPriceEpisodeModel {Id = 52},
                    new ApprenticeshipPriceEpisodeModel {Id = 53},
                    new ApprenticeshipPriceEpisodeModel {Id = 54},
                }
            };
            mocker.Mock<IStartDateValidator>()
                .Setup(validator => validator.Validate(It.IsAny<DataLockValidationModel>()))
                .Returns(() => new ValidationResult
                {
                    DataLockErrorCode = DataLockErrorCode.DLOCK_07,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                    }
                });
            mocker.Mock<INegotiatedPriceValidator>()
                .Setup(validator => validator.Validate(It.IsAny<DataLockValidationModel>()))
                .Returns(() => new ValidationResult
                {
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 52},
                    }
                });
            mocker.Mock<IApprenticeshipPauseValidator>()
                .Setup(validator => validator.Validate(It.IsAny<DataLockValidationModel>()))
                .Returns(() => new ValidationResult
                {
                    DataLockErrorCode = DataLockErrorCode.DLOCK_12,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                    }
                });
            var courseValidator = mocker.Create<CourseValidationProcessor>();
            var result = courseValidator.ValidateCourse(dataLockValidationModel);

            mocker.Mock<IStartDateValidator>()
             .Verify(validator => validator.Validate(It.IsAny<DataLockValidationModel>()), Times.Once);

            mocker.Mock<INegotiatedPriceValidator>()
                 .Verify(validator => validator.Validate(It.IsAny<DataLockValidationModel>()), Times.Once);

            mocker.Mock<IApprenticeshipPauseValidator>()
                 .Verify(validator => validator.Validate(It.IsAny<DataLockValidationModel>()), Times.Once);


            result.DataLockErrors.Should().Contain(DataLockErrorCode.DLOCK_07);
            result.DataLockErrors.Should().Contain(DataLockErrorCode.DLOCK_12);
            result.DataLockErrors.Should().HaveCount(2);
        }
    }
}
