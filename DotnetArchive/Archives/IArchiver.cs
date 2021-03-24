using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetArchive.Archives
{
    public interface IArchiver
    {
        public DisposableFile Archive(string inputRootPath, IEnumerable<string> files, ILogger logger, bool quiet);
        public Task<DisposableFile> ArchiveAsync(string inputRootPath, IEnumerable<string> files, ILogger logger, bool quiet);
    }
}
