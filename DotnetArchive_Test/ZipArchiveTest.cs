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
        private static ILogger<DefaultZipArchiver> archiveLog { get; set; }
        private static ILogger<DefaultArchiveProcessor> processorLog { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Directory.CreateDirectory("Test");
            // Hidden file
            if(File.Exists("Test/.Hide_A.txt"))
                File.SetAttributes("Test/.Hide_A.txt", FileAttributes.Normal);

            using(var s1 = File.Create("Test/.Hide_A.txt")) { }
            File.SetAttributes("Test/.Hide_A.txt", FileAttributes.Hidden | FileAttributes.Normal);

            // SingleFile
            Directory.CreateDirectory("Test/A");
            using var s2 = File.CreateText("Test/A/A.txt");
            // Empty
            Directory.CreateDirectory("Test/B");
            // MultiFile
            Directory.CreateDirectory("Test/C");
            using var s3 = File.CreateText("Test/C/c_0.txt");
            using var s4 = File.CreateText("Test/C/C_1.txt");

            Directory.CreateDirectory("Test/C/D");
            using var s5 = File.CreateText("Test/C/D/D_1.txt");
            using var s6 = File.CreateText("Test/C/D/D_2.txt");

            var loggerFactory = LoggerFactory.Create(m => { });
            ZipArchiveTest.archiveLog = loggerFactory.CreateLogger<DefaultZipArchiver>();
            ZipArchiveTest.processorLog = loggerFactory.CreateLogger<DefaultArchiveProcessor>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete("Test", true);
        }

        [TestMethod]
        public async Task _入力無しで例外が発生する()
        {
            var archiver = new DefaultZipArchiver(archiveLog);
            var archive = new DefaultArchiveProcessor(archiver, processorLog);

            await Assert.ThrowsExceptionAsync<ArgumentException>(
                async () => await archive.ArchiveAsync("", "", "", "output.zip", false, false, false));
        }

        [TestMethod]
        public async Task _出力先無しで何も生成されず例外が発生する()
        {
            var archiver = new DefaultZipArchiver(archiveLog);
            var archive = new DefaultArchiveProcessor(archiver, processorLog);

            await Assert.ThrowsExceptionAsync<ArgumentException>(
                async () => await archive.ArchiveAsync("./", "*", "", "", false, false, false));
        }

        [TestMethod]
        public async Task _Zipが出力される()
        {
            var archiver = new DefaultZipArchiver(archiveLog);
            var archive = new DefaultArchiveProcessor(archiver, processorLog);

            await archive.ArchiveAsync("./", "*", "", "output.zip", false, false, false);

            Assert.IsTrue(File.Exists("output.zip"));
        }

        [TestMethod]
        public async Task _再帰を考慮したZip化が成功する()
        {
            var archiver = new DefaultZipArchiver(archiveLog);
            var archive = new DefaultArchiveProcessor(archiver, processorLog);

            await archive.ArchiveAsync("Test", "**/*", "", "output.zip", true, false, false);
            using(var zip = ZipFile.OpenRead("output.zip"))
            {
                Assert.IsTrue(zip.Entries.Count == 5, zip.Entries.Count.ToString());
            }
        }

        [TestMethod]
        public async Task _隠しファイルを考慮したZipが出力される()
        {
            var archiver = new DefaultZipArchiver(archiveLog);
            var archive = new DefaultArchiveProcessor(archiver, processorLog);
            // No exclude hidden file
            await archive.ArchiveAsync("Test", "**/*", "", "output.zip", false, false, false);
            using(var zip = ZipFile.OpenRead("output.zip"))
            {
                Assert.IsTrue(zip.Entries.Any(m => m.Name == ".Hide_A.txt"));
            }

            // Exclude hidden file
            await archive.ArchiveAsync("Test", "**/*", "", "output.zip", true, false, false);
            using(var zip = ZipFile.OpenRead("output.zip"))
            {
                Assert.IsFalse(zip.Entries.Any(m => m.Name == ".Hide_A.txt"));
            }
        }

        [TestMethod]
        public async Task _大文字小文字を考慮したZipが出力される()
        {
            var archiver = new DefaultZipArchiver(archiveLog);
            var archive = new DefaultArchiveProcessor(archiver, processorLog);
            // No exclude hidden file
            await archive.ArchiveAsync("Test", "**/c*", "", "output.zip", false, false, false);
            using(var zip = ZipFile.OpenRead("output.zip"))
            {
                Assert.IsTrue(zip.Entries.Count == 1);
            }

            // Exclude hidden file
            await archive.ArchiveAsync("Test", "**/c*", "", "output.zip", true, true, false);
            using(var zip = ZipFile.OpenRead("output.zip"))
            {
                Assert.IsTrue(zip.Entries.Count == 2);
            }
        }

        [TestMethod]
        public async Task _GlobでIgnoreが指定できる()
        {
            var archiver = new DefaultZipArchiver(archiveLog);
            var archive = new DefaultArchiveProcessor(archiver, processorLog);
            // No exclude hidden file
            await archive.ArchiveAsync("Test", "**/*", "*/D/**", "output.zip", false, false, false);
            using(var zip = ZipFile.OpenRead("output.zip"))
            {
                Assert.IsTrue(zip.Entries.Count == 4);
            }
        }

        [TestMethod]
        public async Task _Zipを解凍できる()
        {
            var archiver = new DefaultZipArchiver(archiveLog);
            var archive = new DefaultArchiveProcessor(archiver, processorLog);

            await archive.ArchiveAsync("Test", "**/*", "", "output.zip", false, false, false);
            await archiver.UnArchiveAsync("output.zip", "Test/Output/", false, null, false);

        }
    }
}
