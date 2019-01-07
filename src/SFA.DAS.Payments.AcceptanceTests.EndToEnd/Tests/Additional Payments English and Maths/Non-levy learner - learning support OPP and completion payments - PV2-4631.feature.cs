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
namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Tests.AdditionalPaymentsEnglishAndMaths
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.4.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Non-levy learner, changes aim reference for English/maths aims and payments are r" +
        "econciled PV2-463")]
    public partial class Non_LevyLearnerChangesAimReferenceForEnglishMathsAimsAndPaymentsAreReconciledPV2_463Feature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Non-levy learner - learning support OPP and completion payments - PV2-463.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Non-levy learner, changes aim reference for English/maths aims and payments are r" +
                    "econciled PV2-463", "\tAs a provider\r\n\tI want payments to be reconciled correctly\r\n\tso that I can chang" +
                    "e the aim reference for English & Maths", ProgrammingLanguage.CSharp, ((string[])(null)));
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
        [NUnit.Framework.DescriptionAttribute("Non-levy learner provider changes aim reference for English/maths aims and paymen" +
            "ts are reconciled PV2-463")]
        [NUnit.Framework.TestCaseAttribute("R12/Current Academic Year", null)]
        public virtual void Non_LevyLearnerProviderChangesAimReferenceForEnglishMathsAimsAndPaymentsAreReconciledPV2_463(string collection_Period, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Non-levy learner provider changes aim reference for English/maths aims and paymen" +
                    "ts are reconciled PV2-463", null, exampleTags);
#line 5
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "Learner Reference Number",
                        "Uln"});
            table1.AddRow(new string[] {
                        "abc123",
                        "12345678"});
#line 6
 testRunner.Given("the following learners", ((string)(null)), table1, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "Aim Type",
                        "Aim Reference",
                        "Start Date",
                        "Planned Duration",
                        "Actual Duration",
                        "Aim Sequence Number",
                        "Framework Code",
                        "Pathway Code",
                        "Programme Type",
                        "Funding Line Type",
                        "Completion Status"});
            table2.AddRow(new string[] {
                        "Programme",
                        "ZPROG001",
                        "06/May/Current Academic Year",
                        "12 months",
                        "",
                        "1",
                        "403",
                        "1",
                        "2",
                        "16-18 Apprenticeship Non-Levy",
                        "continuing"});
            table2.AddRow(new string[] {
                        "Maths or English",
                        "12345",
                        "06/May/Current Academic Year",
                        "12 months",
                        "",
                        "2",
                        "403",
                        "1",
                        "2",
                        "16-18 Apprenticeship Non-Levy",
                        "continuing"});
#line 11
 testRunner.And("the following aims", ((string)(null)), table2, "And ");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "Price Details",
                        "Total Training Price",
                        "Total Training Price Effective Date",
                        "Total Assessment Price",
                        "Total Assessment Price Effective Date",
                        "Contract Type",
                        "Aim Sequence Number",
                        "SFA Contribution Percentage"});
            table3.AddRow(new string[] {
                        "1st price details",
                        "9000",
                        "06/May/Current Academic Year",
                        "0",
                        "06/May/Current Academic Year",
                        "Act2",
                        "1",
                        "90%"});
            table3.AddRow(new string[] {
                        "2nd price details",
                        "0",
                        "06/May/Current Academic Year",
                        "0",
                        "06/May/Current Academic Year",
                        "Act2",
                        "2",
                        "100%"});
