﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:2.4.0.0
//      SpecFlow Generator Version:2.4.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Tests.Non_LevyLearner_LearnerChangeOfCircumstances
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.4.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Change standard and price but not from start - PV2-205")]
    public partial class ChangeStandardAndPriceButNotFromStart_PV2_205Feature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Change standard and price but not from start - PV2-205.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Change standard and price but not from start - PV2-205", "\tAs a provider,\r\n\tI want a non-levy learner, changes standard with change to nego" +
                    "tiated price, to be paid the correct amount\r\n\tSo that I am accurately paid my ap" +
                    "prenticeship provision.", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Non-Levy learner changes standard with accompanying change to the negotiated pric" +
            "e PV2-205")]
        [NUnit.Framework.TestCaseAttribute("R01/Current Academic Year", null)]
        [NUnit.Framework.TestCaseAttribute("R02/Current Academic Year", null)]
        [NUnit.Framework.TestCaseAttribute("R03/Current Academic Year", null)]
        [NUnit.Framework.TestCaseAttribute("R04/Current Academic Year", null)]
        [NUnit.Framework.TestCaseAttribute("R05/Current Academic Year", null)]
        [NUnit.Framework.TestCaseAttribute("R06/Current Academic Year", null)]
        [NUnit.Framework.TestCaseAttribute("R07/Current Academic Year", null)]
        [NUnit.Framework.TestCaseAttribute("R08/Current Academic Year", null)]
        [NUnit.Framework.TestCaseAttribute("R09/Current Academic Year", null)]
        [NUnit.Framework.TestCaseAttribute("R10/Current Academic Year", null)]
        [NUnit.Framework.TestCaseAttribute("R11/Current Academic Year", null)]
        [NUnit.Framework.TestCaseAttribute("R12/Current Academic Year", null)]
        public virtual void Non_LevyLearnerChangesStandardWithAccompanyingChangeToTheNegotiatedPricePV2_205(string collection_Period, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Non-Levy learner changes standard with accompanying change to the negotiated pric" +
                    "e PV2-205", null, exampleTags);
#line 6
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "Learner Reference Number",
                        "Uln"});
            table1.AddRow(new string[] {
                        "na",
                        "10001000"});
#line 7
 testRunner.Given("the following learners", ((string)(null)), table1, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "Aim Reference",
                        "Start Date",
                        "Planned Duration",
                        "Actual Duration",
                        "Aim Sequence Number",
                        "Programme Type",
                        "Standard Code",
                        "Funding Line Type",
                        "Completion Status"});
            table2.AddRow(new string[] {
                        "ZPROG001",
                        "03/Aug/Current Academic Year",
                        "12 months",
                        "3 months",
                        "1",
                        "25",
                        "51",
                        "16-18 Apprenticeship Non-Levy",
                        "withdrawn"});
            table2.AddRow(new string[] {
                        "ZPROG001",
                        "03/Nov/Current Academic Year",
                        "9 months",
                        "",
                        "2",
                        "25",
                        "52",
                        "16-18 Apprenticeship Non-Levy",
                        "continuing"});
#line 10
 testRunner.And("the following aims", ((string)(null)), table2, "And ");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "Price Episode Id",
                        "Total Training Price Effective Date",
                        "Total Assessment Price",
                        "Total Assessment Price Effective Date",
                        "Contract Type",
                        "Aim Sequence Number",
                        "SFA Contribution Percentage"});
            table3.AddRow(new string[] {
                        "pe-1",
                        "03/Aug/Current Academic Year",
                        "3000",
                        "03/Aug/Current Academic Year",
                        "Act2",
                        "1",
                        "90%"});
            table3.AddRow(new string[] {
                        "pe-2",
                        "03/Nov/Current Academic Year",
                        "1125",
                        "03/Nov/Current Academic Year",
                        "Act2",
                        "2",
                        "90%"});
#line 14
 testRunner.And("price details as follows", ((string)(null)), table3, "And ");
