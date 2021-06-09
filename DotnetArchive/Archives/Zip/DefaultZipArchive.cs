using Microsoft.Extensions.Logging;
using Mono.Unix;
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
                    var entry = zip.CreateEntryFromFile(file, item);
                    if(Environment.OSVersion.Platform == PlatformID.Unix)
                    {
                        // TODO endian?
                        var permission = (int)UnixFileInfo.GetFileSystemEntry(file).FileAccessPermissions << 16;
                        entry.ExternalAttributes = permission;
                    }
                    else
                    {
                        entry.ExternalAttributes = entry.ExternalAttributes = entry.ExternalAttributes | (Convert.ToInt32("664", 8) << 16); ;
                    }
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

        public void UnArchive(string archiveFilePath, string outputDirectory, bool isOverwrite, ILogger logger, bool quiet)
        {
            if(string.IsNullOrEmpty(archiveFilePath))
                throw new ArgumentException(nameof(archiveFilePath));
            if(File.Exists(archiveFilePath) == false)
                throw new DirectoryNotFoundException(archiveFilePath);
            if(string.IsNullOrEmpty(outputDirectory))
                throw new ArgumentException(nameof(outputDirectory));

            var defaultLogLevel = quiet ? LogLevel.Debug : LogLevel.Information;
            long processedCount = 0;
            using(var zip = ZipFile.OpenRead(archiveFilePath))
            {
                processedCount = zip.Entries.Count;
            }
            ZipFile.ExtractToDirectory(archiveFilePath, outputDirectory, isOverwrite);
            this.logger.ZLog(defaultLogLevel, "All file extracted.");
            this.logger.ZLog(defaultLogLevel, "Processed: {0}", processedCount);
        }

        public async Task UnArchiveAsync(string archiveFilePath, string outputDirectory, bool isOverwrite, ILogger logger, bool quiet, CancellationToken token = default)
        {
            if(string.IsNullOrEmpty(archiveFilePath))
                throw new ArgumentException(nameof(archiveFilePath));
            if(File.Exists(archiveFilePath) == false)
                throw new DirectoryNotFoundException(archiveFilePath);
            if(string.IsNullOrEmpty(outputDirectory))
                throw new ArgumentException(nameof(outputDirectory));

            var defaultLogLevel = quiet ? LogLevel.Debug : LogLevel.Information;
            long processedCount = 0;
            using(var zip = ZipFile.OpenRead(archiveFilePath))
            {
                processedCount = zip.Entries.Count;
            }
            await Task.Run(() => ZipFile.ExtractToDirectory(archiveFilePath, outputDirectory, isOverwrite));
            this.logger.ZLog(defaultLogLevel, "All file extracted.");
            this.logger.ZLog(defaultLogLevel, "Processed: {0}", processedCount);
        }
    }
}
