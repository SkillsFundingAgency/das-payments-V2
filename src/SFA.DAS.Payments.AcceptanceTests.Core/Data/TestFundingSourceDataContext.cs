using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.FundingSource.Application.Data;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public class TestFundingSourceDataContext : FundingSourceDataContext
    {
        public TestFundingSourceDataContext(string connectionString) : base(connectionString)
        {
        }

        public TestFundingSourceDataContext(DbContextOptions<FundingSourceDataContext> options) : base(options)
        {
        }
    }
}
