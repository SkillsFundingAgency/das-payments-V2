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
        Task ProcessStopDateChange(ApprenticeshipStopDateChangedEvent stopDateChangedEvent);
        Task ProcessPausedApprenticeship(ApprenticeshipPausedEvent pausedEvent);
        Task ProcessResumedApprenticeship(ApprenticeshipResumedEvent resumedEvent);
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
        private readonly IApprenticeshipPauseService apprenticeshipPauseService;
        private readonly IApprenticeshipResumedService apprenticeshipResumedService;


        public ApprenticeshipProcessor(IPaymentLogger logger, IMapper mapper,
            IApprenticeshipService apprenticeshipService,
            IEndpointInstanceFactory endpointInstanceFactory,
            IApprenticeshipApprovedUpdatedService apprenticeshipApprovedUpdatedService,
            IApprenticeshipDataLockTriageService apprenticeshipDataLockTriageService,
            IApprenticeshipStoppedService apprenticeshipStoppedService,
            IApprenticeshipPauseService apprenticeshipPauseService,
            IApprenticeshipResumedService apprenticeshipResumedService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.apprenticeshipService = apprenticeshipService ?? throw new ArgumentNullException(nameof(apprenticeshipService));
            this.endpointInstanceFactory = endpointInstanceFactory ?? throw new ArgumentNullException(nameof(endpointInstanceFactory));
            this.apprenticeshipApprovedUpdatedService = apprenticeshipApprovedUpdatedService ?? throw new ArgumentNullException(nameof(apprenticeshipApprovedUpdatedService));
            this.apprenticeshipDataLockTriageService = apprenticeshipDataLockTriageService ?? throw new ArgumentNullException(nameof(apprenticeshipDataLockTriageService));
            this.apprenticeshipStoppedService = apprenticeshipStoppedService ?? throw new ArgumentNullException(nameof(apprenticeshipStoppedService));
            this.apprenticeshipPauseService = apprenticeshipPauseService ?? throw new ArgumentNullException(nameof(apprenticeshipPauseService));
            this.apprenticeshipResumedService = apprenticeshipResumedService ?? throw new ArgumentNullException(nameof(apprenticeshipResumedService));
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
                await PublishApprenticeshipUpdate(updatedApprenticeship);

                logger.LogInfo($"Finished processing the apprenticeship updated event. Apprenticeship id: {updatedApprenticeship.Id}, employer account id: {updatedApprenticeship.AccountId}, Ukprn: {updatedApprenticeship.Ukprn}.");
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

                await PublishApprenticeshipUpdate(updatedApprenticeship);

                logger.LogInfo($"Finished processing the Apprenticeship dataLock Triage update event. Apprenticeship id: {updatedApprenticeship.Id}, employer account id: {updatedApprenticeship.AccountId}, Ukprn: {updatedApprenticeship.Ukprn}.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error processing the apprenticeship DataLock Triage update event. Error: {ex.Message}", ex);
                throw;
            }
        }

        public async Task ProcessStoppedApprenticeship(ApprenticeshipStoppedEvent apprenticeshipStoppedEvent)
        {
            logger.LogDebug($"Now processing the stopped apprenticeship with  id: {apprenticeshipStoppedEvent.ApprenticeshipId}");
            var model = new UpdatedApprenticeshipStoppedModel
            {
                ApprenticeshipId = apprenticeshipStoppedEvent.ApprenticeshipId,
                StopDate = apprenticeshipStoppedEvent.StopDate
            };

            await HandleStoppedApprenticeship(model);
        }
        
        public async Task ProcessStopDateChange(ApprenticeshipStopDateChangedEvent stopDateChangedEvent)
        {
            logger.LogDebug($"Now processing the stop date change event for  apprenticeship with  id: {stopDateChangedEvent.ApprenticeshipId}");

            var model = new UpdatedApprenticeshipStoppedModel
            {
                ApprenticeshipId = stopDateChangedEvent.ApprenticeshipId,
                StopDate = stopDateChangedEvent.StopDate
            };

            await HandleStoppedApprenticeship(model);
        }

        public async Task ProcessPausedApprenticeship(ApprenticeshipPausedEvent pausedEvent)
        {
            try
            {
                logger.LogDebug($"Now processing the apprenticeship paused event for Apprenticeship id: {pausedEvent.ApprenticeshipId}");
                var model = new UpdatedApprenticeshipPausedModel
                {
                    ApprenticeshipId = pausedEvent.ApprenticeshipId,
                    PauseDate = pausedEvent.PausedOn,
                };

                var updatedApprenticeship = await apprenticeshipPauseService.UpdateApprenticeship(model).ConfigureAwait(false);

                await PublishApprenticeshipUpdate(updatedApprenticeship);

                logger.LogInfo($"Finished processing the apprenticeship paused event. Apprenticeship id: {updatedApprenticeship.Id}, employer account id: {updatedApprenticeship.AccountId}, Ukprn: {updatedApprenticeship.Ukprn}.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error processing the apprenticeship paused event. Error: {ex.Message}", ex);
                throw;
            }
        }

        public async Task ProcessResumedApprenticeship(ApprenticeshipResumedEvent resumedEvent)
        {
            try
            {
                logger.LogDebug($"Now processing the apprenticeship resumed event for Apprenticeship id: {resumedEvent.ApprenticeshipId}");
                var model = new UpdatedApprenticeshipResumedModel
                {
                    ApprenticeshipId = resumedEvent.ApprenticeshipId,
                    ResumedDate = resumedEvent.ResumedOn,
                };

                var updatedApprenticeship = await apprenticeshipResumedService.UpdateApprenticeship(model).ConfigureAwait(false);

                 await PublishApprenticeshipUpdate(updatedApprenticeship);

                logger.LogInfo($"Finished processing the apprenticeship resumed event. Apprenticeship id: {updatedApprenticeship.Id}, employer account id: {updatedApprenticeship.AccountId}, Ukprn: {updatedApprenticeship.Ukprn}.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error processing the apprenticeship resumed event. Error: {ex.Message}", ex);
                throw;
            }
        }

        private async Task PublishApprenticeshipUpdate(ApprenticeshipModel updatedApprenticeship)
        {
            var updatedEvent = mapper.Map<ApprenticeshipUpdated>(updatedApprenticeship);
            var endpointInstance = await endpointInstanceFactory.GetEndpointInstance().ConfigureAwait(false);
            await endpointInstance.Publish(updatedEvent).ConfigureAwait(false);
        }

        private async Task HandleStoppedApprenticeship(UpdatedApprenticeshipStoppedModel model)
        {
            try
            {
                logger.LogDebug($"Now processing the stopped apprenticeship with  id: {model.ApprenticeshipId}");

                var updatedApprenticeship = await apprenticeshipStoppedService.UpdateApprenticeship(model).ConfigureAwait(false);
                await PublishApprenticeshipUpdate(updatedApprenticeship);

                logger.LogInfo($"Finished processing the stopped apprenticeship event. Apprenticeship id: {updatedApprenticeship.Id}, employer account id: {updatedApprenticeship.AccountId}, Ukprn: {updatedApprenticeship.Ukprn}.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error processing the stopped apprenticeship event. Error: {ex.Message}", ex);
                throw;
            }
        }

    }
}