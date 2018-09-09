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
            builder.RegisterType<IValidateRequiredPaymentEvent>().AsImplementedInterfaces().SingleInstance();

            builder.Register(c => new ContractType2RequiredPaymentHandler
                (
                  new List<ICoInvestedPaymentProcessor>()
                  {
                    new SfaCoInvestedPaymentProcessor(c.Resolve<IValidateRequiredPaymentEvent>(), c.Resolve<IMa),
                    new EmployerCoInvestedPaymentProcessor(c.Resolve<IValidateRequiredPaymentEvent>())
                  }
                )).As<IContractType2RequiredPaymentHandler>();
        }
    }
}