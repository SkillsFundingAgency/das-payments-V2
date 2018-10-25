using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Autofac;

namespace SFA.DAS.Payments.Application.Infrastructure.Ioc
{
    public class ContainerFactory
    {
        public static IContainer Container { get; private set; } //Needed to automapper to resolve instances, yuck...
        //TODO: make non static
        public static IContainer CreateContainer()
        {
            return CreateBuilder().Build();
        }

        public static IContainer CreateContainer(ContainerBuilder builder)
        {
            Container = builder.Build();
            return Container;
        }

        public static ContainerBuilder CreateBuilder()
        {
            var builder = new ContainerBuilder();
            RegisterModules(builder);
            return builder;
        }

        //TODO: clean up code and make safer as AppDomains don't seem to be supported in core 2.1 yet.
        private static void RegisterModules(ContainerBuilder builder)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filenames = Directory.GetFiles(path, "SFA.DAS.Payments.*.dll")
                .ToList();

            foreach (var file in filenames)
            {
                try
                {
                    //TODO: support modules in exe??
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                    builder.RegisterAssemblyModules(assembly);
                }
                catch (Exception ex)
                {
                    //TODO: use logger or re-throw exception?
                    Trace.TraceError($"Error loading assembly: {file}. Error: {ex.Message}. {ex}");
                }
            }
        }
    }
}