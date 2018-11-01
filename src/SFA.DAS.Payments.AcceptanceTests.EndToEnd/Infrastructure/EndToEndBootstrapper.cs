//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
//using TechTalk.SpecFlow;

//namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd
//{
//    [Binding]
//    public class EndToEndBootstrapper : FeatureScopedBootstrapper
//    {
//        public EndToEndBootstrapper(FeatureContext featureContext) : base(featureContext)
//        {
//        }

//        [BeforeTestRun(Order = 50)]
//        public static void CreateContainer2()
//        {
//            Container = Builder.Build();
//        }
//    }
//}
