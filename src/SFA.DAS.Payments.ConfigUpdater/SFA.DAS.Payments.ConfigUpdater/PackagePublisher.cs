using System.Diagnostics;
using System.IO;
using System.Management.Automation.Runspaces;

namespace SFA.DAS.Payments.ConfigUpdater
{
    public class PackagePublisher
    {
        public void PublishPackage(string path, string publishProfile, string vsCommandLinePath)
        {
            CleanAndBuildPackage(path, vsCommandLinePath);
            DeployToServiceFabric(path, publishProfile);
        }

        private void CleanAndBuildPackage(string path, string vsCommandLinePath)
        {
            Process p = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.RedirectStandardInput = true;
            info.UseShellExecute = false;

            p.StartInfo = info;
            p.Start();

            using (StreamWriter sw = p.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine("\"" + vsCommandLinePath + "\"");
                    sw.WriteLine("cd \"" + path + "\"");
                    sw.WriteLine("msbuild \"" + new DirectoryInfo(path).Name + ".sfproj\" /t:clean,Package");
                    sw.WriteLine("exit");
                    p.WaitForExit();
                }
            }
        }

        private static void DeployToServiceFabric(string path, string publishProfile)
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
