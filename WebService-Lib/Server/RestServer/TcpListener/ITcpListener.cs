using WebService_Lib.Server.RestServer.TcpClient;

namespace WebService_Lib.Server.RestServer.TcpListener
{
    /// <summary>
    /// Interface that defines methods for a REST-based TcpListener
    /// </summary>
    public interface ITcpListener
    {
        /// <summary>
        /// Start listening for requests.
        /// </summary>
        void Start();

        /// <summary>
        /// Return a new <c>ITcpClient</c> when a new connection
        /// is made.
        /// </summary>
        /// <returns><c>ITcpClient</c> for the new connection</returns>
        ITcpClient AcceptTcpClient();

        /// <summary>
        /// Stop listening for requests.
        /// </summary>
        void Stop();
    }
}