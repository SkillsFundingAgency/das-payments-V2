using Bogus;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public class TestSession
    {
        public LearnRefNumberGenerator LearnRefNumberGenerator { get; }
        public string SessionId { get; }
        public List<Learner> Learners { get; }
        public Learner Learner => Learners.FirstOrDefault();
        public List<Provider> Providers { get; }
        public Provider Provider => Providers.First();
        public long Ukprn => Provider.Ukprn;
        public long JobId => Provider.JobId;

        public Employer Employer => Employers.First();

        public DateTime IlrSubmissionTime { get; set; }
        public bool MonthEndCommandSent { get; set; }
        public bool AtLeastOneScenarioCompleted { get; private set; }
        
        public List<Employer> Employers { get; }
        private readonly Random random;
        private readonly Faker<Course> courseFaker;
        private static readonly ConcurrentBag<long> allLearners = new ConcurrentBag<long>();

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

        public TestSession(long? ukprn = null)
        {
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
            Providers = new List<Provider> { GenerateProvider() };
            IlrSubmissionTime = Provider.IlrSubmissionTime;
            Learners = new List<Learner> { GenerateLearner(Provider.Ukprn) };
            LearnRefNumberGenerator = new LearnRefNumberGenerator(SessionId);
            Employers = new List<Employer>(GenerateEmployer().Generate(1));
        }
        
        public long GenerateId(int maxValue = 1000000)
        {
            var id = random.Next(maxValue);
            //TODO: make sure that the id isn't already in use.
            return id;
        }

        public void RegenerateUkprn()
        {
            Provider.Ukprn = GenerateId();
        }

        public Provider GenerateProvider()
        {
            return new Provider
            {
                Ukprn = GenerateId(),
                JobId = GenerateId(),
                IlrSubmissionTime = DateTime.UtcNow
            };
        }

        public Provider GetProviderByIdentifier(string identifier)
        {
            return Providers.Single(p => p.Identifier == identifier);
        }

        public string GenerateLearnerReference(string learnerId)
        {
            return string.IsNullOrEmpty(learnerId) ? Learner.LearnRefNumber : LearnRefNumberGenerator.Generate(Ukprn, learnerId);
        }

        public Learner GenerateLearner(long ukprn, long? uniqueLearnerNumber = null)
        {
            var uln = uniqueLearnerNumber ?? GenerateId();
            var limit = 10;
            while (allLearners.Contains(uln))
            {
                if (--limit < 0)
                    throw new InvalidOperationException("Failed to generate random learners.");
                uln = GenerateId();
            }
            allLearners.Add(uln);
            return new Learner
            {
                Ukprn = ukprn,
                Uln = uln,
                LearnRefNumber = uln.ToString(),
                Course = courseFaker.Generate(1).FirstOrDefault()
            };
        }

        public Learner GetLearner(long ukprn,string learnerIdentifier)
        {
            var learner = Learners.FirstOrDefault(l => l.LearnerIdentifier == learnerIdentifier && l.Ukprn == ukprn);
            if (learner == null)
            {
                learner = GenerateLearner(ukprn);
                learner.LearnerIdentifier = learnerIdentifier;
                Learners.Add(learner);
            }

            return learner;
        }

        public void SessionEnd()
        {
            //TODO: clean up Ids
        }

        public void CompleteScenario()
        {
            AtLeastOneScenarioCompleted = true;
            MonthEndCommandSent = false;
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