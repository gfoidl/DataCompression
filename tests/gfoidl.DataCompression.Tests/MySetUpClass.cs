#if !NETCOREAPP

using System;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests
{
    [SetUpFixture]
    public class MySetUpClass
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }
    }
}
#endif
