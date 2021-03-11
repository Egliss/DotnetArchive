using ConsoleAppFramework;
using Cysharp.Text;
using DotnetArchive.Archives;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DotnetArchive
{
    public class ArchiveCommand : ConsoleAppBase
    {
        private readonly ILogger<ArchiveCommand> logger;
        private readonly IZipArchive archive;

        public ArchiveCommand(ILogger<ArchiveCommand> logger, IZipArchive archive)
        {
            this.logger = logger;
            this.archive = archive;
        }

        [Command("zip", "generate zip archive.")]
        public async Task Zip(
            [Option("i", "input file or directory")] string input,
            [Option("p", "input directory based glob pattern")] string pattern = "*",
            [Option("e", "exclude input directory based glob pattern")] string excludePattern = "*",
            [Option("o", "output zip file with extension")] string output = "output.zip",
            [Option("h", "exclude hidden file")] bool excludeHidden = true,
            [Option("c", "ignore case")] bool ignoreCase = true,
            [Option("q", "quiet infomation message")] bool quiet = false
            )
        {
            this.logger.LogInformation(ZString.Format("-i: {0}", input));
            this.logger.LogInformation(ZString.Format("-p: {0}", pattern));
            this.logger.LogInformation(ZString.Format("-e: {0}", excludePattern));
            this.logger.LogInformation(ZString.Format("-o: {0}", output));
            this.logger.LogInformation(ZString.Format("-h: {0}", excludeHidden));
            this.logger.LogInformation(ZString.Format("-c: {0}", ignoreCase));
            this.logger.LogInformation(ZString.Format("-q: {0}", quiet));

            await this.archive.ZipAsync(input, pattern, excludePattern, output, excludeHidden, ignoreCase, quiet);
        }
    }
}
