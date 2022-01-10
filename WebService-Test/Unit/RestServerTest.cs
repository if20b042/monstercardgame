using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using WebService_Lib.Server;
using WebService_Lib.Server.RestServer;
using WebService_Lib.Server.RestServer.TcpClient;
using WebService_Lib.Server.RestServer.TcpListener;

namespace WebService_Test.Unit
{
    public class RestServerTest
    {
        [Test, TestCase(TestName = "Check request on existing endpoint", Description =
             "Check if 'RestServer' reads and responds to a correctly made request")]
        public void CorrectRequest()
        {
            // Mocking with Moq
            // See: https://github.com/Moq/moq4/wiki/Quickstart
            // And: https://github.com/kienboec/DotnetLightsaberFight/blob/main/DotnetLightsaberFight.Test/CombatTest.cs
            // And: https://stackoverflow.com/a/47723362/12347616
            var mapping = new Mock<IMapping>();
            mapping.Setup(m => m.Contains(Method.Get, "/hi"))
                .Returns(true);
            mapping.Setup(m => m.Invoke(Method.Get, "/hi", null, null, null, null))
                .Returns(Response.Status(Status.Ok));
            var client = new Mock<ITcpClient>();
            client.Setup(i => i.ReadRequest(It.Ref<IMapping>.IsAny))
                .Returns(new RequestContext(Method.Get, "/hi", "http/1.1",
                new Dictionary<string, string>(), null, null, null));
            client.Setup(i => i.SendResponse(It.Ref<Response>.IsAny));
            var listener = new Mock<ITcpListener>();
            listener.Setup(l => l.AcceptTcpClient()).Returns(client.Object);
            var restServer = new RestServer(listener.Object, mapping.Object, null);
            
            Task t = Task.Run(() => restServer.Start());
            Thread.Sleep(100);
            restServer.Stop();
            t.Wait();
            
            client.Verify(i => i.SendResponse(It.Ref<Response>.IsAny), Times.AtLeastOnce);
        }
        
        [Test, TestCase(TestName = "Check request on non existing endpoint", Description =
             "Check if 'RestServer' reads and responds to an incorrectly made request")]
        public void IncorrectRequest()
        {
            var mapping = new Mock<IMapping>();
            mapping.Setup(m => m.Contains(It.IsAny<Method>(), It.IsAny<string>()))
                .Returns(false);
            var client = new Mock<ITcpClient>();
            client.Setup(i => i.ReadRequest(It.Ref<IMapping>.IsAny))
                .Returns((RequestContext?)null);
            client.Setup(i => i.SendResponse(It.Ref<Response>.IsAny));
            var listener = new Mock<ITcpListener>();
            listener.Setup(l => l.AcceptTcpClient()).Returns(client.Object);
            var restServer = new RestServer(listener.Object, mapping.Object, null);
            
            Task t = Task.Run(() => restServer.Start());
            Thread.Sleep(200);
            restServer.Stop();
            t.Wait();
            
            var invoked = client.Invocations.Any(
                i => i.Arguments.Count >= 1 && i.Arguments[0] is Response response && response.StatusCode == 404);
            Assert.True(invoked);
        }
        
        [Test, TestCase(TestName = "Check request on existing endpoint with a payload", Description =
             "Check if 'RestServer' reads and responds (with payload) to a correctly made request")]
        public void CorrectRequestWithPayload()
        {
            var mapping = new Mock<IMapping>();
            mapping.Setup(m => m.Contains(Method.Get, "/hi"))
                .Returns(true);
            mapping.Setup(m => m.Invoke(Method.Get, "/hi", null, null, null, null))
                .Returns(Response.PlainText("Payload!"));
            var client = new Mock<ITcpClient>();
            client.Setup(i => i.ReadRequest(It.Ref<IMapping>.IsAny))
                .Returns(new RequestContext(Method.Get, "/hi", "http/1.1",
                    new Dictionary<string, string>(), null, null, null));
            client.Setup(i => i.SendResponse(It.Ref<Response>.IsAny));
            var listener = new Mock<ITcpListener>();
            listener.Setup(l => l.AcceptTcpClient()).Returns(client.Object);
            var restServer = new RestServer(listener.Object, mapping.Object, null);
            
            Task t = Task.Run(() => restServer.Start());
            Thread.Sleep(200);
            restServer.Stop();
            t.Wait();
            
            var invoked = client.Invocations.Any(
                i => (i.Arguments.Count >= 1 && i.Arguments[0] is Response response && response.Payload == "Payload!"));
            Assert.True(invoked);
        }
    }
}