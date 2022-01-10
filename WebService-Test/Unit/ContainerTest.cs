using System;
using System.Collections.Generic;
using NUnit.Framework;
using WebService_Lib;
using WebService_Lib.Attributes;

namespace WebService_Test.Unit
{
    public class ContainerTests
    {

        // Dummy classes to test functionality
        [Component]
        class DummyComponent {}

        [Controller]
        class DummyController
        {
            [Autowired] private DummyComponent component;
            public DummyComponent Component => component;
        }

        [Test, TestCase(TestName = "Count instances in Container", Description =
             "Counts instances initialized by the Container from the component list")]
        public void CountInstances()
        {
            var dummy = typeof(DummyComponent);
            var controller = typeof(DummyController);
            var components = new List<Type> { dummy, controller };

            var container = new Container(components).GetContainer;

            Assert.AreEqual(2, container.Count);
        }

        [Test, TestCase(TestName = "Check instance types", Description =
             "Check if the instances initialized by the Container match the given components")]
        public void CheckTypes()
        {
            var dummy = typeof(DummyComponent);
            var controller = typeof(DummyController);
            var components = new List<Type> { dummy, controller };

            var container = new Container(components).GetContainer;

            Assert.IsTrue(container.ContainsKey(dummy));
            Assert.AreEqual(dummy, container[dummy].GetType());
            Assert.IsTrue(container.ContainsKey(controller));
            Assert.AreEqual(controller, container[controller].GetType());
        }

        [Test, TestCase(TestName = "Test Autowiring", Description =
             "Check if field 'logger' in 'TestController' is automatically set to the instance of 'TestLogger'")]
        public void TestAutowiring()
        {
            var dummy = typeof(DummyComponent);
            var controller = typeof(DummyController);
            var components = new List<Type> { dummy, controller };

            var container = new Container(components).GetContainer;

            var controllerInstance = (DummyController)container[controller];
            Assert.NotNull(controllerInstance.Component);
        }
    }
}