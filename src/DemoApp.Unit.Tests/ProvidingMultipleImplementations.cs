using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemoApp.Unit.Tests
{
    [TestClass]
    public class ProvidingMultipleImplementations
    {
        [TestMethod]
        public void should_get_internal_implementations_for_a_listed_interface()
        {
            var subject = Program.Configure();

            Assert.AreEqual(1, subject.Outputs.Count());
        }

        [TestMethod]
        public void should_get_internal_and_plugin_implementations_for_a_listed_interface()
        {
            var subject = Program.Configure("./"); // note that DemoApp doesn't have the plugins in this directory -- these unit tests reference them directory.

            Assert.AreEqual(3, subject.Outputs.Count());
        }
    }
}