using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using Polly;
using Polly.Registry;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.Services;
using SFA.DAS.Payments.AcceptanceTests.Services.BespokeHttpClient;
using SFA.DAS.Payments.AcceptanceTests.Services.Intefaces;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Infrastructure
{
    public class AcceptanceTestsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.Register(context =>
                             {
                                 var configHelper = context.Resolve<Configuration>();
                                 return new TestPaymentsDataContext(configHelper.PaymentsConnectionString);
                             }).As<TestPaymentsDataContext>().InstancePerLifetimeScope();

            builder.Register(context =>
                             {
                                 var registry = new PolicyRegistry();
                                 registry.Add( "HttpRetryPolicy",
                                              Policy.Handle<HttpRequestException>()
                                                    .WaitAndRetryAsync(
                                                           3, // number of retries
                                                           retryAttempt =>
                                                               TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // exponential backoff
                                                           (exception, timeSpan, retryCount, executionContext) =>
                                                           {
                                                               // add logging
                                                           }));
                                 return registry;
                             }).As<IReadOnlyPolicyRegistry<string>>()
                   .SingleInstance();
            builder.RegisterType<JobService>().As<IJobService>().InstancePerLifetimeScope();
            builder.RegisterType<BespokeHttpClient>().As<IBespokeHttpClient>().InstancePerLifetimeScope();
            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>()
                   .InstancePerLifetimeScope();
        }
    }
}
