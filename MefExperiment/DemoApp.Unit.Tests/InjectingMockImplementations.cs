using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemoApp.Unit.Tests
{
    [TestClass]
    public class InjectingMockImplementations
    {
        [TestInitialize]
        public void Setup() { 
            
        }

        [TestMethod]
        public void actions_invoked_against_interface_are_performed_by_injected_mock() { }

    }
}
