using System;
using System.IO;

namespace DotnetArchive
{
    public class DisposableFile : IDisposable
    {
        public string fullFilePath { get; set; } = string.Empty;
        public bool allowNotFound { get; set; } = false;

        private DisposableFile() { }

        /// <summary>
        /// create new file
        /// file will delete when exist
        /// </summary>
        public static DisposableFile CreateNew(string fileName, bool allowNotFound = false)
        {
            var fullPath = Path.Combine(Environment.CurrentDirectory, fileName);
            if(File.Exists(fullPath))
                File.Delete(fullPath);

            using var stream = File.CreateText(fullPath);
            var instance = new DisposableFile()
            {
                fullFilePath = fullPath,
                allowNotFound = allowNotFound,
            };
            return instance;
        }

        /// <summary>
        /// open exist file
        /// file will create when not exist
        /// </summary>
        public static DisposableFile OpenOrCreate(string fileName, bool allowNotFound = false)
        {
            var fullPath = Path.Combine(Environment.CurrentDirectory, fileName);
            if(File.Exists(fullPath))
                File.Delete(fullPath);

            using var stream = File.CreateText(fullPath);
            var instance = new DisposableFile()
            {
                fullFilePath = fullPath,
                allowNotFound = allowNotFound,
            };
            return instance;
        }

        public void Dispose()
        {
            if(this.allowNotFound && File.Exists(this.fullFilePath) == false)
                return;

            if(File.Exists(this.fullFilePath))
                File.Delete(this.fullFilePath);
            // File already disposed
            else throw new FileNotFoundException(this.fullFilePath);
        }
    }
}
