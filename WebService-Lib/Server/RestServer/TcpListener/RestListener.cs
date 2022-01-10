using System.Net;
using System.Net.Sockets;
using WebService_Lib.Server.RestServer.TcpClient;

namespace WebService_Lib.Server.RestServer.TcpListener
{
    /// <summary>
    /// Custom TcpListener for usage with REST workloads.
    /// </summary>
    public class RestListener : ITcpListener
    {
        private readonly System.Net.Sockets.TcpListener server;
        
        public RestListener(uint port)
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            server = new System.Net.Sockets.TcpListener(localAddr, (int) port);
        }
        
        /// <summary>
        /// Start listening for requests.
        /// </summary>
        public void Start()
        {
            server.Start();
        }

        /// <summary>
        /// Return a new <c>ITcpClient</c> when a new connection
        /// is made.
        /// </summary>
        /// <returns><c>ITcpClient</c> for the new connection</returns>
        public ITcpClient AcceptTcpClient()
        {
            return new RestClient(server.AcceptTcpClient());
        }

        /// <summary>
        /// Stop listening for requests.
        /// </summary>
        public void Stop()
        {
            try {
                server.Stop();
            } catch(SocketException) {}
        }
    }
}