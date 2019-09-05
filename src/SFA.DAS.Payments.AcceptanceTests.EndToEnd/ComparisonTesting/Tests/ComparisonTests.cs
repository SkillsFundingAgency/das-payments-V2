using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using NUnit.Framework;
using Polly;
using Polly.Registry;
using SFA.DAS.Payments.AcceptanceTests.Services;
using SFA.DAS.Payments.AcceptanceTests.Services.BespokeHttpClient;
using SFA.DAS.Payments.AcceptanceTests.Services.Intefaces;
using ESFA.DC.ILR.TestDataGenerator.Api.StorageService;
using SFA.DAS.Payments.AcceptanceTests.Services.Configuration;


namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.ComparisonTesting.Tests

{
    
    [Category("Comparison")]
    public class ComparisonTests
    {
        private IContainer _autofacContainer;
        protected IContainer AutofacContainer
        {
            get
            {
                if (_autofacContainer == null)
                {
                    var builder = new ContainerBuilder();

                    // Repositories
                    builder.RegisterType<JobService>().As<IJobService>().InstancePerLifetimeScope();

                    builder.RegisterType<BespokeHttpClient>().As<IBespokeHttpClient>().InstancePerLifetimeScope();
                    builder.RegisterType<AzureStorageServiceConfig>().As<IAzureStorageKeyValuePersistenceServiceConfig>().InstancePerLifetimeScope();
                    builder.RegisterType<AzureStorageKeyValuePersistenceService>().As<IStreamableKeyValuePersistenceService>().InstancePerLifetimeScope();
                    builder.RegisterType<StorageService>().As<IStorageService>().InstancePerLifetimeScope();



                    builder.Register(context =>
                        {
                            var registry = new PolicyRegistry();
                            registry.Add(
                                "HttpRetryPolicy",
                                Policy.Handle<HttpRequestException>()
                                    .WaitAndRetryAsync(
                                        3, // number of retries
                                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // exponential backoff
                                        (exception, timeSpan, retryCount, executionContext) =>
                                        {
                                            // add logging
                                        }));
                            return registry;
                        }).As<IReadOnlyPolicyRegistry<string>>()
                        .SingleInstance();

                    builder.RegisterType<JobService>().As<IJobService>().InstancePerLifetimeScope();
                    builder.RegisterType<BespokeHttpClient>().As<IBespokeHttpClient>().InstancePerLifetimeScope();

                    builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().InstancePerLifetimeScope();
                    var container = builder.Build();

                    _autofacContainer = container;
                }

                return _autofacContainer;
            }
        }

        protected IJobService DcJobService => AutofacContainer.Resolve<IJobService>();
        protected IAzureStorageKeyValuePersistenceServiceConfig StorageServiceConfig => AutofacContainer.Resolve<IAzureStorageKeyValuePersistenceServiceConfig>();
        protected IStreamableKeyValuePersistenceService StorageService => AutofacContainer.Resolve<IStreamableKeyValuePersistenceService>();

       


        [Test]
        public async Task InitialTest()
        {
            //Arrange
            IlrPublisher publisher = new IlrPublisher(DcJobService, StorageServiceConfig, StorageService);
             var ukprn = 10001144;
             string fileContent = SampleContent();
            string ilrFileName = $"ILR-{ukprn}-1819-{DateTime.Now.Date.ToString().Replace("/", "")}-{DateTime.Now.TimeOfDay.ToString().Replace(":", "")}.xml";

           
            await publisher.StoreAndPublishIlrFile(ukprn: ukprn, ilrFileName: ilrFileName, ilrFile: fileContent, collectionYear: 1819, collectionPeriod: 1);




            Assert.True(true);
        }






        private string SampleContent()
        {
            return @"<?xml version=""1.0"" encoding=""utf-8""?>
<Message xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""ESFA/ILR/2018-19"">
  <Header>
    <CollectionDetails>
      <Collection>ILR</Collection>
      <Year>1819</Year>
      <FilePreparationDate>2019-09-05</FilePreparationDate>
    </CollectionDetails>
    <Source>
      <ProtectiveMarking>OFFICIAL-SENSITIVE-Personal</ProtectiveMarking>
      <UKPRN>10001144</UKPRN>
      <SoftwareSupplier>Own Software</SoftwareSupplier>
      <Release>0.1</Release>
      <SerialNo>01</SerialNo>
      <DateTime>2019-09-05T09:48:43.9785502+00:00</DateTime>
    </Source>
  </Header>
  <LearningProvider>
    <UKPRN>10001144</UKPRN>
  </LearningProvider>
  <Learner>
    <LearnRefNumber>0fm36255</LearnRefNumber>
    <ULN>9900011502</ULN>
    <FamilyName>Sméth</FamilyName>
    <GivenNames>Mary Jane</GivenNames>
    <DateOfBirth>1997-08-01</DateOfBirth>
    <Ethnicity>31</Ethnicity>
    <Sex>M</Sex>
    <LLDDHealthProb>2</LLDDHealthProb>
    <NINumber>LJ000000A</NINumber>
    <PriorAttain>2</PriorAttain>
    <PlanLearnHours>90</PlanLearnHours>
    <PlanEEPHours>1</PlanEEPHours>
    <PostcodePrior>ZZ99 9ZZ</PostcodePrior>
    <Postcode>ZZ99 9ZZ</Postcode>
    <AddLine1>18 Address line road</AddLine1>
    <TelNo>07855555555</TelNo>
    <Email>myemail@myemail.com</Email>
    <LearnerEmploymentStatus>
      <EmpStat>10</EmpStat>
      <DateEmpStatApp>2018-02-01</DateEmpStatApp>
      <EmpId>154549452</EmpId>
      <EmploymentStatusMonitoring>
        <ESMType>EII</ESMType>
        <ESMCode>4</ESMCode>
      </EmploymentStatusMonitoring>
      <EmploymentStatusMonitoring>
        <ESMType>LOE</ESMType>
        <ESMCode>4</ESMCode>
      </EmploymentStatusMonitoring>
    </LearnerEmploymentStatus>
    <LearningDelivery>
      <LearnAimRef>ZPROG001</LearnAimRef>
      <AimType>1</AimType>
      <AimSeqNumber>1</AimSeqNumber>
      <LearnStartDate>2018-08-01</LearnStartDate>
      <LearnPlanEndDate>2019-08-01</LearnPlanEndDate>
      <FundModel>36</FundModel>
      <ProgType>20</ProgType>
      <FworkCode>593</FworkCode>
      <PwayCode>1</PwayCode>
      <DelLocPostCode>ZZ99 9ZZ</DelLocPostCode>
      <CompStatus>1</CompStatus>
      <SWSupAimId>16b8576f-9aaf-4ea9-af02-211d39246356</SWSupAimId>
      <LearningDeliveryFAM>
        <LearnDelFAMType>ACT</LearnDelFAMType>
        <LearnDelFAMCode>2</LearnDelFAMCode>
        <LearnDelFAMDateFrom>2018-08-01</LearnDelFAMDateFrom>
      </LearningDeliveryFAM>
      <LearningDeliveryFAM>
        <LearnDelFAMType>SOF</LearnDelFAMType>
        <LearnDelFAMCode>105</LearnDelFAMCode>
      </LearningDeliveryFAM>
      <LearningDeliveryFAM>
        <LearnDelFAMType>HHS</LearnDelFAMType>
        <LearnDelFAMCode>1</LearnDelFAMCode>
      </LearningDeliveryFAM>
      <AppFinRecord>
        <AFinType>TNP</AFinType>
        <AFinCode>1</AFinCode>
        <AFinDate>2018-08-01</AFinDate>
        <AFinAmount>11250</AFinAmount>
      </AppFinRecord>
      <AppFinRecord>
        <AFinType>TNP</AFinType>
        <AFinCode>1</AFinCode>
        <AFinDate>2018-10-01</AFinDate>
        <AFinAmount>1400</AFinAmount>
      </AppFinRecord>
    </LearningDelivery>
    <LearningDelivery>
      <LearnAimRef>00300545</LearnAimRef>
      <AimType>3</AimType>
      <AimSeqNumber>2</AimSeqNumber>
      <LearnStartDate>2018-08-01</LearnStartDate>
      <LearnPlanEndDate>2019-08-01</LearnPlanEndDate>
      <FundModel>36</FundModel>
      <ProgType>20</ProgType>
      <FworkCode>593</FworkCode>
      <PwayCode>1</PwayCode>
      <DelLocPostCode>ZZ99 9ZZ</DelLocPostCode>
      <CompStatus>1</CompStatus>
      <SWSupAimId>7f27063d-e14f-4b0e-b6b8-d6156af2c6ef</SWSupAimId>
      <LearningDeliveryFAM>
        <LearnDelFAMType>SOF</LearnDelFAMType>
        <LearnDelFAMCode>105</LearnDelFAMCode>
      </LearningDeliveryFAM>
      <LearningDeliveryFAM>
        <LearnDelFAMType>HHS</LearnDelFAMType>
        <LearnDelFAMCode>1</LearnDelFAMCode>
      </LearningDeliveryFAM>
      <LearningDeliveryHE>
        <NUMHUS>2000812012XTT60021</NUMHUS>
        <QUALENT3>X06</QUALENT3>
        <UCASAPPID>AB89</UCASAPPID>
        <TYPEYR>1</TYPEYR>
        <MODESTUD>99</MODESTUD>
        <FUNDLEV>10</FUNDLEV>
        <FUNDCOMP>3</FUNDCOMP>
        <STULOAD>10.0</STULOAD>
        <YEARSTU>1</YEARSTU>
        <MSTUFEE>1</MSTUFEE>
        <PCFLDCS>100</PCFLDCS>
        <SPECFEE>9</SPECFEE>
        <NETFEE>0</NETFEE>
        <GROSSFEE>1</GROSSFEE>
        <DOMICILE>ZZ</DOMICILE>
        <ELQ>9</ELQ>
      </LearningDeliveryHE>
    </LearningDelivery>
  </Learner>
</Message>";
        }
    }
}
