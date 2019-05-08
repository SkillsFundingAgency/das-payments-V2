using System.Collections.Generic;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public interface IBulkCopyConfiguration<T> where T : class
    {
        string TableName { get; }
        IDictionary<string, string> GetColumns { get; }
    }
}