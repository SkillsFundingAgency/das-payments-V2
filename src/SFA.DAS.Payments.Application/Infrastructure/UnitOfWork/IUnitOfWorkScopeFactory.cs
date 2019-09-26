namespace SFA.DAS.Payments.Application.Infrastructure.UnitOfWork
{
    public interface IUnitOfWorkScopeFactory
    {
        IUnitOfWorkScope Create(string operationName);
    }
}