using ConsoleAppFramework;
using DotnetArchive.Archives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DotnetArchive
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder()
                .ConfigureLogging((host, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddSimpleConsole();
                })
                .ConfigureServices(m =>
                {
                    m.AddSingleton<IArchiver, DefaultZipArchiver>();
                    m.AddSingleton<IArchiveProcessor, DefaultArchiveProcessor>();
                })
                .RunConsoleAppFrameworkAsync<ArchiveCommand>(args);
        }
    }
}
