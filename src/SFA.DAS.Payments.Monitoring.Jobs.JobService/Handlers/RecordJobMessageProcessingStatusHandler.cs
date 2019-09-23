using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Infrastructure.UnitOfWork;
using SFA.DAS.Payments.Application.Messaging.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.UnitOfWork;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers
{
    public class RecordJobMessageProcessingStatusHandler : IHandleMessages<RecordJobMessageProcessingStatus>
    {
        private readonly IUnitOfWorkScopeFactory scopeFactory;
        private readonly IPaymentLogger logger;
        private readonly ITelemetry telemetry;

        public RecordJobMessageProcessingStatusHandler(
            IUnitOfWorkScopeFactory scopeFactory,
            IPaymentLogger logger,
            ITelemetry telemetry
            )
        {
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.telemetry = telemetry;
        }

        public Task Handle(RecordJobMessageProcessingStatus message, IMessageHandlerContext context)
        {
            return TelemetryEvent.TrackAsync(telemetry, "JobService.RecordJobMessageProcessingStatus inner", "", async () =>
            {
                IUnitOfWorkScope scope = null;

                telemetry.TrackAction("JobService.RecordJobMessageProcessingStatus scope=", "", () =>
                {
                    scope = scopeFactory.Create("JobService.RecordJobMessageProcessingStatus");
                });

                await telemetry.TrackActionAsync("JobService.RecordJobMessageProcessingStatus i2", "", async () =>
                {
                    try
                    {
                        IJobMessageService jobMessageService = null;

                        telemetry.TrackAction("JobService.RecordJobMessageProcessingStatus i3", "", () =>
                            {
                                jobMessageService = scope.Resolve<IJobMessageService>();
                            });

                        await telemetry.TrackActionAsync("JobService.RecordJobMessageProcessingStatus i4", "", async () =>
                            {
                                await jobMessageService.RecordCompletedJobMessageStatus(message, CancellationToken.None).ConfigureAwait(false);
                            });

                        await telemetry.TrackActionAsync("JobService.RecordJobMessageProcessingStatus i5", "", async () =>
                            {
                                await scope.Commit();
                            });
                    }
                    catch (Exception ex)
                    {
                        telemetry.TrackAction("JobService.RecordJobMessageProcessingStatus i3", "", () =>
                            {
                                scope.Abort();
                            });

                        logger.LogWarning($"Failed to record earnings job: {message.JobId}. Error: {ex.Message}, {ex}");
                        throw;
                    }
                    finally
                    {
                        telemetry.TrackAction("JobService.RecordJobMessageProcessingStatus dispose scope", "", () =>
                        {
                            ((UnitOfWorkScope)scope).Dispose(telemetry);
                            //using (scope)
                            //{
                            //}
                        });
                    }
                });
            });
        }
    }
}