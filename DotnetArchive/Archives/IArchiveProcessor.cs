using System.Threading;
using System.Threading.Tasks;

namespace DotnetArchive.Archives
{
    public interface IArchiveProcessor
    {
        public void Archive(
            string input, string pattern, string excludePattern, string output,
            bool excludeHidden, bool ignoreCase, bool quiet);

        public Task ArchiveAsync(
            string input, string pattern, string excludePattern, string output,
            bool excludeHidden, bool ignoreCase, bool quiet, CancellationToken token = default);
    }
}
