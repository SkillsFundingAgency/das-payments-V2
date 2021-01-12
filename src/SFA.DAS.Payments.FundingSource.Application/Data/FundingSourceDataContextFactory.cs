using System;
using System.Collections.Generic;
using System.Data.Common;
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

        private readonly string connectionString;

        public FundingSourceDataContextFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IFundingSourceDataContext Create(DbTransaction transaction = null)
        {
            var context = new FundingSourceDataContext(connectionString);

            if (transaction != null)
            {
                context.Database.UseTransaction(transaction);
            }

            return context;
        }
    }

}
