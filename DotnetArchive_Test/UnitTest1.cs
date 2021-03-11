using ConsoleAppFramework;
using DotnetArchive;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace DotnetArchive_Test
{
    [TestClass]
    public class ArchiveTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Directory.CreateDirectory("Test");
            File.CreateText("Test");
            // SingleFile
            Directory.CreateDirectory("Test/A");
            File.CreateText("Test/A");
            // Empty
            Directory.CreateDirectory("Test/B");
            // MultiFile
            Directory.CreateDirectory("Test/C");
            File.CreateText("Test/C_0");
            File.CreateText("Test/C_1");
        }
        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete("Test", true);
        }

        [TestMethod]
        public async Task Zip()
        {
            await Host.CreateDefaultBuilder()
                .RunConsoleAppFrameworkAsync<ArchiveCommand>(args);
        }
    }
}
