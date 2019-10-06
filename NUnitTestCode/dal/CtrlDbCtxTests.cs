using NUnit.Framework;
using SysTechTest.dal;

namespace NUnitTestCode.dal
{
    class CtrlDbCtxTests
    {
        [Test]
        public void ConnectionTest()
        {
            DbCtx t_ctx = new DbCtx();
            Assert.IsTrue(true);
        }
    }
}
