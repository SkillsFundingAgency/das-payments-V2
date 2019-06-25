using System;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Services
{
    public class RandomUkprnService : IUkprnService
    {
        private readonly TestPaymentsDataContext dataContext;
        private const int MaximumUkprn = 1_000_000;
        private readonly Random random = new Random(Guid.NewGuid().GetHashCode());

        public RandomUkprnService(TestPaymentsDataContext dataContext)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public int GenerateUkprn()
        {
            var ukprn = random.Next(MaximumUkprn);
            dataContext.ClearPaymentsData(ukprn);
            return ukprn;
        }
    }
}
