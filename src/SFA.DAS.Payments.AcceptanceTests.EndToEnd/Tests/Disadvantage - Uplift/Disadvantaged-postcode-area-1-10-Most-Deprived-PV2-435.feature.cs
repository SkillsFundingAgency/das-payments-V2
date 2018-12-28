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
namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Tests.Disadvantage_Uplift
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.4.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Non-levy learner - on framework , Disadvantage Uplift 1-10% paid")]
    public partial class Non_LevyLearner_OnFrameworkDisadvantageUplift1_10PaidFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Disadvantaged-postcode-area-1-10-Most-Deprived-PV2-435.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Non-levy learner - on framework , Disadvantage Uplift 1-10% paid", null, ProgrammingLanguage.CSharp, ((string[])(null)));
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
        [NUnit.Framework.DescriptionAttribute("Non-levy learner - on framework , Disadvantage Uplift 1-10% paid")]
        [NUnit.Framework.TestCaseAttribute("R01/Current Academic Year", null)]
        [NUnit.Framework.TestCaseAttribute("R02/Current Academic Year", null)]
        public virtual void Non_LevyLearner_OnFrameworkDisadvantageUplift1_10Paid(string collection_Period, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Non-levy learner - on framework , Disadvantage Uplift 1-10% paid", null, exampleTags);
#line 3
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "Start Date",
                        "Planned Duration",
                        "Total Training Price",
                        "Total Training Price Effective Date",
                        "Total Assessment Price",
                        "Total Assessment Price Effective Date",
                        "Actual Duration",
                        "Completion Status",
                        "Contract Type",
                        "Aim Sequence Number",
                        "Aim Reference",
                        "Framework Code",
                        "Programme Type",
                        "Pathway Code",
                        "Funding Line Type",
                        "SFA Contribution Percentage"});
            table1.AddRow(new string[] {
                        "06/Sep/Last Academic Year",
                        "12 months",
                        "15000",
                        "06/Aug/Last Academic Year",
                        "0",
                        "06/Aug/Last Academic Year",
                        "",
                        "continuing",
                        "Act2",
                        "1",
                        "ZPROG001",
                        "593",
                        "20",
                        "1",
                        "16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)",
                        "90%"});
#line 4
 testRunner.Given("the provider previously submitted the following learner details", ((string)(null)), table1, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "Delivery Period",
                        "On-Programme",
                        "Completion",
                        "Balancing",
                        "FirstDisadvantagePayment"});
            table2.AddRow(new string[] {
                        "Aug/Last Academic Year",
                        "1000",
                        "0",
                        "0",
                        "0"});
            table2.AddRow(new string[] {
                        "Sep/Last Academic Year",
                        "1000",
                        "0",
                        "0",
                        "0"});
            table2.AddRow(new string[] {
                        "Oct/Last Academic Year",
                        "1000",
                        "0",
                        "0",
                        "0"});
            table2.AddRow(new string[] {
                        "Nov/Last Academic Year",
                        "1000",
                        "0",
                        "0",
                        "300"});
            table2.AddRow(new string[] {
                        "Dec/Last Academic Year",
                        "1000",
                        "0",
                        "0",
                        "0"});
            table2.AddRow(new string[] {
                        "Jan/Last Academic Year",
                        "1000",
                        "0",
                        "0",
                        "0"});
            table2.AddRow(new string[] {
                        "Feb/Last Academic Year",
                        "1000",
                        "0",
                        "0",
                        "0"});
            table2.AddRow(new string[] {
                        "Mar/Last Academic Year",
                        "1000",
                        "0",
                        "0",
                        "0"});
            table2.AddRow(new string[] {
                        "Apr/Last Academic Year",
                        "1000",
                        "0",
                        "0",
                        "0"});
            table2.AddRow(new string[] {
                        "May/Last Academic Year",
                        "1000",
                        "0",
                        "0",
                        "0"});
            table2.AddRow(new string[] {
                        "Jun/Last Academic Year",
                        "1000",
                        "0",
                        "0",
                        "0"});
            table2.AddRow(new string[] {
                        "Jul/Last Academic Year",
                        "1000",
                        "0",
                        "0",
                        "0"});
#line 7
    testRunner.And("the following earnings had been generated for the learner", ((string)(null)), table2, "And ");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "Collection Period",
                        "Delivery Period",
                        "SFA Co-Funded Payments",
                        "Employer Co-Funded Payments",
                        "SFA Fully-Funded Payments",
                        "Transaction Type"});
            table3.AddRow(new string[] {
                        "R01/Last Academic Year",
                        "Aug/Last Academic Year",
                        "900",
                        "100",
                        "0",
                        "Learning"});
            table3.AddRow(new string[] {
                        "R02/Last Academic Year",
                        "Sep/Last Academic Year",
                        "900",
                        "100",
                        "0",
                        "Learning"});
            table3.AddRow(new string[] {
                        "R03/Last Academic Year",
                        "Oct/Last Academic Year",
                        "900",
                        "100",
                        "0",
                        "Learning"});
            table3.AddRow(new string[] {
                        "R04/Last Academic Year",
                        "Nov/Last Academic Year",
                        "900",
                        "100",
                        "0",
                        "Learning"});
            table3.AddRow(new string[] {
                        "R05/Last Academic Year",
                        "Dec/Last Academic Year",
                        "900",
                        "100",
                        "0",
                        "Learning"});
            table3.AddRow(new string[] {
                        "R06/Last Academic Year",
                        "Jan/Last Academic Year",
                        "900",
                        "100",
                        "0",
                        "Learning"});
            table3.AddRow(new string[] {
                        "R07/Last Academic Year",
                        "Feb/Last Academic Year",
                        "900",
                        "100",
                        "0",
                        "Learning"});
            table3.AddRow(new string[] {
                        "R08/Last Academic Year",
                        "Mar/Last Academic Year",
                        "900",
                        "100",
                        "0",
                        "Learning"});
            table3.AddRow(new string[] {
                        "R09/Last Academic Year",
                        "Apr/Last Academic Year",
                        "900",
                        "100",
                        "0",
                        "Learning"});
            table3.AddRow(new string[] {
                        "R10/Last Academic Year",
                        "May/Last Academic Year",
                        "900",
                        "100",
                        "0",
                        "Learning"});
            table3.AddRow(new string[] {
                        "R11/Last Academic Year",
                        "Jun/Last Academic Year",
                        "900",
                        "100",
                        "0",
                        "Learning"});
            table3.AddRow(new string[] {
                        "R12/Last Academic Year",
                        "Jul/Last Academic Year",
                        "900",
                        "100",
                        "0",
                        "Learning"});
            table3.AddRow(new string[] {
                        "R04/Last Academic Year",
                        "Nov/Last Academic Year",
                        "0",
                        "0",
                        "300",
                        "FirstDisadvantagePayment"});
