using System;
using System.Diagnostics;
using System.IO;
using System.Management.Automation.Runspaces;
using System.Text;

namespace SFA.DAS.Payments.ConfigUpdater
{
    public class PackagePublisher
    {
        public void PublishPackage(string path, string publishProfile, string vsCommandLinePath, Action<string> printLog)
        {
            CleanAndBuildPackage(path, vsCommandLinePath, printLog);
            DeployToServiceFabric(path, publishProfile, printLog);
        }

        private static void CleanAndBuildPackage(string path, string vsCommandLinePath, Action<string> printLog)
        {
            var outputBuilder = new StringBuilder();

            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                },
                EnableRaisingEvents = true
            };

            p.Start();

            p.BeginOutputReadLine();

            p.OutputDataReceived += (sender, args) => { if (args.Data != null && !args.Data.Contains("get compilation errors")) outputBuilder.AppendLine(args.Data); };

            var projectName = new DirectoryInfo(path).Name;

            using (var sw = p.StandardInput)
            {
                if (!sw.BaseStream.CanWrite) return;

                sw.WriteLine("\"" + vsCommandLinePath + "\"");

                sw.WriteLine("cd \"" + path + "\"");

                printLog($"Started Building {projectName}");

                var build = $"msbuild \"{projectName}.sfproj\" /t:clean,Package /m /nologo /verbosity:quiet";

                sw.WriteLine(build);

                sw.WriteLine("exit");

                p.WaitForExit();
            }

            var output = outputBuilder.ToString();

            if (output.Contains("error"))
            {
                printLog($"Error Building {projectName}");
                printLog(output);
            }
            else
            {
                printLog($"Finished Building {projectName}");
            }
        }

        private static void DeployToServiceFabric(string path, string publishProfile, Action<string> printLog)
        {
            var packagePath = path + @"\pkg\Debug";
            var deployScript = path + @"\Scripts\Deploy-FabricApplication.ps1";
            var publishProfilePath = path + @"\PublishProfiles\" + publishProfile;

            var runspaceConfiguration = RunspaceConfiguration.Create();
            var runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
            runspace.Open();
            var pipeline = runspace.CreatePipeline();

            pipeline.Commands.AddScript("Connect-ServiceFabricCluster -ConnectionEndpoint localhost:19000");

            var myCommand = new Command(deployScript);
            myCommand.Parameters.Add("ApplicationPackagePath", packagePath);
            myCommand.Parameters.Add("PublishProfileFile", publishProfilePath);
            myCommand.Parameters.Add("UseExistingClusterConnection", true);
            myCommand.Parameters.Add("OverwriteBehavior", "Always");
            pipeline.Commands.Add(myCommand);

            var results = pipeline.Invoke();

            string output;
            using (var sw = new StringWriter())
            {
                foreach (var invoke in results)
                {
                    var result = invoke.ToString();
                    if (result != "System.Fabric.Description.ApplicationDescription")
                        sw.WriteLine(result);
                }

                output = sw.ToString();
            }

            printLog(output);
        }

        public void UnInstallServiceFabricApp(string path, Action<string> printLog)
        {
            //.\Remove-ServiceFabricApplicationAndType.ps1 -applicationName SFA.DAS.Payments.EarningEvents.ServiceFabric -Verbose
            var deployScript = Directory.GetParent(path).FullName + @"\Scripts\ServiceFabric\Remove-ServiceFabricApplicationAndType.ps1";

            var runspaceConfiguration = RunspaceConfiguration.Create();
            var runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
            runspace.Open();
            var pipeline = runspace.CreatePipeline();
            
            var myCommand = new Command(deployScript);
            var applicationName = new DirectoryInfo(path).Name;

            //Datalocks Directory Name has lowercase l but ServiceFabric App has capital L and because of that the uninstall is silently failing
            myCommand.Parameters.Add("applicationName", applicationName.Contains("Datalocks") ? "SFA.DAS.Payments.DataLocks.ServiceFabric" : applicationName);
            
            pipeline.Commands.Add(myCommand);

            var results = pipeline.Invoke();

            string output;
            using (var sw = new StringWriter())
            {
                foreach (var invoke in results)
                {
                    var result = invoke.ToString();
                    if (result != "System.Fabric.Description.ApplicationDescription")
                        sw.WriteLine($"{applicationName} {result}");
                }

                output = sw.ToString();
            }

            printLog(output);
        }
    }
}