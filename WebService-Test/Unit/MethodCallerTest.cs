using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using WebService_Lib;
using WebService_Lib.Server;

namespace WebService_Test.Unit
{
    public class MethodCallerTest
    {
        // Dummy class to test functionaltiy
        private class DummyEndpoint
        {
            public Response MyMethod0() => Response.Status(Status.Ok);
            public Response MyMethod1(AuthDetails? auth) => Response.Status(Status.Ok);
            public Response MyMethod2(string? payload, AuthDetails? auth)  => Response.Status(Status.Ok);
            public Response MyMethod3(PathVariable<string> pv, string? payload, AuthDetails? auth)
                => Response.Status(Status.Ok);
            public Response MyMethod3B(RequestParam rv, object? payload, AuthDetails? auth)
                => Response.Status(Status.Ok);
            
            public Response MyMethod3C(string? payload, AuthDetails? auth, PathVariable<int> pv)
                => Response.Status(Status.Ok);
        }
        
        
        [Test, TestCase(TestName = "Invoke parameterless endpoint", Description =
             "Invoke endpoint function 'MyMethod0' defined in 'DummyEndpoint' that takes no parameters")]
        public void InvokeWith0Parameters()
        {
            var methodInfo = typeof(DummyEndpoint).GetMethods();
            var method = methodInfo.Single(m => m.Name == "MyMethod0");
            var instance = new DummyEndpoint();
            var mappingParams = new List<Mapping.MappingParams>();
            var pathVariableType = (Type?)null!;
            var methodCaller = new Mapping.MethodCaller(method, instance, mappingParams, pathVariableType);

            var response = methodCaller.Invoke(null, null, null, null);
            
            Assert.NotNull(response);
            Assert.IsTrue(response.IsStatus);
        }
        
        [Test, TestCase(TestName = "Invoke endpoint with 1 parameter", Description =
             "Invoke endpoint function 'MyMethod1' defined in 'DummyEndpoint' that takes 1 parameter")]
        public void InvokeWith1Parameters()
        {
            var methodInfo = typeof(DummyEndpoint).GetMethods();
            var method = methodInfo.Single(m => m.Name == "MyMethod1");
            var instance = new DummyEndpoint();
            var mappingParams = new List<Mapping.MappingParams>() {Mapping.MappingParams.Auth};
            var pathVariableType = (Type?)null!;
            var methodCaller = new Mapping.MethodCaller(method, instance, mappingParams, pathVariableType);

            var response = methodCaller.Invoke(null, null, null, null);
            
            Assert.NotNull(response);
            Assert.IsTrue(response.IsStatus);
        }
        
        [Test, TestCase(TestName = "Invoke endpoint with 2 parameter", Description =
             "Invoke endpoint function 'MyMethod2' defined in 'DummyEndpoint' that takes 2 parameter")]
        public void InvokeWith2Parameters()
        {
            var methodInfo = typeof(DummyEndpoint).GetMethods();
            var method = methodInfo.Single(m => m.Name == "MyMethod2");
            var instance = new DummyEndpoint();
            var mappingParams = new List<Mapping.MappingParams>() 
                {Mapping.MappingParams.Text, Mapping.MappingParams.Auth};
            var pathVariableType = (Type?)null!;
            var methodCaller = new Mapping.MethodCaller(method, instance, mappingParams, pathVariableType);

            var response 
                = methodCaller.Invoke(null, "payload!", null, null);
            
            Assert.NotNull(response);
            Assert.IsTrue(response.IsStatus);
        }
        
        [Test, TestCase(TestName = "Invoke endpoint with 3 parameter", Description =
             "Invoke endpoint function 'MyMethod3' defined in 'DummyEndpoint' that takes 3 parameter")]
        public void InvokeWith3Parameters()
        {
            var methodInfo = typeof(DummyEndpoint).GetMethods();
            var method = methodInfo.Single(m => m.Name == "MyMethod3");
            var instance = new DummyEndpoint();
            var mappingParams = new List<Mapping.MappingParams>() 
                {Mapping.MappingParams.PathVariable, Mapping.MappingParams.Text, Mapping.MappingParams.Auth};
            var pathVariableType = typeof(string);
            var methodCaller = new Mapping.MethodCaller(method, instance, mappingParams, pathVariableType);

            var response 
                = methodCaller.Invoke(null, "payload!", "pathVariable", null);
            
            Assert.NotNull(response);
            Assert.IsTrue(response.IsStatus);
        }
        
        [Test, TestCase(TestName = "Invoke alternative endpoint with 3 parameter", Description =
             "Invoke endpoint function 'MyMethod3B' defined in 'DummyEndpoint' that takes 3 parameter")]
        public void InvokeWith3ParametersB()
        {
            var methodInfo = typeof(DummyEndpoint).GetMethods();
            var method = methodInfo.Single(m => m.Name == "MyMethod3B");
            var instance = new DummyEndpoint();
            var mappingParams = new List<Mapping.MappingParams>() 
                {Mapping.MappingParams.RequestParam, Mapping.MappingParams.Json, Mapping.MappingParams.Auth};
            var pathVariableType = (Type?)null!;
            var methodCaller = new Mapping.MethodCaller(method, instance, mappingParams, pathVariableType);

            var response 
                = methodCaller.Invoke(null, new Dictionary<string, object>() {["a"]="b"}, null, null);
            
            Assert.NotNull(response);
            Assert.IsTrue(response.IsStatus);
        }
        
        [Test, TestCase(TestName = "Invoke alternative endpoint with 3 parameter", Description =
             "Invoke endpoint function 'MyMethod3C' defined in 'DummyEndpoint' that takes 3 parameter")]
        public void InvokeWith3ParametersC()
        {
            var methodInfo = typeof(DummyEndpoint).GetMethods();
            var method = methodInfo.Single(m => m.Name == "MyMethod3C");
            var instance = new DummyEndpoint();
            var mappingParams = new List<Mapping.MappingParams>() 
                {Mapping.MappingParams.Text, Mapping.MappingParams.Auth, Mapping.MappingParams.PathVariable};
            var pathVariableType = typeof(int);
            var methodCaller = new Mapping.MethodCaller(method, instance, mappingParams, pathVariableType);

            var response 
                = methodCaller.Invoke(new AuthDetails("admin", "admin"), "payload!", "42", null);
            
            Assert.NotNull(response);
            Assert.IsTrue(response.IsStatus);
        }
        
    }
}