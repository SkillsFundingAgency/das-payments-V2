using Autofac;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Services;
using System.Collections.Generic;

namespace SFA.DAS.Payments.FundingSource.Application.Infrastructure.ioc
{
    public class FundingSourceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new ContractType2RequiredPaymentHandler
                (
                  new List<ICoInvestedPaymentProcessor>()
                  {
                    new SfaCoInvestedPaymentProcessor(),
                    new EmployerCoInvestedPaymentProcessor()
                  }
                )).As<IContractType2RequiredPaymentHandler>();
        }
    }
}