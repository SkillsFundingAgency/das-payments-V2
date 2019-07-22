using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using NServiceBus;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public interface IApprenticeshipProcessor
    {
        Task Process(ApprenticeshipCreatedEvent createdEvent);
        Task ProcessUpdatedApprenticeship(ApprenticeshipUpdatedApprovedEvent updatedEvent);
        Task ProcessApprenticeshipDataLockTriage(DataLockTriageApprovedEvent apprenticeshipDataLockTriageEvent);
        Task ProcessStoppedApprenticeship(ApprenticeshipStoppedEvent apprenticeshipStoppedEvent);
    }

    public class ApprenticeshipProcessor : IApprenticeshipProcessor
    {
        private readonly IPaymentLogger logger;
        private readonly IMapper mapper;
        private readonly IApprenticeshipService apprenticeshipService;
        private readonly IEndpointInstanceFactory endpointInstanceFactory;
        private readonly IApprenticeshipApprovedUpdatedService apprenticeshipApprovedUpdatedService;
        private readonly IApprenticeshipDataLockTriageService apprenticeshipDataLockTriageService;
        private readonly IApprenticeshipStoppedService apprenticeshipStoppedService;


        public ApprenticeshipProcessor(IPaymentLogger logger, IMapper mapper, 
            IApprenticeshipService apprenticeshipService, 
            IEndpointInstanceFactory endpointInstanceFactory,
            IApprenticeshipApprovedUpdatedService apprenticeshipApprovedUpdatedService,
            IApprenticeshipDataLockTriageService apprenticeshipDataLockTriageService,
            IApprenticeshipStoppedService apprenticeshipStoppedService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.apprenticeshipService = apprenticeshipService ?? throw new ArgumentNullException(nameof(apprenticeshipService));
            this.endpointInstanceFactory = endpointInstanceFactory ?? throw new ArgumentNullException(nameof(endpointInstanceFactory));
            this.apprenticeshipApprovedUpdatedService = apprenticeshipApprovedUpdatedService ?? throw new ArgumentNullException(nameof(apprenticeshipApprovedUpdatedService));
            this.apprenticeshipDataLockTriageService = apprenticeshipDataLockTriageService ?? throw new ArgumentNullException(nameof(apprenticeshipDataLockTriageService));
            this.apprenticeshipStoppedService = apprenticeshipStoppedService ?? throw new ArgumentNullException(nameof(apprenticeshipStoppedService));
        }

        public async Task Process(ApprenticeshipCreatedEvent createdEvent)
        {
            try
            {
                logger.LogDebug($"Now processing the apprenticeship created event. Apprenticeship id: {createdEvent.ApprenticeshipId}, employer account id: {createdEvent.AccountId}, Ukprn: {createdEvent.ProviderId}.");
                var model = mapper.Map<ApprenticeshipModel>(createdEvent);
                var duplicates = await apprenticeshipService.NewApprenticeship(model).ConfigureAwait(false);
                var updatedEvent = mapper.Map<ApprenticeshipUpdated>(model);
                updatedEvent.Duplicates = duplicates.Select(duplicate => new ApprenticeshipDuplicate { Ukprn = duplicate.Ukprn, ApprenticeshipId = duplicate.ApprenticeshipId }).ToList();
                var endpointInstance = await endpointInstanceFactory.GetEndpointInstance().ConfigureAwait(false);
                await endpointInstance.Publish(updatedEvent).ConfigureAwait(false);
                logger.LogInfo($"Finished processing the apprenticeship created event. Apprenticeship id: {createdEvent.ApprenticeshipId}, employer account id: {createdEvent.AccountId}, Ukprn: {createdEvent.ProviderId}.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error processing the apprenticeship event. Error: {ex.Message}", ex);
                throw;
            }
        }

        public async Task ProcessUpdatedApprenticeship(ApprenticeshipUpdatedApprovedEvent apprenticeshipApprovedEvent)
        {
            try
            {
                logger.LogDebug($"Now processing the apprenticeship update even for Apprenticeship id: {apprenticeshipApprovedEvent.ApprenticeshipId}");
                var model = mapper.Map<UpdatedApprenticeshipApprovedModel>(apprenticeshipApprovedEvent);

                var updatedApprenticeship = await apprenticeshipApprovedUpdatedService.UpdateApprenticeship(model).ConfigureAwait(false);

                var updatedEvent = mapper.Map<ApprenticeshipUpdated>(updatedApprenticeship);

                var endpointInstance = await endpointInstanceFactory.GetEndpointInstance().ConfigureAwait(false);
                await endpointInstance.Publish(updatedEvent).ConfigureAwait(false);

                logger.LogInfo($"Finished processing the apprenticeship updated event. Apprenticeship id: {updatedEvent.Id}, employer account id: {updatedEvent.EmployerAccountId}, Ukprn: {updatedEvent.Ukprn}.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error processing the apprenticeship updated event. Error: {ex.Message}", ex);
                throw;
            }
        }

        public async Task ProcessApprenticeshipDataLockTriage(DataLockTriageApprovedEvent apprenticeshipDataLockTriageEvent)
        {
            try
            {
                logger.LogDebug($"Now processing the apprenticeship DataLock Triage update for Apprenticeship id: {apprenticeshipDataLockTriageEvent.ApprenticeshipId}");
                var model = mapper.Map<UpdatedApprenticeshipDataLockTriageModel>(apprenticeshipDataLockTriageEvent);

                var updatedApprenticeship = await apprenticeshipDataLockTriageService.UpdateApprenticeship(model).ConfigureAwait(false);

                var updatedEvent = mapper.Map<ApprenticeshipUpdated>(updatedApprenticeship);

                var endpointInstance = await endpointInstanceFactory.GetEndpointInstance().ConfigureAwait(false);
                await endpointInstance.Publish(updatedEvent).ConfigureAwait(false);

                logger.LogInfo($"Finished processing the Apprenticeship dataLock Triage update event. Apprenticeship id: {updatedEvent.Id}, employer account id: {updatedEvent.EmployerAccountId}, Ukprn: {updatedEvent.Ukprn}.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error processing the apprenticeship DataLock Triage update event. Error: {ex.Message}", ex);
                throw;
            }
        }

        public async Task ProcessStoppedApprenticeship(ApprenticeshipStoppedEvent apprenticeshipStoppedEvent)
        {
            try
            {
                logger.LogDebug($"Now processing the stopped apprenticeship with  id: {apprenticeshipStoppedEvent.ApprenticeshipId}");
                var model = new UpdatedApprenticeshipStoppedModel
                {
                    ApprenticeshipId = apprenticeshipStoppedEvent.ApprenticeshipId,
                    StopDate = apprenticeshipStoppedEvent.StopDate
                };
                
                var updatedApprenticeship = await apprenticeshipStoppedService.UpdateApprenticeship(model).ConfigureAwait(false);

                var updatedEvent = mapper.Map<ApprenticeshipUpdated>(updatedApprenticeship);

                var endpointInstance = await endpointInstanceFactory.GetEndpointInstance().ConfigureAwait(false);
                await endpointInstance.Publish(updatedEvent).ConfigureAwait(false);

                logger.LogInfo($"Finished processing the stopped apprenticeship event. Apprenticeship id: {updatedEvent.Id}, employer account id: {updatedEvent.EmployerAccountId}, Ukprn: {updatedEvent.Ukprn}.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error processing the stopped apprenticeship event. Error: {ex.Message}", ex);
                throw;
            }
        }



    }
}