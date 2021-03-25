using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using ZLogger;

namespace DotnetArchive.Archives
{
    public class DefaultZipArchiver : IArchiver
    {
        private readonly ILogger<DefaultZipArchiver> logger;
        public DefaultZipArchiver(ILogger<DefaultZipArchiver> logger)
        {
            this.logger = logger;
        }

        public DisposableFile Archive(string inputRootPath, IEnumerable<string> files, ILogger logger, bool quiet)
        {
            if(string.IsNullOrEmpty(inputRootPath))
                throw new ArgumentException(nameof(inputRootPath));
            if(Directory.Exists(inputRootPath) == false)
                throw new DirectoryNotFoundException(inputRootPath);
            if(files == null)
                throw new ArgumentNullException(nameof(files));

            var defaultLogLevel = quiet ? LogLevel.Debug : LogLevel.Information;
            var tmpFilePath = Path.Combine(Path.GetTempPath(), "tmp.zip");
            var temporaryFile = DisposableFile.OpenOrCreate(tmpFilePath, true);
            long processedCount = 0;

            using(var zip = ZipFile.Open(tmpFilePath, ZipArchiveMode.Update))
            {
                foreach(var item in files)
                {
                    var file = Path.Combine(inputRootPath, item);
                    zip.CreateEntryFromFile(file, item);
                    processedCount++;
                    this.logger.ZLog(defaultLogLevel, item);
                }
            }
            this.logger.ZLog(defaultLogLevel, " ");
            this.logger.ZLog(defaultLogLevel, "All file archived.");
            this.logger.ZLog(defaultLogLevel, "Processed: {0}", processedCount);

            return temporaryFile;
        }
        public Task<DisposableFile> ArchiveAsync(string inputRootPath, IEnumerable<string> files, ILogger logger, bool quiet, CancellationToken token = default)
        {
            logger.LogWarning("ZipAsync() not implemented. use syncrhonized Zip()");

            return Task.FromResult(this.Archive(inputRootPath, files, logger, quiet));
        }

        public void UnArchive(string archiveFilePath, string outputDirectory, ILogger logger, bool quiet)
        {
            throw new NotImplementedException();
        }

        public Task UnArchiveAsync(string archiveFilePath, string outputDirectory, ILogger logger, bool quiet, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
