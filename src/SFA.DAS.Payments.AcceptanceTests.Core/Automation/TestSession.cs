using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public class TestSession
    {
        public LearnRefNumberGenerator LearnRefNumberGenerator { get; }
        public string SessionId { get; }
        public long Ukprn { get; private set; }
        public List<Learner> Learners { get; }
        public Learner Learner => Learners.FirstOrDefault();
        public Employer Employer => GetEmployer("test employer");
        public long JobId { get; private set; }
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
                .RuleFor(course => course.FundingLineType, faker => faker.Name.JobDescriptor()?? "FundingLine")
                .RuleFor(course => course.LearnAimRef, "ZPROG001")
                .RuleFor(course => course.LearningPlannedEndDate, DateTime.Today.AddMonths(12))
                .RuleFor(course => course.LearningStartDate, DateTime.Today)
                .RuleFor(course => course.PathwayCode, faker => faker.Random.Short(1))
                .RuleFor(course => course.ProgrammeType, faker => faker.Random.Short(1))
                .RuleFor(course => course.StandardCode, faker => faker.Random.Int(1))
                .RuleFor(course => course.AgreedPrice, 15000);

            SessionId = Guid.NewGuid().ToString();
            random = new Random(Guid.NewGuid().GetHashCode());
            Ukprn = ukprn ?? GenerateId();
            Learners = new List<Learner> { GenerateLearner() };
            JobId = GenerateId();
            LearnRefNumberGenerator = new LearnRefNumberGenerator(SessionId);
            IlrSubmissionTime = DateTime.UtcNow;
            Employers = new List<Employer>();
        }

        public void SetJobId(long newJobId)
        {
            JobId = newJobId;
        }
        
        public long GenerateId(int maxValue = 1000000)
        {
            var id = random.Next(maxValue);
            //TODO: make sure that the id isn't already in use.
            return id;
        }

        public void RegenerateUkprn()
        {
            Ukprn = GenerateId();
        }

        public string GenerateLearnerReference(string learnerId)
        {
            return string.IsNullOrEmpty(learnerId) ? Learner.LearnRefNumber : LearnRefNumberGenerator.Generate(Ukprn, learnerId);
        }

        public Learner GenerateLearner(long? uniqueLearnerNumber = null)
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
                Ukprn = Ukprn,
                Uln = uln,
                LearnRefNumber = uln.ToString(),
                Course = courseFaker.Generate(1).FirstOrDefault()
            };
        }

        public Learner GetLearner(string learnerIdentifier)
        {
            var learner = Learners.FirstOrDefault(l => l.LearnerIdentifier == learnerIdentifier);
            if (learner == null)
            {
                learner = GenerateLearner();
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