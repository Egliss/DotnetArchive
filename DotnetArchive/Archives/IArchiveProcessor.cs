using System.Threading.Tasks;

namespace DotnetArchive.Archives
{
    public interface IArchiveProcessor
    {
        public void Process(
            string input, string pattern, string excludePattern, string output,
            bool excludeHidden, bool ignoreCase, bool quiet);

        public Task ProcessAsync(
            string input, string pattern, string excludePattern, string output,
            bool excludeHidden, bool ignoreCase, bool quiet);
    }
}
