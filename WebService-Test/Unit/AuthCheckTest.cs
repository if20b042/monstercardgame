using System.Collections.Generic;
using NUnit.Framework;
using WebService_Lib;
using WebService_Lib.Attributes;
using WebService_Lib.Server;

namespace WebService_Test.Unit
{
    public class AuthCheckTest
    {
        // Dummy class to test functionality
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
        
        private ISecurity security = null!;

        [OneTimeSetUp]
        public void Setup() => security = new DummySecurity();
        

        [Test, TestCase(TestName = "Check secured path", Description =
             "Check if secured path, defined in 'TestSecurity', is really secured")]
        public void CheckSecuredPath()
        {
            var authCheck = new AuthCheck(security);

            var isSecured = authCheck.IsSecured(Method.Get, "/secured");

            Assert.IsTrue(isSecured);
        }

        [Test, TestCase(TestName = "Check not secured path", Description =
             "Check if not secured path is correctly interpreted")]
        public void CheckNotSecuredPath()
        {
            var authCheck = new AuthCheck(security);

            var isSecured = authCheck.IsSecured(Method.Get, "/notsecured");

            Assert.IsFalse(isSecured);
        }

        [Test, TestCase(TestName = "Check authentication with correct credentials", Description =
             "Check if authentication, defined in 'TestSecurity', is correctly handled with correct credentials")]
        public void CheckAuthenticationCorrectCredentials()
        {
            var authCheck = new AuthCheck(security);

            var isAuthenticated = authCheck.Authenticate("admin", "admin");

            Assert.IsTrue(isAuthenticated.Item1);
        }

        [Test, TestCase(TestName = "Check authentication with incorrect credentials", Description =
             "Check if authentication, defined in 'TestSecurity', is correctly handled with incorrect credentials")]
        public void CheckAuthenticationIncorrectCredentials()
        {
            var authCheck = new AuthCheck(security);

            var isAuthenticated = authCheck.Authenticate("admin", "1234");

            Assert.IsFalse(isAuthenticated.Item1);
        }
    }
}