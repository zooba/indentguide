using IndentGuide;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Threading;

namespace UnitTests
{
    [TestClass]
    static class TestsAssemblyInitialize
    {
        [AssemblyInitialize]
        public static void TestsInitialize(TestContext _)
        {
            var jtc = new JoinableTaskContext();
            IndentGuidePackage.JoinableTaskFactory = jtc.Factory;
        }
    }
}
