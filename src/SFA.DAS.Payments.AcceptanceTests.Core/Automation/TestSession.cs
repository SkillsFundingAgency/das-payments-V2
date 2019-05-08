using Bogus;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.Services;
using SFA.DAS.Payments.AcceptanceTests.Services.Intefaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public class TestSession
    {
        public LearnRefNumberGenerator LearnRefNumberGenerator { get; }
        public string SessionId { get; }
        public List<Learner> Learners { get; }
        public Learner Learner => GetLearner(Provider.Ukprn, "learner a");
        public Employer Employer => GetEmployer("test employer");

        public DateTime IlrSubmissionTime { get; set; }

        public bool AtLeastOneScenarioCompleted { get; private set; }

        public List<Employer> Employers { get; }
        private readonly Random random;
        private readonly Faker<Course> courseFaker;
        
        public List<Provider> Providers { get; }
        public Provider Provider => GetProviderByIdentifier("Test Provider");
        public long Ukprn => Provider.Ukprn;
        public long JobId => Provider.JobId;

        private readonly IUkprnService _ukprnService;

        public Employer GetEmployer(string identifier)
        {
            if (identifier == null) return Employer;

            var employer = Employers.SingleOrDefault(x => x.Identifier == identifier);
            if (employer == null)
            {
                employer = GenerateEmployer().Generate(1).First();
                employer.Identifier = identifier;
                Employers.Add(employer);
            }

            return employer;
        }

        public TestSession(IUkprnService ukprnService)
        {
            _ukprnService = ukprnService;

            courseFaker = new Faker<Course>();
            courseFaker
                .RuleFor(course => course.AimSeqNumber, faker => faker.Random.Short(1))
                .RuleFor(course => course.FrameworkCode, faker => faker.Random.Short(1))
                .RuleFor(course => course.FundingLineType, faker => faker.Name.JobDescriptor() ?? "FundingLine")
                .RuleFor(course => course.LearnAimRef, "ZPROG001")
                .RuleFor(course => course.LearningPlannedEndDate, DateTime.Today.AddMonths(12))
                .RuleFor(course => course.LearningStartDate, DateTime.Today)
                .RuleFor(course => course.PathwayCode, faker => faker.Random.Short(1))
                .RuleFor(course => course.ProgrammeType, faker => faker.Random.Short(1))
                .RuleFor(course => course.StandardCode, faker => faker.Random.Int(1))
                .RuleFor(course => course.AgreedPrice, 15000);

            SessionId = Guid.NewGuid().ToString();
            random = new Random(Guid.NewGuid().GetHashCode());

            Providers = new List<Provider>();
            IlrSubmissionTime = Provider.IlrSubmissionTime;
            Learners = new List<Learner>();
            LearnRefNumberGenerator = new LearnRefNumberGenerator(SessionId);
            Employers = new List<Employer>();

        }

        public TestSession(long? ukprn = null) : this(new RandomUkprnService())
        {
        }

        public long GenerateId(int maxValue = 1000000)
        {
            //return _ukprnService.GenerateUkprn();

            var id = random.Next(maxValue);
            //TODO: make sure that the id isn't already in use.
            return id;
        }

        public void RegenerateUkprn()
        {
            Provider.Ukprn = _ukprnService.GenerateUkprn();
        }

        private Provider GenerateProvider()
        {
            return new Provider
            {
                Ukprn = _ukprnService.GenerateUkprn(),
                JobId = GenerateId(),
                IlrSubmissionTime = DateTime.UtcNow
            };
        }

        public Provider GetProviderByIdentifier(string identifier)
        {
            if (identifier == null) return Provider;

            var provider = Providers.SingleOrDefault(x => x.Identifier == identifier);
            if (provider == null)
            {
                provider = GenerateProvider();
                provider.Identifier = identifier;
                Providers.Add(provider);
            }

            return provider;
        }

        public string GenerateLearnerReference(string learnerId)
        {
            return string.IsNullOrEmpty(learnerId) ? Learner.LearnRefNumber : LearnRefNumberGenerator.Generate(Ukprn, learnerId);
        }

        public Learner GenerateLearner(long ukprn, long? uniqueLearnerNumber = null)
        {
            var uln = uniqueLearnerNumber ?? GenerateId();
            return new Learner
            {
                Ukprn = ukprn,
                Uln = uln,
                LearnRefNumber = GenerateId().ToString(),
                Course = courseFaker.Generate(1).FirstOrDefault()
            };
        }

        public Learner GetLearner(long ukprn, string learnerIdentifier)
        {
            var learner = Learners.FirstOrDefault(l => l.LearnerIdentifier == learnerIdentifier && l.Ukprn == ukprn);
            if (learner == null)
            {
                var uln = GetUlnFromLearnerId(learnerIdentifier);
                learner = GenerateLearner(ukprn, uln);
                learner.LearnerIdentifier = learnerIdentifier;
                Learners.Add(learner);
            }

            return learner;
        }

        public long GetUlnFromLearnerId(string learnerId)
        {
            var learner = Learners.FirstOrDefault(x => x.LearnerIdentifier == learnerId);
            if (learner == null)
            {
                return GenerateId();
            }

            return learner.Uln;
        }

        public void CompleteScenario()
        {
            AtLeastOneScenarioCompleted = true;
            Providers.ForEach(x => x.MonthEndJobIdGenerated = false);
        }

        private Faker<Employer> GenerateEmployer()
        {
            var fakeEmployer = new Faker<Employer>();

            fakeEmployer
                // TODO: uncomment this when DataLock populates EmployerAccountId on PayableEarningEvent
                .RuleFor(employer => employer.AccountId, faker => faker.Random.Long(1, long.MaxValue))
                .RuleFor(employer => employer.AccountHashId, faker => faker.Random.Long(1, long.MaxValue).ToString())
                .RuleFor(employer => employer.AccountName, faker => faker.Company.CompanyName())
                .RuleFor(employer => employer.Balance, faker => faker.Random.Decimal())
                .RuleFor(employer => employer.SequenceId, faker => faker.Random.Long(1, long.MaxValue))
                .RuleFor(employer => employer.IsLevyPayer, true)
                .RuleFor(employer => employer.TransferAllowance, 0.0m)
                .RuleFor(employer => employer.Identifier, faker => faker.Random.String(10))
                ;

            return fakeEmployer;
        }
    }
}