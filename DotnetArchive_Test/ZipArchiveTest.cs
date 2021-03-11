using DotnetArchive.Archives;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetArchive_Test
{
    [TestClass]
    public class ZipArchiveTest
    {
        private static ILogger<DefaultZipArchive> logger { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Directory.CreateDirectory("Test");
            // Hidden file
            if(File.Exists("Test/Hide_A.txt"))
                File.SetAttributes("Test/Hide_A.txt", FileAttributes.Normal);

            using(var s1 = File.Create("Test/Hide_A.txt")) { }
            File.SetAttributes("Test/Hide_A.txt", FileAttributes.Hidden);
            // SingleFile
            Directory.CreateDirectory("Test/A");
            using var s2 = File.CreateText("Test/A/A.txt");
            // Empty
            Directory.CreateDirectory("Test/B");
            // MultiFile
            Directory.CreateDirectory("Test/C");
            using var s3 = File.CreateText("Test/C/c_0.txt");
            using var s4 = File.CreateText("Test/C/C_1.txt");

            var loggerFactory = LoggerFactory.Create(m => { });
            ZipArchiveTest.logger = loggerFactory.CreateLogger<DefaultZipArchive>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete("Test", true);
        }

        [TestMethod]
        public async Task _入力無しで何も生成されず例外が発生する()
        {
            File.Delete("output.zip");
            var archive = new DefaultZipArchive(logger);

            await Assert.ThrowsExceptionAsync<ArgumentException>(
                async () => await archive.ZipAsync("", "", "output.zip", false, false, false));

            Assert.IsFalse(File.Exists("output.zip"));
        }

        [TestMethod]
        public async Task _出力先無しで何も生成されず例外が発生する()
        {
            var archive = new DefaultZipArchive(logger);
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                async () => await archive.ZipAsync("./", "*", "", false, false, false));
        }

        [TestMethod]
        public async Task _Zipが出力される()
        {
            File.Delete("output.zip");
            var archive = new DefaultZipArchive(logger);
            await archive.ZipAsync("./", "*", "output.zip", false, false, false);

            Assert.IsTrue(File.Exists("output.zip"));
        }

        [TestMethod]
        public async Task _隠しファイルを考慮したZipが出力される()
        {
            var archive = new DefaultZipArchive(logger);
            // No exclude hidden file
            await archive.ZipAsync("Test", "**/*", "output.zip", false, false, false);
            using(var zip = ZipFile.OpenRead("output.zip"))
            {
                Assert.IsTrue(zip.Entries.Any(m => m.Name == "Hide_A.txt"));
            }

            // Exclude hidden file
            await archive.ZipAsync("Test", "**/*", "output.zip", true, false, false);
            using(var zip = ZipFile.OpenRead("output.zip"))
            {
                Assert.IsFalse(zip.Entries.Any(m => m.Name == "Hide_A.txt"));
            }
        }

        [TestMethod]
        public async Task _大文字小文字を考慮したZipが出力される()
        {
            var archive = new DefaultZipArchive(logger);
            // No exclude hidden file
            await archive.ZipAsync("Test", "**/c*", "output.zip", false, false, false);
            using(var zip = ZipFile.OpenRead("output.zip"))
            {
                Assert.IsTrue(zip.Entries.Count == 1);
            }

            // Exclude hidden file
            await archive.ZipAsync("Test", "**/c*", "output.zip", true, true, false);
            using(var zip = ZipFile.OpenRead("output.zip"))
            {
                Assert.IsTrue(zip.Entries.Count == 2);
            }
        }
    }
}
