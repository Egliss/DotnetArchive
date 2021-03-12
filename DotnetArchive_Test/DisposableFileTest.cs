
using DotnetArchive;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DotnetArchive_Test
{
    [TestClass]
    public class DisposableFileTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Directory.CreateDirectory("Test");
            using var file1 = File.CreateText("Test/A1");
            using var file2 = File.CreateText("Test/A2");
        }
        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete("Test", true);
        }

        [TestMethod]
        public void _ファイルが正しく生成_削除される()
        {
            using(var file = DisposableFile.CreateNew("Test/B"))
            {
                Assert.IsTrue(File.Exists("Test/B"));
            }
            Assert.IsFalse(File.Exists("Test/B"));
        }

        [TestMethod]
        public void _ファイルが正しくオープン_削除される()
        {
            using(var file = DisposableFile.OpenOrCreate("Test/A1"))
            {
                Assert.IsTrue(File.Exists("Test/A1"));
            }
            Assert.IsFalse(File.Exists("Test/A1"));
        }

        [TestMethod]
        public void _ファイルが削除済みの場合にallowNotFoundが機能する()
        {
            // CreateNew_Exception
            var f1 = DisposableFile.CreateNew("Test/B");
            File.Delete(f1.fullFilePath);

            Assert.ThrowsException<FileNotFoundException>(() => f1.Dispose());

            // CreateNew_NoError
            using(var f2 = DisposableFile.CreateNew("Test/B", true))
            {
                File.Delete(f2.fullFilePath);
            }

            // Open_Exception
            var f3 = DisposableFile.OpenOrCreate("Test/A1");
            File.Delete(f3.fullFilePath);

            Assert.ThrowsException<FileNotFoundException>(() => f3.Dispose());

            // Open_NoError
            using(var f4 = DisposableFile.OpenOrCreate("Test/A2", true))
            {
                File.Delete(f4.fullFilePath);
            }
        }
    }
}