#line 21
    testRunner.And("the following provider payments had been generated", ((string)(null)), table3, "And ");
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "Start Date",
                        "Planned Duration",
                        "Total Training Price",
                        "Total Training Price Effective Date",
                        "Total Assessment Price",
                        "Total Assessment Price Effective Date",
                        "Actual Duration",
                        "Completion Status",
                        "Contract Type",
                        "Aim Sequence Number",
                        "Aim Reference",
                        "Framework Code",
                        "Programme Type",
                        "Pathway Code",
                        "Funding Line Type",
                        "SFA Contribution Percentage"});
            table4.AddRow(new string[] {
                        "06/Aug/Last Academic Year",
                        "12 months",
                        "15000",
                        "06/Aug/Last Academic Year",
                        "0",
                        "06/Aug/Last Academic Year",
                        "12 months",
                        "completed",
                        "Act2",
                        "1",
                        "ZPROG001",
                        "593",
                        "20",
                        "1",
                        "16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)",
                        "90%"});
#line 37
    testRunner.But("the Provider now changes the Learner details as follows", ((string)(null)), table4, "But ");
#line 40
 testRunner.When(string.Format("the amended ILR file is re-submitted for the learners in collection period {0}", collection_Period), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "Delivery Period",
                        "On-Programme",
                        "Completion",
                        "Balancing",
                        "SecondDisadvantagePayment"});
            table5.AddRow(new string[] {
                        "Aug/Current Academic Year",
                        "1000",
                        "0",
                        "0",
                        "300"});
            table5.AddRow(new string[] {
                        "Sep/Current Academic Year",
                        "0",
                        "3000",
                        "0",
                        "0"});
#line 41
 testRunner.Then("the following learner earnings should be generated", ((string)(null)), table5, "Then ");
#line hidden
            TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                        "Collection Period",
                        "Delivery Period",
                        "On-Programme",
                        "Completion",
                        "Balancing",
                        "SecondDisadvantagePayment"});
            table6.AddRow(new string[] {
                        "R01/Current Academic Year",
                        "Aug/Current Academic Year",
                        "1000",
                        "0",
                        "0",
                        "300"});
            table6.AddRow(new string[] {
                        "R02/Current Academic Year",
                        "Sep/Current Academic Year",
                        "0",
                        "3000",
                        "0",
                        "0"});
#line 46
    testRunner.And("only the following payments will be calculated", ((string)(null)), table6, "And ");
#line hidden
            TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                        "Collection Period",
                        "Delivery Period",
                        "SFA Co-Funded Payments",
                        "Employer Co-Funded Payments",
                        "SFA Fully-Funded Payments",
                        "Transaction Type"});
            table7.AddRow(new string[] {
                        "R01/Current Academic Year",
                        "Aug/Current Academic Year",
                        "900",
                        "100",
                        "0",
                        "Learning"});
            table7.AddRow(new string[] {
                        "R01/Current Academic Year",
                        "Aug/Current Academic Year",
                        "0",
                        "0",
                        "300",
                        "SecondDisadvantagePayment"});
            table7.AddRow(new string[] {
                        "R02/Current Academic Year",
                        "Sep/Current Academic Year",
                        "2700",
                        "300",
                        "0",
                        "Completion"});
#line 50
 testRunner.And("only the following provider payments will be recorded", ((string)(null)), table7, "And ");
#line hidden
            TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
                        "Collection Period",
                        "Delivery Period",
                        "SFA Co-Funded Payments",
                        "Employer Co-Funded Payments",
                        "SFA Fully-Funded Payments",
                        "Transaction Type"});
            table8.AddRow(new string[] {
                        "R01/Current Academic Year",
                        "Aug/Current Academic Year",
                        "900",
                        "100",
                        "0",
                        "Learning"});
            table8.AddRow(new string[] {
                        "R01/Current Academic Year",
                        "Aug/Current Academic Year",
                        "0",
                        "0",
                        "300",
                        "SecondDisadvantagePayment"});
            table8.AddRow(new string[] {
                        "R02/Current Academic Year",
                        "Sep/Current Academic Year",
                        "2700",
                        "300",
                        "0",
                        "Completion"});
#line 55
 testRunner.And("at month end only the following provider payments will be generated", ((string)(null)), table8, "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
