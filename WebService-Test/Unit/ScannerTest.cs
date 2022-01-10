using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using WebService_Lib;
using WebService_Lib.Attributes;
using WebService_Lib.Server;

namespace WebService_Test.Unit
{
    public class ScannerTests
    {
        // Dummy classes to simulate assembly
        [Component]
        private class DummyComponent {}
        [Component]
        private class AnotherDummyComponent {}
        [Controller]
        private class DummyController {}
        [Security]
        private class FaultySecurity {}

        [Security]
        private class DummySecurity : ISecurity
        {
            public AuthDetails AuthDetails(string token)
            {
                var username = token.Substring(0, token.Length - 6);
                return new AuthDetails(token, username);
            }
            private readonly HashSet<string> tokens = new HashSet<string>();
            public bool Authenticate(string token) => tokens.Contains(token);
            public (bool, string) Register(string username, string password) => (true, GenerateToken("admin"));
            public string GenerateToken(string username) => username + "-token";
            public void AddToken(string token) => tokens.Add(token);
            public void RevokeToken(string token) => tokens.Remove(token);
            public Dictionary<Method, List<string>> SecurePaths() => new Dictionary<Method, List<string>>()
            {
                {Method.Delete, new List<string>(){"/secured"}},
                {Method.Get, new List<string>(){"/secured"}},
                {Method.Patch, new List<string>(){"/secured"}},
                {Method.Post, new List<string>(){"/secured"}},
                {Method.Put, new List<string>(){"/secured"}}
            };
            public bool CheckCredentials(string username, string password) => (username == "admin" && password == "admin");
        }
        
        private List<Type> assembly = null!;

        [OneTimeSetUp]
        public void Setup()
        {
            Type dummy = typeof(DummyComponent);
            Type otherDummy = typeof(AnotherDummyComponent);
            Type controller = typeof(DummyController);
            Type faultySecurity = typeof(FaultySecurity);
            Type security = typeof(DummySecurity);
            assembly = new List<Type> { dummy, otherDummy, controller, faultySecurity, security };
        }

        [Test, TestCase(TestName = "Count components", Description =
        "Count components (classes annotated by 'Component', 'Controller' or 'Security') from provided Assembly")]
        public void CountComponents()
        {
            var scanner = new Scanner(assembly);

            var result = scanner.ScanAssembly();

            Assert.AreEqual(4, result.Item1.Count);
        }

        [Test, TestCase(TestName = "Check components types", Description =
        "Check if components have expected types")]
        public void CheckComponentsTypes()
        {
            var scanner = new Scanner(assembly);

            var result = scanner.ScanAssembly();

            Assert.Contains(typeof(DummyComponent), result.Item1);
            Assert.Contains(typeof(AnotherDummyComponent), result.Item1);
            Assert.Contains(typeof(DummyController), result.Item1);
            Assert.Contains(typeof(DummySecurity), result.Item1);
        }

        [Test, TestCase(TestName = "Count controllers", Description =
        "Count controllers (classes annotated by 'Controller') from provided Assembly")]
        public void CountControllers()
        {
            var scanner = new Scanner(assembly);

            var result = scanner.ScanAssembly();

            Assert.AreEqual(1, result.Item2.Count);
        }

        [Test, TestCase(TestName = "Check controller types", Description =
        "Check if controllers have expected types")]
        public void CheckControllerTypes()
        {
            var scanner = new Scanner(assembly);

            var result = scanner.ScanAssembly();

            Assert.Contains(typeof(DummyController), result.Item1);
        }

        [Test, TestCase(TestName = "Security exists", Description =
        "Check if there is a security config (classes annotated by 'Security' implementing 'ISecurity') " +
        "from provided Assembly")]
        public void SecurityExists()
        {
            var scanner = new Scanner(assembly);

            var result = scanner.ScanAssembly();

            Assert.NotNull(result.Item3);
        }

        [Test, TestCase(TestName = "Check security type", Description =
        "Check if security config has expected type")]
        public void CheckSecurityType()
        {
            var scanner = new Scanner(assembly);

            var result = scanner.ScanAssembly();

            Assert.AreEqual(typeof(DummySecurity), result.Item3);
        }
        
        [Test, TestCase(TestName = "Check components, controllers and security config with extracted execution assembly", 
             Description = "Check components, controllers and security config from extracted execution Assembly")]
        public void UseRealAssembly()
        {
            // Use assembly of this class
            var executionAssembly = Assembly.GetExecutingAssembly().GetTypes().ToList();
            var scanner = new Scanner(executionAssembly);

            var result = scanner.ScanAssembly();
            
            Assert.NotNull(result.Item3);
            Assert.Contains(typeof(DummyComponent), result.Item1);
            Assert.Contains(typeof(AnotherDummyComponent), result.Item1);
            Assert.Contains(typeof(DummyController), result.Item1);
            Assert.Contains(typeof(DummySecurity), result.Item1);
            Assert.Contains(typeof(DummyController), result.Item2);
            Assert.AreEqual(typeof(DummySecurity), result.Item3);
        }
    }
}