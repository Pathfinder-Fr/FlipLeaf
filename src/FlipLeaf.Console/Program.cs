using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using Serilog;

namespace FlipLeaf
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ContainerBuilder();

            var runtime = LoadRuntimeSettingsFromArgs(args);

            // settings
            builder.RegisterInstance(runtime);
            builder.Register(c => SetupSettings(c.Resolve<RuntimeSettings>()));
            builder.Register(c => SetupLog(c.Resolve<RuntimeSettings>()));

            // engine
            builder.RegisterType<RenderContext>().AsSelf();
            builder.RegisterType<Engine>().AsSelf();

            // parsers
            builder.RegisterAssemblyTypes(typeof(Engine).Assembly)
                .Where(t => t.Namespace == "FlipLeaf.Parsers" && t.Name.EndsWith("Parser") && !t.IsAbstract && !t.IsInterface)
                .AsSelf();

            // pipelines
            builder.RegisterType<Pipelines.MdPipeline>().As<Pipelines.IRenderPipeline>();

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var engine = scope.Resolve<Engine>();
                await engine.RenderAllAsync();
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        private static SiteSettings SetupSettings(RuntimeSettings runtime)
        {
            var path = Path.Combine(runtime.InputDir, runtime.ConfigFile ?? SiteSettings.DefaultFileName);
            if (File.Exists(path))
            {
                return new Parsers.YamlConfigParser().ParseConfig(path);
            }

            return new SiteSettings();
        }

        private static RuntimeSettings LoadRuntimeSettingsFromArgs(string[] args)
        {
            return new RuntimeSettings
            {
                InputDir = Path.GetFullPath((args.Length != 0) ? args[0] : Environment.CurrentDirectory)
            };
        }

        private static ILogger SetupLog(RuntimeSettings runtime)
        {
            var config = new LoggerConfiguration()
                .WriteTo.Console();

            return config.CreateLogger();
        }
    }
}