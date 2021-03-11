using ConsoleAppFramework;
using GlobExpressions;
using Microsoft.Extensions.Logging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ZLogger;

namespace DotnetArchive
{
    class ArchiveCommand : ConsoleAppBase
    {
        private readonly ILogger<ArchiveCommand> logger;

        public ArchiveCommand(ILogger<ArchiveCommand> logger)
        {
            this.logger = logger;
        }

        [Command("zip", "generate zip archive.")]
        public void Zip(
            [Option("i", "input file or directory")] string input,
            [Option("p", "input directory based glob pattern")] string pattern = "*",
            [Option("o", "output zip file with extension")] string output = "output.zip",
            [Option("h", "exclude hidden file")] bool excludeHidden = true,
            [Option("c", "ignore case")] bool ignoreCase = true,
            [Option("q", "quiet infomation message")] bool quiet = false
            )
        {
            // Configuration
            var defaultLogLevel = quiet ? LogLevel.Debug : LogLevel.Information;
            var options = 0 + (ignoreCase ? GlobOptions.CaseInsensitive : 0);

            var files = Glob.Files(input, pattern, options).ToArray();
            if(File.Exists(output))
            {
                File.Delete(output);
            }

            using var zip = ZipFile.Open(output, ZipArchiveMode.Update);

            long processedCount = 0;
            long skipCount = 0;
            foreach(var item in files)
            {
                // hidden file
                if(excludeHidden)
                {
                    var isHidden = ((int)File.GetAttributes(item)) & ((int)FileAttributes.Hidden);
                    if(isHidden > 0)
                        continue;
                }
                try
                {
                    zip.CreateEntryFromFile(item, item);
                }
                catch(IOException ex) when(ex.HResult == -2147024864)
                {
                    skipCount++;
                    this.logger.ZLogWarning("[Skip] {0} using by other process.", item);
                }
                processedCount++;
                this.logger.ZLog(defaultLogLevel, item);
            }

            this.logger.ZLog(defaultLogLevel, " ");
            this.logger.ZLog(defaultLogLevel, "All file archived.");
            this.logger.ZLog(defaultLogLevel, "Processed: {0} Skip: {1}", processedCount, skipCount);
        }
    }
}