#line 19
    testRunner.When(string.Format("the ILR file is submitted for the learners for collection period {0}", collection_Period), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "Delivery Period",
                        "On-Programme",
                        "Completion",
                        "Balancing",
                        "Aim Sequence Number",
                        "Price Episode Identifier"});
            table4.AddRow(new string[] {
                        "Aug/Current Academic Year",
                        "1000",
                        "0",
                        "0",
                        "1",
                        "pe-1"});
            table4.AddRow(new string[] {
                        "Sep/Current Academic Year",
                        "1000",
                        "0",
                        "0",
                        "1",
                        "pe-1"});
            table4.AddRow(new string[] {
                        "Oct/Current Academic Year",
                        "1000",
                        "0",
                        "0",
                        "1",
                        "pe-1"});
            table4.AddRow(new string[] {
                        "Nov/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "1",
                        "pe-1"});
            table4.AddRow(new string[] {
                        "Dec/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "1",
                        "pe-1"});
            table4.AddRow(new string[] {
                        "Jan/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "1",
                        "pe-1"});
            table4.AddRow(new string[] {
                        "Feb/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "1",
                        "pe-1"});
            table4.AddRow(new string[] {
                        "Mar/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "1",
                        "pe-1"});
            table4.AddRow(new string[] {
                        "Apr/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "1",
                        "pe-1"});
            table4.AddRow(new string[] {
                        "May/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "1",
                        "pe-1"});
            table4.AddRow(new string[] {
                        "Jun/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "1",
                        "pe-1"});
            table4.AddRow(new string[] {
                        "Jul/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "1",
                        "pe-1"});
            table4.AddRow(new string[] {
                        "Aug/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "2",
                        "pe-2"});
            table4.AddRow(new string[] {
                        "Sep/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "2",
                        "pe-2"});
            table4.AddRow(new string[] {
                        "Oct/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "2",
                        "pe-2"});
            table4.AddRow(new string[] {
                        "Nov/Current Academic Year",
                        "500",
                        "0",
                        "0",
                        "2",
                        "pe-2"});
            table4.AddRow(new string[] {
                        "Dec/Current Academic Year",
                        "500",
                        "0",
                        "0",
                        "2",
                        "pe-2"});
            table4.AddRow(new string[] {
                        "Jan/Current Academic Year",
                        "500",
                        "0",
                        "0",
                        "2",
                        "pe-2"});
            table4.AddRow(new string[] {
                        "Feb/Current Academic Year",
                        "500",
                        "0",
                        "0",
                        "2",
                        "pe-2"});
            table4.AddRow(new string[] {
                        "Mar/Current Academic Year",
                        "500",
                        "0",
                        "0",
                        "2",
                        "pe-2"});
            table4.AddRow(new string[] {
                        "Apr/Current Academic Year",
                        "500",
                        "0",
                        "0",
                        "2",
                        "pe-2"});
            table4.AddRow(new string[] {
                        "May/Current Academic Year",
                        "500",
                        "0",
                        "0",
                        "2",
                        "pe-2"});
            table4.AddRow(new string[] {
                        "Jun/Current Academic Year",
                        "500",
                        "0",
                        "0",
                        "2",
                        "pe-2"});
            table4.AddRow(new string[] {
                        "Jul/Current Academic Year",
                        "500",
                        "0",
                        "0",
                        "2",
                        "pe-2"});
#line 20
    testRunner.Then("the following learner earnings should be generated", ((string)(null)), table4, "Then ");
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "Collection Period",
                        "Delivery Period",
                        "On-Programme",
                        "Completion",
                        "Balancing"});
            table5.AddRow(new string[] {
                        "R01/Current Academic Year",
                        "Aug/Current Academic Year",
                        "1000",
                        "0",
                        "0"});
            table5.AddRow(new string[] {
                        "R02/Current Academic Year",
                        "Sep/Current Academic Year",
                        "1000",
                        "0",
                        "0"});
            table5.AddRow(new string[] {
                        "R03/Current Academic Year",
                        "Oct/Current Academic Year",
                        "1000",
                        "0",
                        "0"});
            table5.AddRow(new string[] {
                        "R04/Current Academic Year",
                        "Nov/Current Academic Year",
                        "500",
                        "0",
                        "0"});
            table5.AddRow(new string[] {
                        "R05/Current Academic Year",
                        "Dec/Current Academic Year",
                        "500",
                        "0",
                        "0"});
            table5.AddRow(new string[] {
                        "R06/Current Academic Year",
                        "Jan/Current Academic Year",
                        "500",
                        "0",
                        "0"});
            table5.AddRow(new string[] {
                        "R07/Current Academic Year",
                        "Feb/Current Academic Year",
                        "500",
                        "0",
                        "0"});
            table5.AddRow(new string[] {
                        "R08/Current Academic Year",
                        "Mar/Current Academic Year",
                        "500",
                        "0",
                        "0"});
            table5.AddRow(new string[] {
                        "R09/Current Academic Year",
                        "Apr/Current Academic Year",
                        "500",
                        "0",
                        "0"});
            table5.AddRow(new string[] {
                        "R10/Current Academic Year",
                        "May/Current Academic Year",
                        "500",
                        "0",
                        "0"});
            table5.AddRow(new string[] {
                        "R11/Current Academic Year",
                        "Jun/Current Academic Year",
                        "500",
                        "0",
                        "0"});
            table5.AddRow(new string[] {
                        "R12/Current Academic Year",
                        "Jul/Current Academic Year",
                        "500",
                        "0",
                        "0"});
