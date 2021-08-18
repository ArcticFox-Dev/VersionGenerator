using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static CommandLine.Parser;

namespace DotNet.GitHubAction
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
                .Build();

            var parser = Default.ParseArguments<ActionInputs>(() => new(), args);
            parser.WithNotParsed(
                errors =>
                {
                    Get<ILoggerFactory>(host)
                        .CreateLogger("DotNet.GitHubAction.Program")
                        .LogError(
                            string.Join(Environment.NewLine, errors.Select(error => error.ToString())));
        
                    Environment.Exit(2);
                });

            await parser.WithParsedAsync(options => BuildVersionString(options, host));
            await host.RunAsync();
        }    
        static TService Get<TService>(IHost host)
            where TService : notnull =>
            host.Services.GetRequiredService<TService>();

        private static async Task BuildVersionString(ActionInputs inputs, IHost host)
        {
            // using ProjectWorkspace workspace = Get<ProjectWorkspace>(host);
            using CancellationTokenSource tokenSource = new();
            
            Console.CancelKeyPress += delegate { tokenSource.Cancel(); };
            
            var now = DateTime.Now.ToString("yyyyMMddHHmm");
            string version = String.Empty;
            try
            {
                version = await File.ReadAllTextAsync($"{inputs.Directory}/{inputs.VersionFilename}",
                    tokenSource.Token);
            }
            catch (Exception e)
            {
            }
            finally
            {
                version = "6.6.6";
            }
            

            // https://docs.github.com/actions/reference/workflow-commands-for-github-actions#setting-an-output-parameter
            Console.WriteLine($"::set-output name=artefact-version::{version}-{inputs.Branch}-{now}");

            Environment.Exit(0);
        }
    }
}