#line 15
 testRunner.And("price details as follows", ((string)(null)), table3, "And ");
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "Delivery Period",
                        "On-Programme",
                        "Completion",
                        "Balancing",
                        "OnProgrammeMathsAndEnglish"});
            table4.AddRow(new string[] {
                        "Aug/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "0"});
            table4.AddRow(new string[] {
                        "Sep/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "0"});
            table4.AddRow(new string[] {
                        "Oct/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "0"});
            table4.AddRow(new string[] {
                        "Nov/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "0"});
            table4.AddRow(new string[] {
                        "Dec/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "0"});
            table4.AddRow(new string[] {
                        "Jan/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "0"});
            table4.AddRow(new string[] {
                        "Feb/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "0"});
            table4.AddRow(new string[] {
                        "Mar/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "0"});
            table4.AddRow(new string[] {
                        "Apr/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "0"});
            table4.AddRow(new string[] {
                        "May/Current Academic Year",
                        "600",
                        "0",
                        "0",
                        "39.25"});
            table4.AddRow(new string[] {
                        "Jun/Current Academic Year",
                        "600",
                        "0",
                        "0",
                        "39.25"});
            table4.AddRow(new string[] {
                        "Jul/Current Academic Year",
                        "600",
                        "0",
                        "0",
                        "39.25"});
#line 20
    testRunner.And("the following earnings had been generated for the learner", ((string)(null)), table4, "And ");
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "Collection Period",
                        "Delivery Period",
                        "SFA Co-Funded Payments",
                        "Employer Co-Funded Payments",
                        "SFA Fully-Funded Payments",
                        "Transaction Type"});
            table5.AddRow(new string[] {
                        "R10/Current Academic Year",
                        "May/Current Academic Year",
                        "540",
                        "60",
                        "0",
                        "Learning"});
            table5.AddRow(new string[] {
                        "R11/Current Academic Year",
                        "Jun/Current Academic Year",
                        "540",
                        "60",
                        "0",
                        "Learning"});
            table5.AddRow(new string[] {
                        "R10/Current Academic Year",
                        "May/Current Academic Year",
                        "0",
                        "0",
                        "39.25",
                        "OnProgrammeMathsAndEnglish"});
            table5.AddRow(new string[] {
                        "R11/Current Academic Year",
                        "Jun/Current Academic Year",
                        "0",
                        "0",
                        "39.25",
                        "OnProgrammeMathsAndEnglish"});
#line 34
    testRunner.And("the following provider payments had been generated", ((string)(null)), table5, "And ");
#line hidden
            TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                        "Aim Type",
                        "Aim Reference",
                        "Start Date",
                        "Planned Duration",
                        "Actual Duration",
                        "Aim Sequence Number",
                        "Framework Code",
                        "Pathway Code",
                        "Programme Type",
                        "Funding Line Type",
                        "Completion Status"});
            table6.AddRow(new string[] {
                        "Maths or English",
                        "56789",
                        "06/May/Current Academic Year",
                        "12 months",
                        "",
                        "1",
                        "403",
                        "1",
                        "2",
                        "16-18 Apprenticeship Non-Levy",
                        "continuing"});
            table6.AddRow(new string[] {
                        "Programme",
                        "ZPROG001",
                        "06/May/Current Academic Year",
                        "12 months",
                        "",
                        "2",
                        "403",
                        "1",
                        "2",
                        "16-18 Apprenticeship Non-Levy",
                        "continuing"});
#line 43
    testRunner.But("aims details are changed as follows", ((string)(null)), table6, "But ");
#line hidden
            TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                        "Price Details",
                        "Total Training Price",
                        "Total Training Price Effective Date",
                        "Total Assessment Price",
                        "Total Assessment Price Effective Date",
                        "Contract Type",
                        "Aim Sequence Number",
                        "SFA Contribution Percentage"});
            table7.AddRow(new string[] {
                        "1st price details",
                        "9000",
                        "06/May/Current Academic Year",
                        "0",
                        "06/May/Current Academic Year",
                        "Act2",
                        "2",
                        "90%"});
            table7.AddRow(new string[] {
                        "3rd price details",
                        "0",
                        "06/May/Current Academic Year",
                        "0",
                        "06/May/Current Academic Year",
                        "Act2",
                        "1",
                        "100%"});
#line 47
 testRunner.And("price details are changed as follows", ((string)(null)), table7, "And ");
#line 54
 testRunner.When(string.Format("the amended ILR file is re-submitted for the learners in collection period {0}", collection_Period), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
                        "Delivery Period",
                        "On-Programme",
                        "Completion",
                        "Balancing",
                        "OnProgrammeMathsAndEnglish"});
            table8.AddRow(new string[] {
                        "May/Current Academic Year",
                        "600",
                        "0",
                        "0",
                        "39.25"});
            table8.AddRow(new string[] {
                        "Jun/Current Academic Year",
                        "600",
                        "0",
                        "0",
                        "39.25"});
            table8.AddRow(new string[] {
                        "Jul/Current Academic Year",
                        "600",
                        "0",
                        "0",
                        "39.25"});
#line 56
    testRunner.Then("the following learner earnings should be generated", ((string)(null)), table8, "Then ");
#line hidden
            TechTalk.SpecFlow.Table table9 = new TechTalk.SpecFlow.Table(new string[] {
                        "Collection Period",
                        "Delivery Period",
                        "On-Programme",
                        "Completion",
                        "Balancing",
                        "OnProgrammeMathsAndEnglish"});
            table9.AddRow(new string[] {
                        "R12/Current Academic Year",
                        "May/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "39.25"});
            table9.AddRow(new string[] {
                        "R12/Current Academic Year",
                        "Jun/Current Academic Year",
                        "0",
                        "0",
                        "0",
                        "39.25"});
            table9.AddRow(new string[] {
                        "R12/Current Academic Year",
                        "Jul/Current Academic Year",
                        "600",
                        "0",
                        "0",
                        "39.25"});
#line 71
    testRunner.And("only the following payments will be calculated", ((string)(null)), table9, "And ");
#line hidden
            TechTalk.SpecFlow.Table table10 = new TechTalk.SpecFlow.Table(new string[] {
                        "Collection Period",
                        "Delivery Period",
                        "SFA Co-Funded Payments",
                        "Employer Co-Funded Payments",
                        "SFA Fully-Funded Payments",
                        "Transaction Type"});
            table10.AddRow(new string[] {
                        "R12/Current Academic Year",
                        "May/Current Academic Year",
                        "0",
                        "0",
                        "39.25",
                        "OnProgrammeMathsAndEnglish"});
            table10.AddRow(new string[] {
                        "R12/Current Academic Year",
                        "Jun/Current Academic Year",
                        "0",
                        "0",
                        "39.25",
                        "OnProgrammeMathsAndEnglish"});
            table10.AddRow(new string[] {
                        "R12/Current Academic Year",
                        "Jul/Current Academic Year",
                        "0",
                        "0",
                        "39.25",
                        "OnProgrammeMathsAndEnglish"});
            table10.AddRow(new string[] {
                        "R12/Current Academic Year",
                        "Jul/Current Academic Year",
                        "540",
                        "60",
                        "0",
                        "Learning"});
#line 80
    testRunner.And("only the following provider payments will be recorded", ((string)(null)), table10, "And ");
#line hidden
            TechTalk.SpecFlow.Table table11 = new TechTalk.SpecFlow.Table(new string[] {
                        "Collection Period",
                        "Delivery Period",
                        "SFA Co-Funded Payments",
                        "Employer Co-Funded Payments",
                        "SFA Fully-Funded Payments",
                        "Transaction Type"});
            table11.AddRow(new string[] {
                        "R12/Current Academic Year",
                        "May/Current Academic Year",
                        "0",
                        "0",
                        "39.25",
                        "OnProgrammeMathsAndEnglish"});
            table11.AddRow(new string[] {
                        "R12/Current Academic Year",
                        "Jun/Current Academic Year",
                        "0",
                        "0",
                        "39.25",
                        "OnProgrammeMathsAndEnglish"});
            table11.AddRow(new string[] {
                        "R12/Current Academic Year",
                        "Jul/Current Academic Year",
                        "0",
                        "0",
                        "39.25",
                        "OnProgrammeMathsAndEnglish"});
            table11.AddRow(new string[] {
                        "R12/Current Academic Year",
                        "Jul/Current Academic Year",
                        "540",
                        "60",
                        "0",
                        "Learning"});
#line 89
 testRunner.And("at month end only the following provider payments will be generated", ((string)(null)), table11, "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
