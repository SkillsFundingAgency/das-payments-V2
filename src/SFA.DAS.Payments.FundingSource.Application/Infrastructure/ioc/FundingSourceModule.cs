using Autofac;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Services;
using System.Collections.Generic;
using AutoMapper;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.ServiceFabric.Core;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache;

namespace SFA.DAS.Payments.FundingSource.Application.Infrastructure.Ioc
{
    public class FundingSourceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ValidateRequiredPaymentEvent>().AsImplementedInterfaces();
            builder.RegisterType<CoInvestedFundingSourcePaymentEventMapper>().AsImplementedInterfaces();
            builder.RegisterType<SfaFullyFundedPaymentProcessor>().AsImplementedInterfaces();
            builder.RegisterType<SfaFullyFundedFundingSourcePaymentEventMapper>().AsImplementedInterfaces();
            builder.RegisterType<IncentiveRequiredPaymentProcessor>().AsImplementedInterfaces();
            builder.RegisterType<LevyAccountRepository>().AsImplementedInterfaces();

            builder.Register(c => new ContractType2RequiredPaymentEventFundingSourceService
                (
                  new List<ICoInvestedPaymentProcessor>()
                  {
                    new SfaCoInvestedPaymentProcessor(c.Resolve<IValidateRequiredPaymentEvent>()),
                    new EmployerCoInvestedPaymentProcessor(c.Resolve<IValidateRequiredPaymentEvent>())
                  },
                  c.Resolve<ICoInvestedFundingSourcePaymentEventMapper>()
                )).As<IContractType2RequiredPaymentEventFundingSourceService>();

            builder.Register(c =>
            {
                var stateManagerProvider = c.Resolve<IActorStateManagerProvider>();
                return new ContractType1RequiredPaymentEventFundingSourceService
                (
                    new List<IPaymentProcessor>
                    {
                        new LevyPaymentProcessor(),
                        new SfaCoInvestedPaymentProcessor(c.Resolve<IValidateRequiredPaymentEvent>()),
                        new EmployerCoInvestedPaymentProcessor(c.Resolve<IValidateRequiredPaymentEvent>())
                    },
                    c.Resolve<IMapper>(),
                    new ReliableCollectionCache<ApprenticeshipContractType1RequiredPaymentEvent>(stateManagerProvider.Current),
                    new ReliableCollectionCache<List<string>>(stateManagerProvider.Current),
                    c.Resolve<ILevyAccountRepository>()
                );
            }).As<IContractType1RequiredPaymentEventFundingSourceService>();
        }
    }
}