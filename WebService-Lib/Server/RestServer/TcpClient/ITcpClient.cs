using System;
using System.IO;

namespace WebService_Lib.Server.RestServer.TcpClient
{
    /// <summary>
    /// Interface that defines methods for a REST-based TcpClient
    /// </summary>
    public interface ITcpClient : IDisposable
    {
        /// <summary>
        /// Read an incoming REST request.
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns>
        /// The received request as a <c>RequestContext</c> or null
        /// when given endpoint does not exists or an error occurs. 
        /// </returns>
        RequestContext? ReadRequest(in IMapping mapping);

        /// <summary>
        /// Send a given REST response.
        /// </summary>
        /// <param name="response"></param>
        void SendResponse(in Response response);
    }
}