#line 48
    testRunner.And("only the following payments will be calculated", ((string)(null)), table5, "And ");
#line hidden
            TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                        "Collection Period",
                        "Delivery Period",
                        "SFA Co-Funded Payments",
                        "Employer Co-Funded Payments",
                        "Transaction Type"});
            table6.AddRow(new string[] {
                        "R01/Current Academic Year",
                        "Aug/Current Academic Year",
                        "900",
                        "100",
                        "Learning"});
            table6.AddRow(new string[] {
                        "R02/Current Academic Year",
                        "Sep/Current Academic Year",
                        "900",
                        "100",
                        "Learning"});
            table6.AddRow(new string[] {
                        "R03/Current Academic Year",
                        "Oct/Current Academic Year",
                        "900",
                        "100",
                        "Learning"});
            table6.AddRow(new string[] {
                        "R04/Current Academic Year",
                        "Nov/Current Academic Year",
                        "450",
                        "50",
                        "Learning"});
            table6.AddRow(new string[] {
                        "R05/Current Academic Year",
                        "Dec/Current Academic Year",
                        "450",
                        "50",
                        "Learning"});
            table6.AddRow(new string[] {
                        "R06/Current Academic Year",
                        "Jan/Current Academic Year",
                        "450",
                        "50",
                        "Learning"});
            table6.AddRow(new string[] {
                        "R07/Current Academic Year",
                        "Feb/Current Academic Year",
                        "450",
                        "50",
                        "Learning"});
            table6.AddRow(new string[] {
                        "R08/Current Academic Year",
                        "Mar/Current Academic Year",
                        "450",
                        "50",
                        "Learning"});
            table6.AddRow(new string[] {
                        "R09/Current Academic Year",
                        "Apr/Current Academic Year",
                        "450",
                        "50",
                        "Learning"});
            table6.AddRow(new string[] {
                        "R10/Current Academic Year",
                        "May/Current Academic Year",
                        "450",
                        "50",
                        "Learning"});
            table6.AddRow(new string[] {
                        "R11/Current Academic Year",
                        "Jun/Current Academic Year",
                        "450",
                        "50",
                        "Learning"});
            table6.AddRow(new string[] {
                        "R12/Current Academic Year",
                        "Jul/Current Academic Year",
                        "450",
                        "50",
                        "Learning"});
#line 62
    testRunner.And("only the following provider payments will be recorded", ((string)(null)), table6, "And ");
#line hidden
            TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                        "Collection Period",
                        "Delivery Period",
                        "SFA Co-Funded Payments",
                        "Employer Co-Funded Payments",
                        "Transaction Type"});
            table7.AddRow(new string[] {
                        "R01/Current Academic Year",
                        "Aug/Current Academic Year",
                        "900",
                        "100",
                        "Learning"});
            table7.AddRow(new string[] {
                        "R02/Current Academic Year",
                        "Sep/Current Academic Year",
                        "900",
                        "100",
                        "Learning"});
            table7.AddRow(new string[] {
                        "R03/Current Academic Year",
                        "Oct/Current Academic Year",
                        "900",
                        "100",
                        "Learning"});
            table7.AddRow(new string[] {
                        "R04/Current Academic Year",
                        "Nov/Current Academic Year",
                        "450",
                        "50",
                        "Learning"});
            table7.AddRow(new string[] {
                        "R05/Current Academic Year",
                        "Dec/Current Academic Year",
                        "450",
                        "50",
                        "Learning"});
            table7.AddRow(new string[] {
                        "R06/Current Academic Year",
                        "Jan/Current Academic Year",
                        "450",
                        "50",
                        "Learning"});
            table7.AddRow(new string[] {
                        "R07/Current Academic Year",
                        "Feb/Current Academic Year",
                        "450",
                        "50",
                        "Learning"});
            table7.AddRow(new string[] {
                        "R08/Current Academic Year",
                        "Mar/Current Academic Year",
                        "450",
                        "50",
                        "Learning"});
            table7.AddRow(new string[] {
                        "R09/Current Academic Year",
                        "Apr/Current Academic Year",
                        "450",
                        "50",
                        "Learning"});
            table7.AddRow(new string[] {
                        "R10/Current Academic Year",
                        "May/Current Academic Year",
                        "450",
                        "50",
                        "Learning"});
            table7.AddRow(new string[] {
                        "R11/Current Academic Year",
                        "Jun/Current Academic Year",
                        "450",
                        "50",
                        "Learning"});
            table7.AddRow(new string[] {
                        "R12/Current Academic Year",
                        "Jul/Current Academic Year",
                        "450",
                        "50",
                        "Learning"});
#line 76
 testRunner.And("at month end only the following provider payments will be generated", ((string)(null)), table7, "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
