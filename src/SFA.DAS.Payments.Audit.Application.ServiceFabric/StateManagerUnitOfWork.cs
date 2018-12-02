using System;
using System.Threading.Tasks;
using NServiceBus.UnitOfWork;

namespace SFA.DAS.Payments.Audit.Application.ServiceFabric
{
    public class StateManagerUnitOfWork: IManageUnitsOfWork
    {
        
        public Task Begin()
        {
            return Task.CompletedTask;
        }

        public Task End(Exception ex = null)
        {
            return Task.CompletedTask;
        }
    }
}
