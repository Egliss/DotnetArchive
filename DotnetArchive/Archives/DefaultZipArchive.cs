using GlobExpressions;
using Microsoft.Extensions.Logging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using ZLogger;

namespace DotnetArchive.Archives
{
    public class DefaultZipArchive : IZipArchive
    {
        private readonly ILogger<DefaultZipArchive> logger;
        public DefaultZipArchive(ILogger<DefaultZipArchive> logger)
        {
            this.logger = logger;
        }

        public Task ZipAsync(string input, string pattern, string output, bool excludeHidden, bool ignoreCase, bool quiet)
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

            return Task.CompletedTask;
        }
    }
}
