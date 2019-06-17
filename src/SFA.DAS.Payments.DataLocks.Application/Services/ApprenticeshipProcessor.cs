using System;
using System.Threading.Tasks;
using AutoMapper;
using NServiceBus;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public interface IApprenticeshipProcessor
    {
        Task Process(ApprenticeshipCreatedEvent createdEvent);
    }

    public class ApprenticeshipProcessor : IApprenticeshipProcessor
    {
        private readonly IPaymentLogger logger;
        private readonly IMapper mapper;
        private readonly IApprenticeshipService apprenticeshipService;
        private readonly IMessageSession messageSession;

        public ApprenticeshipProcessor(IPaymentLogger logger, IMapper mapper, IApprenticeshipService apprenticeshipService, IMessageSession messageSession )
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.apprenticeshipService = apprenticeshipService ?? throw new ArgumentNullException(nameof(apprenticeshipService));
            this.messageSession = messageSession ?? throw new ArgumentNullException(nameof(messageSession));
        }

        public async Task Process(ApprenticeshipCreatedEvent createdEvent)
        {
            try
            {
                logger.LogDebug($"Now processing the apprenticeship created event. Apprenticeship id: {createdEvent.ApprenticeshipId}, employer account id: {createdEvent.AccountId}, Ukprn: {createdEvent.ProviderId}.");
                var model = mapper.Map<ApprenticeshipModel>(createdEvent);
                await apprenticeshipService.NewApprenticeship(model).ConfigureAwait(false);
                var updatedEvent = mapper.Map<ApprenticeshipUpdated>(model);
                await messageSession.Publish(updatedEvent).ConfigureAwait(false);
                logger.LogInfo($"Finished processing the apprenticeship created event. Apprenticeship id: {createdEvent.ApprenticeshipId}, employer account id: {createdEvent.AccountId}, Ukprn: {createdEvent.ProviderId}.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error processing the apprenticeship event. Error: {ex.Message}", ex);
                throw;
            }
        }
    }
}