//using Moq;
//using NUnit.Framework;
//using SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers;
//using SFA.DAS.Payments.Monitoring.Alerts.Function.JsonHelpers;
//using SFA.DAS.Payments.Monitoring.Alerts.Function.Services;
//using SFA.DAS.Payments.Monitoring.Alerts.Function.TypedClients;

//namespace SFA.DAS.Payments.Monitoring.Alerts.Function.UnitTests.Services
//{
//    public class SlackServiceTests
//    {
//        private Mock<IAppInsightsClient> _defaultAppInsightsClientMock;
//        private Mock<ISlackAlertHelper> _defaultSlackAlertHelperMock;
//        private Mock<ISlackClient> _defaultSlackClientMock;
//        private Mock<IDynamicJsonDeserializer> _defaultDeserializerMock;


//        [SetUp]
//        public void SetUp()
//        {
//            _defaultAppInsightsClientMock = new Mock<IAppInsightsClient>();
//            _defaultSlackAlertHelperMock= new Mock<ISlackAlertHelper>();
//            _defaultSlackClientMock = new Mock<ISlackClient>();
//            _defaultDeserializerMock = new Mock<IDynamicJsonDeserializer>();
//        }

//        [Test]
//        public void PostSlackAlertCallsDynamicDeserializerDependency()
//        {
//            //Arrange
//            var slackService = new SlackService(_defaultDeserializerMock.Object,
//                                                _defaultSlackAlertHelperMock.Object,
//                                                _defaultSlackClientMock.Object,
//                                                _defaultAppInsightsClientMock.Object);

//            //Act
            
//            var result = slackService.PostSlackAlert(jsonInput, urlInput);

//            //Assert
//            _defaultDeserializerMock.Verify(x => x.Deserialize(jsonInput), Times.Once());
//        }
//    }
//}
