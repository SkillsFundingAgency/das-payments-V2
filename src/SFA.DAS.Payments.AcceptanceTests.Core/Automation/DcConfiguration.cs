﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public class DcConfiguration
    {
        public static string DcServiceBusConnectionString =>
            ConfigurationManager.ConnectionStrings["DCServiceBusConnectionString"]?.ConnectionString;

        public static string AzureRedisConnectionString =>
            ConfigurationManager.ConnectionStrings["AzureRedisConnectionString"]?.ConnectionString;

        public static string TopicName => ConfigurationManager.AppSettings["TopicName"];

        public static string SubscriptionName => ConfigurationManager.AppSettings["SubscriptionName"];

        public static string ServiceBusQueue =>
            ConfigurationManager.AppSettings["ServiceBusQueue"];
    }
}
