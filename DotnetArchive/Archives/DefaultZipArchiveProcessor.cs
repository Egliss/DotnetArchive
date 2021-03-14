using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DotnetArchive.Archives
{
    public class DefaultZipArchiveProcessor : IZipArchiveProcessor
    {
        private readonly ILogger<DefaultZipArchiveProcessor> logger;
        public DefaultZipArchiveProcessor(ILogger<DefaultZipArchiveProcessor> logger)
        {
            this.logger = logger;
        }

        public void Process(IZipArchiver archive, string input, string pattern, string excludePattern, string output,
            bool excludeHidden, bool ignoreCase, bool quiet)
        {
            var files = ValidateAndGlob(input, pattern, excludePattern, output, excludeHidden, ignoreCase);
            var file = archive.Zip(input, files, this.logger, quiet);

            File.Move(file.fullFilePath, output);
            file.allowNotFound = true;
            file.Dispose();
        }

        public async Task ProcessAsync(IZipArchiver archive, string input, string pattern, string excludePattern, string output,
            bool excludeHidden, bool ignoreCase, bool quiet)
        {
            var files = ValidateAndGlob(input, pattern, excludePattern, output, excludeHidden, ignoreCase);
            var file = await archive.ZipAsync(input, files, this.logger, quiet);

            File.Move(file.fullFilePath, output);
            file.allowNotFound = true;
            file.Dispose();
        }

        private static IEnumerable<string> ValidateAndGlob(string input, string pattern, string excludePattern, string output, bool excludeHidden, bool ignoreCase)
        {
            if(string.IsNullOrEmpty(input))
                throw new ArgumentException(nameof(input));
            if(string.IsNullOrEmpty(output))
                throw new ArgumentException(nameof(input));

            if(File.Exists(output))
                File.Delete(output);

            return EglGlob.Run(input, pattern, excludePattern, excludeHidden, ignoreCase);
        }
    }
}
