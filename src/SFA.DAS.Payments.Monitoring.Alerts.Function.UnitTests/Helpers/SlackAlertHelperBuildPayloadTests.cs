using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers;
using System;
using System.Collections.Generic;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.UnitTests.Helpers
{
    [TestFixture]
    public class SlackAlertHelperBuildPayloadTests
    {
        private string _alertEmoji;
        private string _alertTitle;
        private string _appInsightsSearchResultsUiLink;
        private DateTime _timeStamp;
        private string _jobId;
        private string _academicYear;
        private string _collectionPeriod;
        private string _collectionPeriodPayments;
        private string _yearToDatePayments;
        private string _numberOfLearners;

        [SetUp]
        public void Setup()
        { 
            _alertEmoji = "alert_emoji"; 
            _alertTitle = "alertTitle";
            _appInsightsSearchResultsUiLink = "linktoui";
            _timeStamp = new DateTime(2003, 11, 11, 10, 10, 10);
            _jobId = "jobid";
            _academicYear = "2122";
            _collectionPeriod = "3";
            _collectionPeriodPayments = "1000";
            _yearToDatePayments = "22222";
            _numberOfLearners = "6789";
        }


        [Test]
        public void BuildSlackPayloadConstructsObjectList()
        {
            //Arrange
      

            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                                                  _timeStamp,
                                                  _jobId,
                                                  _academicYear,
                                                  _collectionPeriod,
                                                  _collectionPeriodPayments,
                                                  _yearToDatePayments,
                                                  _numberOfLearners,
                                                  _alertTitle,
                                                  _appInsightsSearchResultsUiLink);

            //Assert
            result.Should().BeOfType<List<Block>>();
            result.Count.Should().Be(3);
        }

        [Test]
        public void BuildSlackPayloadConstructsHeaderObjectTypeProperty()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[0].Type.Should().Be("header");
        }

        [Test]
        public void BuildSlackPayloadConstructsHeaderObjectPropertyType()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[0].Text.Type.Should().Be("plain_text");
        }

        [Test]
        public void BuildSlackPayloadConstructsHeaderObjectPropertyText()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[0].Text.Text.Should().Be("alert_emoji alertTitle.");
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectTypeProperty()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[1].Type.Should().Be("section");
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectTextObjectTextProperty()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[1].Text.Text.Should().Be($"<{_appInsightsSearchResultsUiLink}|View in Azure App Insights>");
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectTextObjectTypeProperty()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[1].Text.Type.Should().Be("mrkdwn");
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectTimeStampMarkdownItem()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[1].Fields.Count.Should().Be(8);
            result[1].Fields[0].Type.Should().Be("mrkdwn");
            result[1].Fields[0].Text.Should().Be("*Timestamp*");
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectJobMarkdownItem()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[1].Fields[1].Type.Should().Be("mrkdwn");
            result[1].Fields[1].Text.Should().Be("*Job*");
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectTimestampPlainTextItem()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[1].Fields[2].Type.Should().Be("plain_text");
            result[1].Fields[2].Text.Should().Be(_timeStamp.ToString("f"));
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectJobIdPlainTextItem()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[1].Fields[3].Type.Should().Be("plain_text");
            result[1].Fields[3].Text.Should().Be("jobid");
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectAcademicYearMarkdownItem()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[1].Fields[4].Type.Should().Be("mrkdwn");
            result[1].Fields[4].Text.Should().Be("*Academic Year*");
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectCollectionPeriodMarkdownItem()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[1].Fields[5].Type.Should().Be("mrkdwn");
            result[1].Fields[5].Text.Should().Be("*Collection Period*");
        }
        
        [Test]
        public void BuildSlackPayloadConstructsSectionObjectPaymentsYearToDateMarkdownItem()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[2].Fields[0].Type.Should().Be("mrkdwn");
            result[2].Fields[0].Text.Should().Be("*Previous Payments Year To Date*");
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectCollectionPeriodPaymentsMarkdownItem()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[2].Fields[1].Type.Should().Be("mrkdwn");
            result[2].Fields[1].Text.Should().Be("*Collection Period Payments*");
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectNumberOfLearnersMarkdownItem()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[2].Fields[4].Type.Should().Be("mrkdwn");
            result[2].Fields[4].Text.Should().Be("*In Learning*");
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectNumberOfLearnersSpacerMarkdownItem()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[2].Fields[5].Type.Should().Be("mrkdwn");
            result[2].Fields[5].Text.Should().Be(" ");
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectAcademicYearPlainTextItem()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[1].Fields[6].Type.Should().Be("plain_text");
            result[1].Fields[6].Text.Should().Be(_academicYear);
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectCollectionPeriodPlainTextItem()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[1].Fields[7].Type.Should().Be("plain_text");
            result[1].Fields[7].Text.Should().Be(_collectionPeriod);
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectPaymentsYearToDatePlainTextItem()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            var decimalValue = Convert.ToDecimal(_yearToDatePayments);
            result[2].Fields[2].Type.Should().Be("plain_text");
            result[2].Fields[2].Text.Should().Be($"£{decimalValue.ToString("N2")}");
        }

        [Test]
        public void PaymentsYearToDateIsRenderedVerbatimIfCannotFormatAsCurrency()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                "nil",
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[2].Fields[2].Type.Should().Be("plain_text");
            result[2].Fields[2].Text.Should().Be("£nil");
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectCollectionPeriodPaymentsPlainTextItem()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            var decimalValue = Convert.ToDecimal(_collectionPeriodPayments);
            result[2].Fields[3].Type.Should().Be("plain_text");
            result[2].Fields[3].Text.Should().Be($"£{decimalValue.ToString("N2")}");
        }

        [Test]
        public void CollectionPeriodPaymentsRenderedVerbatimIfCannotFormatAsCurrency()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                "nil",
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[2].Fields[3].Type.Should().Be("plain_text");
            result[2].Fields[3].Text.Should().Be("£nil");
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectNumberOfLearnersPlainTextItem()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result[2].Fields[6].Type.Should().Be("plain_text");
            result[2].Fields[6].Text.Should().Be(_numberOfLearners);
        }

        [Test]
        public void OptionalBlockValuesAreNotRenderedIfNotPresent()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                string.Empty,
                string.Empty,
                string.Empty,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result.Count.Should().Be(2);
        }

        [Test]
        public void CollectionPeriodPaymentsIsNotRenderedIfNotPresent()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                string.Empty,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result.Count.Should().Be(3);
            result[2].Fields.Count.Should().Be(5);
            result[2].Fields[1].Type.Should().Be("plain_text");
            var decimalValue = Convert.ToDecimal(_yearToDatePayments);
            result[2].Fields[1].Text.Should().Be($"£{decimalValue.ToString("N2")}");
        }

        [Test]
        public void PreviousPaymentsYearToDateIsNotRenderedIfNotPresent()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                string.Empty,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result.Count.Should().Be(3);
            result[2].Fields.Count.Should().Be(5);
            result[2].Fields[1].Type.Should().Be("plain_text");
            var decimalValue = Convert.ToDecimal(_collectionPeriodPayments);
            result[2].Fields[1].Text.Should().Be($"£{decimalValue.ToString("N2")}");
        }

        [Test]
        public void NumberOfLearnersIsNotRenderedIfNotPresent()
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                string.Empty,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            //Assert
            result.Count.Should().Be(3);
            result[2].Fields.Count.Should().Be(4);
        }

        [Test]
        public void SlackPayloadSerializesInFormatExpectedByApi()
        {
            // Arrange
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(_alertEmoji,
                _timeStamp,
                _jobId,
                _academicYear,
                _collectionPeriod,
                _collectionPeriodPayments,
                _yearToDatePayments,
                _numberOfLearners,
                _alertTitle,
                _appInsightsSearchResultsUiLink);

            var jsonString = JsonSerializer.Serialize(result, serializeOptions);

            // Assert
            jsonString.Should().Contain("type");
            jsonString.Should().Contain("text");
            jsonString.Should().Contain("fields");
            jsonString.Should().NotContain("Type");
            jsonString.Should().NotContain("Text");
            jsonString.Should().NotContain("Fields");
        }
    }
}