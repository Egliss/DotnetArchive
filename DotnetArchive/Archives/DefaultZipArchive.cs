using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Compression;
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

        public Task ZipAsync(string input, string pattern, string excludePattern, string output, bool excludeHidden, bool ignoreCase, bool quiet)
        {
            if(string.IsNullOrEmpty(input))
                throw new ArgumentException(nameof(input));

            if(File.Exists(output))
                File.Delete(output);

            var defaultLogLevel = quiet ? LogLevel.Debug : LogLevel.Information;
            using var zip = ZipFile.Open(output, ZipArchiveMode.Update);
            var files = EglGlob.Run(input, pattern, excludePattern, excludeHidden, ignoreCase);

            long processedCount = 0;
            long skipCount = 0;
            foreach(var item in files)
            {
                var file = Path.Combine(input, item);

                try
                {
                    zip.CreateEntryFromFile(file, file);
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
