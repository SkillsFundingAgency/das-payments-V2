using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.RequiredPayment;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Monitoring.Jobs.Client;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Audit.RequiredPaymentService.Handlers
{
    public class RequiredPaymentEventModelHandler : IHandleMessageBatches<RequiredPaymentEventModel>
    {
        private readonly IRequiredPaymentEventStorageService storageService;
        private readonly IJobMessageClientFactory factory;

        public RequiredPaymentEventModelHandler(IRequiredPaymentEventStorageService storageService, IJobMessageClientFactory factory)
        {
            this.storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task Handle(IList<RequiredPaymentEventModel> models, CancellationToken cancellationToken)
        {
            await storageService.StoreRequiredPayments(models.ToList(), cancellationToken).ConfigureAwait(false);
            var jobStatusClient = factory.Create();
            foreach (var model in models)
            {
                await jobStatusClient.ProcessedJobMessage(model.JobId, model.EventId, model.GetType().Name, new List<GeneratedMessage>());
            }
        }
    }
}