using GlobExpressions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotnetArchive
{
    public static class EglGlob
    {
        public struct Options
        {
            // glob root directory
            public string input { get; set; }
            // glob input pattern
            public string pattern { get; set; }
            // glob exclude pattern
            public string excludePattern { get; set; }
            // exclude file hidden attributed 
            public bool excludeHidden { get; set; }
            // ignore word case of pattern matching
            public bool ignoreCase { get; set; }
        }

        public static IEnumerable<string> Run(Options option)
        {
            var options = 0 + (option.ignoreCase ? GlobOptions.CaseInsensitive : 0);
            var files = Glob.Files(option.input, option.pattern, options).ToArray();

            var hideDirectories = Glob.Directories(option.input, option.pattern, options)
                .Select(m => Path.Combine(option.input, m))
                .Where(m => (new DirectoryInfo(m).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                .Select(m => new Regex(m.Replace('\\', '/') + "*"))
                .ToArray();
            var excludeFiles = Glob
                .Files(option.input, option.excludePattern, options)
                .Select(m => Path.Combine(option.input, m).Replace('\\', '/'))
                .ToHashSet();

            foreach(var item in files)
            {
                var file = Path.Combine(option.input, item).Replace('\\', '/');
                // hidden file
                if(option.excludeHidden)
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
        public static IEnumerable<string> Run(string input, string pattern, string excludePattern, bool excludeHidden, bool ignoreCase)
        {
            var options = new Options()
            {
                input = input,
                pattern = pattern,
                excludePattern = excludePattern,
                excludeHidden = excludeHidden,
                ignoreCase = ignoreCase,
            };
            return EglGlob.Run(options);
        }
    }
}
