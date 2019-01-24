using System;
using Autofac;

namespace SFA.DAS.Payments.Application.Infrastructure.Ioc
{
    public interface IContainerScopeFactory
    {
        ILifetimeScope CreateScope();
    }

    public class ContainerScopeFactory: IContainerScopeFactory
    {
        public ILifetimeScope CreateScope()
        {
            return ContainerFactory.Container.BeginLifetimeScope();
        }
    }
}