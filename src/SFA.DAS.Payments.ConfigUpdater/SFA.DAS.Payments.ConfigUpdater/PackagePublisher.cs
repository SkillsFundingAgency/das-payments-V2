using System.Management.Automation.Runspaces;

namespace SFA.DAS.Payments.ConfigUpdater
{
    public class PackagePublisher
    {
        public void PublishPackage(string path, string publishProfile)
        {
            var packagePath = path + @"\pkg\Debug";
            var deployScript = path + @"\Scripts\Deploy-FabricApplication.ps1";
            var publishProfilePath = path + @"\PublishProfiles\" + publishProfile;

            var runspaceConfiguration = RunspaceConfiguration.Create();
            var runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
            runspace.Open();
            Pipeline pipeline = runspace.CreatePipeline();

            pipeline.Commands.AddScript("Connect-ServiceFabricCluster -ConnectionEndpoint localhost:19000");

            Command myCommand = new Command(deployScript);
            myCommand.Parameters.Add("ApplicationPackagePath", packagePath);
            myCommand.Parameters.Add("PublishProfileFile", publishProfilePath);
            myCommand.Parameters.Add("UseExistingClusterConnection", true);
            myCommand.Parameters.Add("OverwriteBehavior", "Always");
            pipeline.Commands.Add(myCommand);

            pipeline.Invoke();
        }
    }
}
