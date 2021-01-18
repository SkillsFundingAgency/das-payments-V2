using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Payments.FundingSource.Application.Data
{
    public interface IFundingSourceDataContextFactory
    {
        IFundingSourceDataContext Create(DbTransaction transaction = null);
    }
    
    public class FundingSourceDataContextFactory : IFundingSourceDataContextFactory
    {

        private readonly DbContextOptions<FundingSourceDataContext> options;

        public FundingSourceDataContextFactory(string connectionString)
        {
            this.options = new DbContextOptionsBuilder<FundingSourceDataContext>()
                .UseSqlServer(new SqlConnection(connectionString))
                .Options;
        }

        public IFundingSourceDataContext Create(DbTransaction transaction = null)
        {
            var context = new FundingSourceDataContext(options);

            if (transaction != null)
            {
                context.Database.UseTransaction(transaction);
            }

            return context;
        }
    }

}
