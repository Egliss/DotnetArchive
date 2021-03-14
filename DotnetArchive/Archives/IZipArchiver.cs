using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetArchive.Archives
{
    public interface IZipArchiver
    {
        public DisposableFile Zip(string inputRootPath, IEnumerable<string> files, ILogger logger, bool quiet);
        public Task<DisposableFile> ZipAsync(string inputRootPath, IEnumerable<string> files, ILogger logger, bool quiet);
    }
}
