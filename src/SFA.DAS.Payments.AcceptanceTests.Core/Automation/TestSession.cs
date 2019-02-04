using Bogus;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
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
        public Learner Learner => Learners.FirstOrDefault();
        public List<Provider> Providers { get; }
        public Provider Provider { get; private set; }
        public long Ukprn { get; private set; }
        public long JobId { get; private set; }
        public DateTime IlrSubmissionTime { get; private set; }
        public bool AtLeastOneScenarioCompleted { get; private set; }
        //private static ConcurrentDictionary<string, ConcurrentBag<TestSession>> Sessions { get;  } = new ConcurrentDictionary<string, ConcurrentBag<TestSession>>();  //TODO: will need to be refactored at some point
        private readonly Random random;
        private readonly Faker<Course> courseFaker;
        private static readonly ConcurrentBag<long> allLearners = new ConcurrentBag<long>();
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
           
            Provider = GenerateProvider();
            Ukprn = Provider.Ukprn;
            JobId = Provider.JobId;
            IlrSubmissionTime = Provider.IlrSubmissionTime;
            Providers = new List<Provider> { Provider };

            Learners = new List<Learner> { GenerateLearner(Provider.Ukprn) };
            LearnRefNumberGenerator = new LearnRefNumberGenerator(SessionId);

        }

        public void SetJobId(long newJobId)
        {
            Providers.First().JobId = newJobId;
        }

        public long GenerateId(int maxValue = 1000000)
        {
            var id = random.Next(maxValue);
            //TODO: make sure that the id isn't already in use.
            return id;
        }

        public void RegenerateUkprn()
        {
            Providers.First().Ukprn = GenerateId();
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

        public Learner GetLearner(string learnerIdentifier)
        {
            return Learners.FirstOrDefault(l => l.LearnerIdentifier == learnerIdentifier) ??
                   throw new ArgumentException($"Learner with identifier: '{learnerIdentifier}' not found.");
        }

        public void SessionEnd()
        {
            //TODO: clean up Ids
        }

        public void CompleteScenario()
        {
            AtLeastOneScenarioCompleted = true;
        }
    }
}