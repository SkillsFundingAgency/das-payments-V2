using System;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Application.Infrastructure.UnitOfWork
{
    public interface IUnitOfWorkScope : IDisposable
    {
        T Resolve<T>();
        void Abort();
        Task Commit();
    }
}