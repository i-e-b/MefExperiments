﻿using DemoApp.Contracts;
using DemoApp.Implementations;
using DemoApp.Roots;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DemoApp.Unit.Tests
{
    [TestClass]
    public class InjectingMockImplementations
    {
        [TestMethod]
        public void configure_method_populates_roots_with_default_implementations()
        {
            var subject = Program.Configure();

            Assert.IsInstanceOfType(subject.Instance, typeof(DemoInstance));
            Assert.IsInstanceOfType(subject.Instance.DataSource, typeof(DemoDataSource));
        }

        [TestMethod]
        public void actions_invoked_against_interface_are_performed_by_injected_mock()
        {
            var dataSource = new Mock<IDemoDataSource>();
            var output = new Mock<IOutput>();

            var subject = new DemoInstance(dataSource.Object, new []{output.Object});

            Assert.AreSame(dataSource.Object, subject.DataSource);

            subject.GoDoStuff();
            dataSource.Verify(m => m.GetData(It.IsAny<int>()));
        }

    }
}
