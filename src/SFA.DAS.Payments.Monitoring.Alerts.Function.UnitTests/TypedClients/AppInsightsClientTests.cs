using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Alerts.Function.JsonHelpers;
using SFA.DAS.Payments.Monitoring.Alerts.Function.TypedClients;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.UnitTests.TypedClients
{
    public class AppInsightsClientTests
    {
        private IDynamicJsonDeserializer _defaultSimpleDeserializer;

        [SetUp]
        public void Setup() 
        {
            var deserializerMock = new Mock<IDynamicJsonDeserializer>();

            deserializerMock.Setup(x => x.Deserialize(It.IsAny<string>()))
                            .Returns(new { property = "Value" });

            _defaultSimpleDeserializer = deserializerMock.Object;
        }

        [Test]
        public async Task GetSearchresultsNullUrlThrowsArgumentNullException()
        {
            //Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"{ ""property"": ""value"" }")
                })
                .Verifiable();

            var httpClientObject = new HttpClient(handlerMock.Object);

            var appInsightsClient = new AppInsightsClient(httpClientObject, _defaultSimpleDeserializer);

            //Act
            var act = () => appInsightsClient.GetSearchResultsAsync(null);

            //Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task GetSearchResultsHttpStatusNotFoundThrowsHttpRequestException()
        {
            //Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent(@"{ ""property"": ""value"" }")
                })
                .Verifiable();

            var httpClientObject = new HttpClient(handlerMock.Object);

            var appInsightsClient = new AppInsightsClient(httpClientObject, _defaultSimpleDeserializer);

            //Act
            var act = () => appInsightsClient.GetSearchResultsAsync("http://someurl.com/somepath");

            //Assert
            await act.Should().ThrowAsync<HttpRequestException>();
        }

        [Test]
        public async Task GetSearchResultsUrlNotNulllCallsHttpClientWithCorrectUrl()
        {
            //Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"{ ""property"": ""value"" }")
                })
                .Verifiable();

            var httpClientObject = new HttpClient(handlerMock.Object);

            var appInsightsClient = new AppInsightsClient(httpClientObject, _defaultSimpleDeserializer);

            //Act
            var result = await appInsightsClient.GetSearchResultsAsync("http://someurl.com/somepath");

            //Assert
            var expectedUri = new Uri("http://someurl.com/somepath");
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task GetSearchResultsWithOkResponseReturnsValue()
        {
            //Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"{ ""property"": ""value"" }")
                })
                .Verifiable();

            var deserializerMock = new Mock<IDynamicJsonDeserializer>();

            deserializerMock.Setup(x => x.Deserialize(It.IsAny<string>()))
                            .Returns(new { property = "Value" });

            var httpClientObject = new HttpClient(handlerMock.Object);
            var deserializerObject = deserializerMock.Object;
            var appInsightsClient = new AppInsightsClient(httpClientObject, deserializerObject);

            //Act
            var result = (object)await appInsightsClient.GetSearchResultsAsync("http://someurl.com/somepath");

            //Assert
            result.Should().Be(new { property = "Value" });
        }

        [Test]
        public async Task GetSearchResultsWithOkResponseCallsDeserializerWithCorrectInput()
        {
            //Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"{ ""property"": ""value"" }")
                })
                .Verifiable();

            var deserializerMock = new Mock<IDynamicJsonDeserializer>();

            deserializerMock.Setup(x => x.Deserialize(It.IsAny<string>()))
                            .Returns(new { property = "Value" });

            var httpClientObject = new HttpClient(handlerMock.Object);
            var deserializerObject = deserializerMock.Object;
            var appInsightsClient = new AppInsightsClient(httpClientObject, deserializerObject);

            //Act
            var result = await appInsightsClient.GetSearchResultsAsync("http://someurl.com/somepath");

            //Assert
            deserializerMock.Verify(x => x.Deserialize(It.Is<string>(x => x == @"{ ""property"": ""value"" }")));
        }
    }
}