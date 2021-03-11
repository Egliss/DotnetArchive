using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DotnetArchive_Test
{
    public static class Global
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext testContext)
        {
            var info = new DirectoryInfo("./");
            info.Attributes &= ~FileAttributes.Hidden;
        }

        [AssemblyCleanup]
        public static void Finalize(TestContext testContext)
        {
            var info = new DirectoryInfo("./");
            info.Attributes |= FileAttributes.Hidden;
        }
    }
}
