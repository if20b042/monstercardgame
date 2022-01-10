using System.Collections.Generic;
using NUnit.Framework;
using WebService_Lib;
using WebService_Lib.Attributes;
using WebService_Lib.Attributes.Rest;
using WebService_Lib.Server;

namespace WebService_Test.Unit
{
    public class MappingTest
    {
        
        // Dummy class to test functionality
        [Controller]
        class DummyController
        {
            [Get("/hi")]
            public Response Hi() => Response.PlainText("Hi!");
            [Get("/secure")]
            public Response Secure(AuthDetails? authDetails) => Response.PlainText("Secure");
            [Post("/other/secure")]
            public Response OtherSecure(AuthDetails? authDetails, string? payload) => Response.PlainText("OtherSecure");
            [Put("/insert")]
            public Response Insert(Dictionary<string, object>? json) => Response.PlainText("Insert");
            [Patch("/patch")]
            public Response SomePatch(string? payload, AuthDetails? authDetails) => Response.PlainText("Patch");
            [Delete("/delete")]
            public Response Delete(PathVariable<int> id) => Response.Status(Status.Ok);
            [Delete("/other/delete")]
            public Response DeleteIdByRequest(RequestParam id) => Response.Status(Status.Ok);
        }
        
        private DummyController controller = null!;

        [SetUp]
        public void Setup() => controller = new DummyController();


        [Test, TestCase(TestName = "Count endpoints", Description =
             "Check if number of endpoints, defined in 'DummyController', matches mapping count")]
        public void CountEndpoints()
        {
            var mapping = new Mapping(new List<object> { controller });

            Assert.AreEqual(2, mapping.GetMappings[Method.Get].Count);
            Assert.AreEqual(1, mapping.GetMappings[Method.Post].Count);
            Assert.AreEqual(1, mapping.GetMappings[Method.Put].Count);
            Assert.AreEqual(1, mapping.GetMappings[Method.Patch].Count);
            Assert.AreEqual(2, mapping.GetMappings[Method.Delete].Count);
        }

        [Test, TestCase(TestName = "Check endpoint paths", Description =
             "Check if paths, defined in 'DummyController', are found in the mappings")]
        public void CheckPaths()
        {
            var mapping = new Mapping(new List<object> { controller });

            Assert.IsTrue(mapping.GetMappings[Method.Get].ContainsKey("/hi"));
            Assert.IsTrue(mapping.GetMappings[Method.Get].ContainsKey("/secure"));
            Assert.IsTrue(mapping.GetMappings[Method.Post].ContainsKey("/other/secure"));
            Assert.IsTrue(mapping.GetMappings[Method.Put].ContainsKey("/insert"));
            Assert.IsTrue(mapping.GetMappings[Method.Patch].ContainsKey("/patch"));
            Assert.IsTrue(mapping.GetMappings[Method.Delete].ContainsKey("/delete"));
            Assert.IsTrue(mapping.GetMappings[Method.Delete].ContainsKey("/other/delete"));
        }

        [Test, TestCase(TestName = "Invoke endpoint with no parameters", Description =
             "Invoke GET endpoint '/hi', defined in 'DummyController', with no parameters")]
        public void InvokeNoParams()
        {
            var mapping = new Mapping(new List<object> { this.controller });
            var response = mapping.Invoke(Method.Get, "/hi", null, null, null, null);

            Assert.NotNull(response);
        }

        [Test, TestCase(TestName = "Invoke endpoint with one parameter (AuthDetails)", Description =
             "Invoke GET endpoint '/secure', defined in 'DummyController', with 'AuthDetails' parameter")]
        public void InvokeAuthDetailsParam()
        {
            var authDetails = new AuthDetails("test", "test");

            var mapping = new Mapping(new List<object> { this.controller });
            var response = mapping.Invoke(Method.Get, "/secure", authDetails, null, null, null);

            Assert.NotNull(response);
        }

        [Test, TestCase(TestName = "Invoke endpoint with two parameters (AuthDetails, string)", Description =
             "Invoke POST endpoint '/other/secure', defined in 'DummyController', with 'AuthDetails' and string plaintext parameter")]
        public void InvokeAuthDetailsAndPlainTextParam()
        {
            var authDetails = new AuthDetails("test", "test");
            var plainText = "test";

            var mapping = new Mapping(new List<object> { this.controller });
            var response = mapping.Invoke(Method.Post, "/other/secure", authDetails, plainText, null, null);

            Assert.NotNull(response);
        }

        [Test, TestCase(TestName = "Invoke endpoint with one parameter (Dictionary)", Description =
             "Invoke PUT endpoint '/insert', defined in 'DummyController', with Dictionary json parameter")]
        public void InvokeJsonParam()
        {
            var json = new Dictionary<string, object> { ["test"] = 1 };

            var mapping = new Mapping(new List<object> { this.controller });
            var response = mapping.Invoke(Method.Put, "/insert", null, json, null, null);

            Assert.NotNull(response);
        }

        [Test, TestCase(TestName = "Invoke endpoint with two parameters (string, AuthDetails)", Description =
             "Invoke PATCH endpoint '/patch', defined in 'DummyController', with string plaintext and 'AuthDetails' parameter")]
        public void InvokePlainTextAndAuthDetailsParam()
        {
            var authDetails = new AuthDetails("test", "test");
            var plainText = "test";

            var mapping = new Mapping(new List<object> { this.controller });
            var response = mapping.Invoke(Method.Patch, "/patch", authDetails, plainText, null, null);

            Assert.NotNull(response);
        }

        [Test, TestCase(TestName = "Invoke endpoint with one parameter (PathVariable)", Description =
             "Invoke DELETE endpoint '/delete', defined in 'DummyController', with PathVariable<int> parameter")]
        public void InvokePathVariable()
        {
            var pathVariable = "1";

            var mapping = new Mapping(new List<object> { this.controller });
            var response = mapping.Invoke(Method.Delete, "/delete", null, null, pathVariable, null);

            Assert.NotNull(response);
        }
        
        [Test, TestCase(TestName = "Invoke endpoint with one parameter (RequestParam)", Description =
             "Invoke DELETE endpoint '/other/delete', defined in 'DummyController', with RequestParam parameter")]
        public void InvokeRequestParam()
        {
            var requestParam = "id=1";

            var mapping = new Mapping(new List<object> { this.controller });
            var response = mapping.Invoke(Method.Delete, "/other/delete", null, null, null, requestParam);

            Assert.NotNull(response);
        }

    }
}