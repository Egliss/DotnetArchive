using ConsoleAppFramework;
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
        }

        [Command("zip", "generate zip archive.")]
        public async Task Zip(
            [Option("i", "input file or directory")] string input,
            [Option("p", "input directory based glob pattern")] string pattern = "*",
            [Option("o", "output zip file with extension")] string output = "output.zip",
            [Option("h", "exclude hidden file")] bool excludeHidden = true,
            [Option("c", "ignore case")] bool ignoreCase = true,
            [Option("q", "quiet infomation message")] bool quiet = false
            )
        {
            await this.archive.ZipAsync(input, pattern, output, excludeHidden, ignoreCase, quiet);
        }
    }
}
