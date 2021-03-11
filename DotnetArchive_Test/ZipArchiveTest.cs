using DotnetArchive.Archives;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
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
            using(var s1 = File.Create("Test/A.txt")){}
            File.SetAttributes("Test/A.txt", FileAttributes.Hidden);
            // SingleFile
            Directory.CreateDirectory("Test/A");
            using var s2 = File.CreateText("Test/A/A.txt");
            // Empty
            Directory.CreateDirectory("Test/B");
            // MultiFile
            Directory.CreateDirectory("Test/C");
            using var s3 = File.CreateText("Test/C/C_0");
            using var s4 = File.CreateText("Test/C/C_1");

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
    }
}
