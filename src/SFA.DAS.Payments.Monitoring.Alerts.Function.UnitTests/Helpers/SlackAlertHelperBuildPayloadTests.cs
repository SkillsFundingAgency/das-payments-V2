using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers;
using System;
using System.Collections.Generic;

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
            result.Should().BeOfType<List<object>>();
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
            var typePropertyValue = result[0].GetType().GetProperty("type").GetValue(result[0], null);
            
            typePropertyValue
                .Should().BeOfType(typeof(string))
                .And.Be("header");
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
            var textPropertyObject = result[0].GetType().GetProperty("text").GetValue(result[0], null);
            var textObjectTypePropertyValue = textPropertyObject.GetType().GetProperty("type").GetValue(textPropertyObject, null);

            textObjectTypePropertyValue
                .Should().BeOfType(typeof(string))
                .And.Be("plain_text");
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
            var textPropertyObject = result[0].GetType().GetProperty("text").GetValue(result[0], null);
            var textObjectTypePropertyValue = textPropertyObject.GetType().GetProperty("text").GetValue(textPropertyObject, null);

            textObjectTypePropertyValue
                .Should().BeOfType(typeof(string))
                .And.Be("alert_emoji alertTitle.");
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
            var typePropertyValue = result[1].GetType().GetProperty("type").GetValue(result[1], null);

            typePropertyValue
                .Should().BeOfType(typeof(string))
                .And.Be("section");
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
            var textObject = result[1].GetType().GetProperty("text").GetValue(result[1], null);
            var textProperty = textObject.GetType().GetProperty("text").GetValue(textObject, null);

            textProperty
                .Should().BeOfType(typeof(string))
                .And.Be($"<{_appInsightsSearchResultsUiLink}|View in Azure App Insights>");
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
            var textObject = result[1].GetType().GetProperty("text").GetValue(result[1], null);
            var typeProperty = textObject.GetType().GetProperty("type").GetValue(textObject, null);

            typeProperty
                .Should().BeOfType(typeof(string))
                .And.Be("mrkdwn");
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
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject[0].Should().BeEquivalentTo(new
            {
                type = "mrkdwn",
                text = "*Timestamp*"
            });
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
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject[1].Should().BeEquivalentTo(new
            {
                type = "mrkdwn",
                text = "*Job*"
            });
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
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject[2].Should().BeEquivalentTo(new
            {
                type = "plain_text",
                text = _timeStamp.ToString("f")
            });
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
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject[3].Should().BeEquivalentTo(new
            {
                type = "plain_text",
                text = "jobid"
            });
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
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject[4].Should().BeEquivalentTo(new
            {
                type = "mrkdwn",
                text = "*Academic Year*"
            });
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
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject[5].Should().BeEquivalentTo(new
            {
                type = "mrkdwn",
                text = "*Collection Period*"
            });
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
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject[6].Should().BeEquivalentTo(new
            {
                type = "mrkdwn",
                text = "*Previous Payments Year To Date*"
            });
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
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject[7].Should().BeEquivalentTo(new
            {
                type = "mrkdwn",
                text = "*Collection Period Payments*"
            });
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
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject[8].Should().BeEquivalentTo(new
            {
                type = "mrkdwn",
                text = "*In Learning*"
            });
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
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject[9].Should().BeEquivalentTo(new
            {
                type = "plain_text",
                text = _academicYear
            });
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
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject[10].Should().BeEquivalentTo(new
            {
                type = "plain_text",
                text = _collectionPeriod
            });
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
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject[11].Should().BeEquivalentTo(new
            {
                type = "plain_text",
                text = _yearToDatePayments
            });
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
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject[12].Should().BeEquivalentTo(new
            {
                type = "plain_text",
                text = _collectionPeriodPayments
            });
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
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject[13].Should().BeEquivalentTo(new
            {
                type = "plain_text",
                text = _numberOfLearners
            });
        }
    }
}