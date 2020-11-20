using System;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Queueing;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
  public interface ITopicPublishingServiceFactory
  {
    ITopicPublishService<JobContextDto> GetPeriodEndTaskPublisher();
    ITopicPublishService<JobContextDto> GetSubmissionPublisher();
  }

  public class TopicPublishingServiceFactory : ITopicPublishingServiceFactory
  {
    private readonly IJsonSerializationService serializationService;

    public TopicPublishingServiceFactory(IJsonSerializationService serializationService)
    {
      this.serializationService = serializationService ?? throw new ArgumentNullException(nameof(serializationService));
    }

    public ITopicPublishService<JobContextDto> GetPeriodEndTaskPublisher()
    {
      return Get(DcConfiguration.PeriodEndTopicName, DcConfiguration.PeriodEndSubscriptionName);
    }

    public ITopicPublishService<JobContextDto> GetSubmissionPublisher()
    {
      return Get(DcConfiguration.TopicName, DcConfiguration.SubscriptionName);
    }

    private ITopicPublishService<JobContextDto> Get(string topicName, string subscriptionName)
    {
      var config = new TopicConfiguration(DcConfiguration.DcServiceBusConnectionString,
        topicName, subscriptionName, 10, maximumCallbackTimeSpan: TimeSpan.FromMinutes(40));
      return new TopicPublishService<JobContextDto>(config, serializationService);
    }
  }
}