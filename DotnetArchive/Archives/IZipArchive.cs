using System.Threading.Tasks;

namespace DotnetArchive.Archives
{
    public interface IZipArchive
    {
        public Task ZipAsync(
            string input,
            string pattern,
            string excludePattern,
            string output,
            bool excludeHidden,
            bool ignoreCase,
            bool quiet
        );
    }
}
