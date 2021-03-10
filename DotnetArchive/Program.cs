using ConsoleAppFramework;
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
                .ConfigureLogging((host,logging) => {
                    logging.ClearProviders();
                    logging.AddSimpleConsole();
                })
                .RunConsoleAppFrameworkAsync<ArchiveCommand>(args);
        }
    }
}
