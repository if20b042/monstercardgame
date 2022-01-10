using NUnit.Framework;
using WebService_Lib.Server;

namespace WebService_Test.Unit
{
    public class PathParamTest
    {
        [Test, TestCase(TestName = "Test string based PathVariable", Description =
             "Test a PathVariable of type string with 'text' value")]
        public void PathVariableString()
        {
            var value = "text";

            var result = new PathVariable<string>(value);

            Assert.IsTrue(result.Ok);
            Assert.AreEqual("text", result.Value);
        }
        
        [Test, TestCase(TestName = "Test int based PathVariable", Description =
             "Test a PathVariable of type int with '1' value")]
        public void PathVariableInt()
        {
            var value = "42";

            var result = new PathVariable<int>(value);

            Assert.IsTrue(result.Ok);
            Assert.AreEqual(42, result.Value);
        }
        
        [Test, TestCase(TestName = "Test wrong PathVariable value", Description =
             "Test a faulty input of a PathVariable of type int with a value of 'text'")]
        public void PathVariableWrongConversion()
        {
            var value = "text";

            var result = new PathVariable<int>(value);

            Assert.IsFalse(result.Ok);
        }
        
        [Test, TestCase(TestName = "Test RequestParam with one parameter")]
        public void RequestParamWithOneParameter()
        {
            var value = "a=1";

            var result = new RequestParam(value);

            Assert.IsFalse(result.Empty);
            Assert.AreEqual(1, result.Value.Count);
            Assert.AreEqual("1", result.Value["a"]);
        }
        
        [Test, TestCase(TestName = "Test RequestParam with two parameters")]
        public void RequestParamWithTwoParameters()
        {
            var value = "a=1&b=test";

            var result = new RequestParam(value);

            Assert.IsFalse(result.Empty);
            Assert.AreEqual(2, result.Value.Count);
            Assert.AreEqual("1", result.Value["a"]);
            Assert.AreEqual("test", result.Value["b"]);
        }
        
        [Test, TestCase(TestName = "Test RequestParam with three parameters")]
        public void RequestParamWithThreeParameters()
        {
            var value = "a=1&b=test&c=c";

            var result = new RequestParam(value);

            Assert.IsFalse(result.Empty);
            Assert.AreEqual(3, result.Value.Count);
            Assert.AreEqual("1", result.Value["a"]);
            Assert.AreEqual("test", result.Value["b"]);
            Assert.AreEqual("c", result.Value["c"]);
        }
        
        [Test, TestCase(TestName = "Test RequestParam with faulty parameter")]
        public void RequestParamWrongConversion()
        {
            var value = "42";

            var result = new RequestParam(value);

            Assert.IsTrue(result.Empty);
        }
    }
}