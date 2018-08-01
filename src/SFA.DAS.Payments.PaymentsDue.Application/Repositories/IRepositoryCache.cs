using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.PaymentsDue.Application.Repositories
{
    public interface IRepositoryCache<T>
    {
        bool IsInitialised { get; set;  }
        Task Reset();
        Task Add(string key, T entity);
        Task<T> Get(string key);
        Task Update(string key, T entity);

    }
}
