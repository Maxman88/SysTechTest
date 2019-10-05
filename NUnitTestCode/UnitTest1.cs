using NUnit.Framework;
using SysTechTest;

namespace NUnitTestCode
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            CtrlT tt = new CtrlT();
            Assert.IsTrue(9 == tt.Sum(5, 4));
        }
    }
}