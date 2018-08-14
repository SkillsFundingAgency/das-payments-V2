using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.Payment.ServiceFabric.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Entities;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.UnitTests.Service
{
    [TestFixture, Explicit]
    public class TestDataGenerator
    {
        [Test]
        public async Task PublishPayableEarning()
        {

            IPayableEarningEvent earning = new PayableEarningEvent
            {
                Ukprn = 1,
                LearnRefNumber = "2",
                ContractType = ContractType.Act2,
                Learner = new LearnerEntity(),
                LearnAim = new LearnAimEntity
                {
                    ProgrammeType = 3,
                    FrameworkCode = 4,
                    PathwayCode = 5,
                    StandardCode = 6,
                    LearnAimRef = "7"
                },
                PriceEpisodes = new[]
                {
                    new PriceEpisodeEntity
                    {
                        StartDate = DateTime.Today.AddMonths(-1),
                        EndDate = DateTime.Today,
                        Periods = new byte[]
                        {
                            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12
                        },
                        Price = 111
                    }
                }
            };

            var sender = new EndpointCommunicationSender<IPayableEarningEvent>("sfa-das-payments-paymentsdue-integration-test", "UseDevelopmentStorage=true", "sfa-das-payments-paymentsdue-proxyservice", new ContainerBuilder().Build());
            await sender.Send(earning).ConfigureAwait(false);
        }
    }
}
