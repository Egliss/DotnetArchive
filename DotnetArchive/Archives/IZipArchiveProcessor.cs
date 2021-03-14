using System.Threading.Tasks;

namespace DotnetArchive.Archives
{
    public interface IZipArchiveProcessor
    {
        public void Process(
            IZipArchiver archive,
            string input, string pattern, string excludePattern, string output,
            bool excludeHidden, bool ignoreCase, bool quiet);

        public Task ProcessAsync(
            IZipArchiver archive,
            string input, string pattern, string excludePattern, string output,
            bool excludeHidden, bool ignoreCase, bool quiet);
    }
}
