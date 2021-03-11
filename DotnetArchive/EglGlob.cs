using GlobExpressions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotnetArchive
{
    public static class EglGlob
    {
        public static IEnumerable<string> Run(string input, string pattern, string excludePattern, bool excludeHidden, bool ignoreCase)
        {
            var options = 0 + (ignoreCase ? GlobOptions.CaseInsensitive : 0);

            var files = Glob.Files(input, pattern, options).ToArray();

            var hideDirectories = Glob.Directories(input, pattern, options)
                .Select(m => Path.Combine(input, m))
                .Where(m => (new DirectoryInfo(m).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                .Select(m => new Regex(m.Replace('\\', '/') + "*"))
                .ToArray();
            var excludeFiles = Glob
                .Files(input, excludePattern, options)
                .Select(m => Path.Combine(input, m).Replace('\\', '/'))
                .ToHashSet();

            foreach(var item in files)
            {
                var file = Path.Combine(input, item).Replace('\\','/');
                // hidden file
                if(excludeHidden)
                {
                    var directory = Path.GetDirectoryName(file).Replace('\\', '/');
                    var directoryAttribute = new DirectoryInfo(directory).Attributes;
                    var fileAttribute = File.GetAttributes(file);

                    if((fileAttribute & FileAttributes.Hidden) == FileAttributes.Hidden)
                        continue;
                    if(hideDirectories.Any(m => m.IsMatch(directory)))
                        continue;
                }

                if(excludeFiles.Contains(file))
                    continue;

                yield return item;
            }
        }
    }
}
