using NUnit.Framework;
using WebService_Lib.Attributes.Rest;
using WebService_Lib.Server;

namespace WebService_Test.Unit
{
    public class MethodUtilitiesTest
    {
        [Test, TestCase(TestName = "Test 'GetMethod' for method 'GET' by name")]
        public void GetConversionByName()
        {
            var method = "Get";

            var result = MethodUtilities.GetMethod(method);

            Assert.AreEqual(Method.Get, result);
        }
        
        [Test, TestCase(TestName = "Test 'GetMethod' for method 'POST' by name")]
        public void PostConversionByName()
        {
            var method = "Post";

            var result = MethodUtilities.GetMethod(method);

            Assert.AreEqual(Method.Post, result);
        }
        
        [Test, TestCase(TestName = "Test 'GetMethod' for method 'PUT' by name")]
        public void PutConversionByName()
        {
            var method = "Put";

            var result = MethodUtilities.GetMethod(method);

            Assert.AreEqual(Method.Put, result);
        }
        
        [Test, TestCase(TestName = "Test 'GetMethod' for method 'DELETE' by name")]
        public void DeleteConversionByName()
        {
            var method = "Get";

            var result = MethodUtilities.GetMethod(method);

            Assert.AreEqual(Method.Get, result);
        }
        
        [Test, TestCase(TestName = "Test 'GetMethod' for method 'PATCH' by name")]
        public void PatchConversionByName()
        {
            var method = "Patch";

            var result = MethodUtilities.GetMethod(method);

            Assert.AreEqual(Method.Patch, result);
        }
        
        [Test, TestCase(TestName = "Test 'GetMethod' for undefined method")]
        public void UndefinedConversionByName()
        {
            var method = "other";

            var result = MethodUtilities.GetMethod(method);

            Assert.AreEqual(Method.Get, result);
        }
        
        [Test, TestCase(TestName = "Test 'GetMethod' for mixed lower/uppercase method name")]
        public void MixedCasingConversionByName()
        {
            var method = "DeLeTe";

            var result = MethodUtilities.GetMethod(method);

            Assert.AreEqual(Method.Delete, result);
        }
        
        [Test, TestCase(TestName = "Test 'GetMethod' for method 'GET' by type")]
        public void GetConversionByType()
        {
            var method = typeof(Get);

            var result = MethodUtilities.GetMethod(method);

            Assert.AreEqual(Method.Get, result);
        }
        
        [Test, TestCase(TestName = "Test 'GetMethod' for method 'POST' by type")]
        public void PostConversionByType()
        {
            var method = typeof(Post);

            var result = MethodUtilities.GetMethod(method);

            Assert.AreEqual(Method.Post, result);
        }
        
        [Test, TestCase(TestName = "Test 'GetMethod' for method 'PUT' by type")]
        public void PutConversionByType()
        {
            var method = typeof(Put);

            var result = MethodUtilities.GetMethod(method);

            Assert.AreEqual(Method.Put, result);
        }
        
        [Test, TestCase(TestName = "Test 'GetMethod' for method 'DELETE' by type")]
        public void DeleteConversionByType()
        {
            var method = typeof(Delete);

            var result = MethodUtilities.GetMethod(method);

            Assert.AreEqual(Method.Delete, result);
        }
        
        [Test, TestCase(TestName = "Test 'GetMethod' for method 'PATCH' by type")]
        public void PatchConversionByType()
        {
            var method = typeof(Patch);

            var result = MethodUtilities.GetMethod(method);

            Assert.AreEqual(Method.Patch, result);
        }

        
    }
}