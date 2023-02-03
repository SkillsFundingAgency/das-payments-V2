using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers;
using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.UnitTests.Helpers
{
    public class SlackAlertHelperBuildPayloadTests
    {
        [Test]
        public void BuildSlackPayloadConstructsObjectList()
        {
            //Arrange
            var alertEmoji = "alert_emoji";
            var alertTitle = "alertTitle";
            var appInsightsSearchResultsUiLink = "linktoui";
            var timeStamp = new DateTime(2003, 11, 11, 10, 10, 10);
            var jobId = "jobid";
            var academicYear = "academicYear";
            var collectionPeriod = "collectionPeriod";

            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(alertEmoji,
                                                  timeStamp,
                                                  jobId,
                                                  academicYear,
                                                  collectionPeriod,
                                                  alertTitle,
                                                  appInsightsSearchResultsUiLink);

            //Assert
            result.Should().BeOfType<List<object>>();
        }

        [Test]
        public void BuildSlackPayloadConstructsHeaderObjectTypeProperty()
        {
            //Arrange
            var alertEmoji = "alert_emoji";
            var alertTitle = "alertTitle";
            var appInsightsSearchResultsUiLink = "linktoui";
            var timeStamp = new DateTime(2003, 11, 11, 10, 10, 10);
            var jobId = "jobid";
            var academicYear = "academicYear";
            var collectionPeriod = "collectionPeriod";

            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(alertEmoji,
                                                  timeStamp,
                                                  jobId,
                                                  academicYear,
                                                  collectionPeriod,
                                                  alertTitle,
                                                  appInsightsSearchResultsUiLink);

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
            var alertEmoji = "alert_emoji";
            var alertTitle = "alertTitle";
            var appInsightsSearchResultsUiLink = "linktoui";
            var timeStamp = new DateTime(2003, 11, 11, 10, 10, 10);
            var jobId = "jobid";
            var academicYear = "academicYear";
            var collectionPeriod = "collectionPeriod";

            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(alertEmoji,
                                                  timeStamp,
                                                  jobId,
                                                  academicYear,
                                                  collectionPeriod,
                                                  alertTitle,
                                                  appInsightsSearchResultsUiLink);

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
            var alertEmoji = "alert_emoji";
            var alertTitle = "alertTitle";
            var appInsightsSearchResultsUiLink = "linktoui";
            var timeStamp = new DateTime(2003, 11, 11, 10, 10, 10);
            var jobId = "jobid";
            var academicYear = "academicYear";
            var collectionPeriod = "collectionPeriod";

            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(alertEmoji,
                                                  timeStamp,
                                                  jobId,
                                                  academicYear,
                                                  collectionPeriod,
                                                  alertTitle,
                                                  appInsightsSearchResultsUiLink);

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
            var alertEmoji = "alert_emoji";
            var alertTitle = "alertTitle";
            var appInsightsSearchResultsUiLink = "linktoui";
            var timeStamp = new DateTime(2003, 11, 11, 10, 10, 10);
            var jobId = "jobid";
            var academicYear = "academicYear";
            var collectionPeriod = "collectionPeriod";

            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(alertEmoji,
                                                  timeStamp,
                                                  jobId,
                                                  academicYear,
                                                  collectionPeriod,
                                                  alertTitle,
                                                  appInsightsSearchResultsUiLink);

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
            var alertEmoji = "alert_emoji";
            var alertTitle = "alertTitle";
            var appInsightsSearchResultsUiLink = "linktoui";
            var timeStamp = new DateTime(2003, 11, 11, 10, 10, 10);
            var jobId = "jobid";
            var academicYear = "academicYear";
            var collectionPeriod = "collectionPeriod";

            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(alertEmoji,
                                                  timeStamp,
                                                  jobId,
                                                  academicYear,
                                                  collectionPeriod,
                                                  alertTitle,
                                                  appInsightsSearchResultsUiLink);

            //Assert
            var textObject = result[1].GetType().GetProperty("text").GetValue(result[1], null);
            var textProperty = textObject.GetType().GetProperty("text").GetValue(textObject, null);

            textProperty
                .Should().BeOfType(typeof(string))
                .And.Be("<linktoui|View in Azure App Insights>");
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectTextObjectTypeProperty()
        {
            //Arrange
            var alertEmoji = "alert_emoji";
            var alertTitle = "alertTitle";
            var appInsightsSearchResultsUiLink = "linktoui";
            var timeStamp = new DateTime(2003, 11, 11, 10, 10, 10);
            var jobId = "jobid";
            var academicYear = "academicYear";
            var collectionPeriod = "collectionPeriod";

            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(alertEmoji,
                                                  timeStamp,
                                                  jobId,
                                                  academicYear,
                                                  collectionPeriod,
                                                  alertTitle,
                                                  appInsightsSearchResultsUiLink);

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
            var alertEmoji = "alert_emoji";
            var alertTitle = "alertTitle";
            var appInsightsSearchResultsUiLink = "linktoui";
            var timeStamp = new DateTime(2003, 11, 11, 10, 10, 10);
            var jobId = "jobid";
            var academicYear = "academicYear";
            var collectionPeriod = "collectionPeriod";

            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(alertEmoji,
                                                  timeStamp,
                                                  jobId,
                                                  academicYear,
                                                  collectionPeriod,
                                                  alertTitle,
                                                  appInsightsSearchResultsUiLink);

            //Assert
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject.Should().ContainEquivalentOf(new
            {
                type = "mrkdwn",
                text = "*Timestamp*"
            });
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectJobMarkdownItem()
        {
            //Arrange
            var alertEmoji = "alert_emoji";
            var alertTitle = "alertTitle";
            var appInsightsSearchResultsUiLink = "linktoui";
            var timeStamp = new DateTime(2003, 11, 11, 10, 10, 10);
            var jobId = "jobid";
            var academicYear = "academicYear";
            var collectionPeriod = "collectionPeriod";

            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(alertEmoji,
                                                  timeStamp,
                                                  jobId,
                                                  academicYear,
                                                  collectionPeriod,
                                                  alertTitle,
                                                  appInsightsSearchResultsUiLink);

            //Assert
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject.Should().ContainEquivalentOf(new
            {
                type = "mrkdwn",
                text = "*Job*"
            });
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectTimestampPlainTextItem()
        {
            //Arrange
            var alertEmoji = "alert_emoji";
            var alertTitle = "alertTitle";
            var appInsightsSearchResultsUiLink = "linktoui";
            var timeStamp = new DateTime(2003, 11, 11, 10, 10, 10);
            var jobId = "jobid";
            var academicYear = "academicYear";
            var collectionPeriod = "collectionPeriod";

            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(alertEmoji,
                                                  timeStamp,
                                                  jobId,
                                                  academicYear,
                                                  collectionPeriod,
                                                  alertTitle,
                                                  appInsightsSearchResultsUiLink);

            //Assert
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject.Should().ContainEquivalentOf(new
            {
                type = "plain_text",
                text = "Tuesday, November 11, 2003 10:10 AM"
            });
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectJobIdPlainTextItem()
        {
            //Arrange
            var alertEmoji = "alert_emoji";
            var alertTitle = "alertTitle";
            var appInsightsSearchResultsUiLink = "linktoui";
            var timeStamp = new DateTime(2003, 11, 11, 10, 10, 10);
            var jobId = "jobid";
            var academicYear = "academicYear";
            var collectionPeriod = "collectionPeriod";

            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(alertEmoji,
                                                  timeStamp,
                                                  jobId,
                                                  academicYear,
                                                  collectionPeriod,
                                                  alertTitle,
                                                  appInsightsSearchResultsUiLink);

            //Assert
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject.Should().ContainEquivalentOf(new
            {
                type = "plain_text",
                text = "jobid"
            });
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectAcademicYearMarkdownItem()
        {
            //Arrange
            var alertEmoji = "alert_emoji";
            var alertTitle = "alertTitle";
            var appInsightsSearchResultsUiLink = "linktoui";
            var timeStamp = new DateTime(2003, 11, 11, 10, 10, 10);
            var jobId = "jobid";
            var academicYear = "academicYear";
            var collectionPeriod = "collectionPeriod";

            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(alertEmoji,
                                                  timeStamp,
                                                  jobId,
                                                  academicYear,
                                                  collectionPeriod,
                                                  alertTitle,
                                                  appInsightsSearchResultsUiLink);

            //Assert
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject.Should().ContainEquivalentOf(new
            {
                type = "mrkdwn",
                text = "*Academic Year*"
            });
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectCollectionPeriodMarkdownItem()
        {
            //Arrange
            var alertEmoji = "alert_emoji";
            var alertTitle = "alertTitle";
            var appInsightsSearchResultsUiLink = "linktoui";
            var timeStamp = new DateTime(2003, 11, 11, 10, 10, 10);
            var jobId = "jobid";
            var academicYear = "academicYear";
            var collectionPeriod = "collectionPeriod";

            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(alertEmoji,
                                                  timeStamp,
                                                  jobId,
                                                  academicYear,
                                                  collectionPeriod,
                                                  alertTitle,
                                                  appInsightsSearchResultsUiLink);

            //Assert
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject.Should().ContainEquivalentOf(new
            {
                type = "mrkdwn",
                text = "*Collection Period*"
            });
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectAcademicYearPlainTextItem()
        {
            //Arrange
            var alertEmoji = "alert_emoji";
            var alertTitle = "alertTitle";
            var appInsightsSearchResultsUiLink = "linktoui";
            var timeStamp = new DateTime(2003, 11, 11, 10, 10, 10);
            var jobId = "jobid";
            var academicYear = "academicYear";
            var collectionPeriod = "collectionPeriod";

            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(alertEmoji,
                                                  timeStamp,
                                                  jobId,
                                                  academicYear,
                                                  collectionPeriod,
                                                  alertTitle,
                                                  appInsightsSearchResultsUiLink);

            //Assert
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject.Should().ContainEquivalentOf(new
            {
                type = "plain_text",
                text = "academicYear"
            });
        }

        [Test]
        public void BuildSlackPayloadConstructsSectionObjectCollectionPeriodPlainTextItem()
        {
            //Arrange
            var alertEmoji = "alert_emoji";
            var alertTitle = "alertTitle";
            var appInsightsSearchResultsUiLink = "linktoui";
            var timeStamp = new DateTime(2003, 11, 11, 10, 10, 10);
            var jobId = "jobid";
            var academicYear = "academicYear";
            var collectionPeriod = "collectionPeriod";

            var helper = new SlackAlertHelper();

            //Act
            var result = helper.BuildSlackPayload(alertEmoji,
                                                  timeStamp,
                                                  jobId,
                                                  academicYear,
                                                  collectionPeriod,
                                                  alertTitle,
                                                  appInsightsSearchResultsUiLink);

            //Assert
            var fieldsObject = result[1].GetType().GetProperty("fields").GetValue(result[1], null);
            var castedFieldsObject = (List<object>)fieldsObject;

            castedFieldsObject.Should().ContainEquivalentOf(new
            {
                type = "plain_text",
                text = "collectionPeriod"
            });
        }
    }
}