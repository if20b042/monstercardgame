using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WebService_Lib.Server.RestServer.TcpClient
{
    /// <summary>
    /// Custom TcpClient for usage with REST workloads.
    /// </summary>
    public class RestClient : ITcpClient
    {
        private readonly System.Net.Sockets.TcpClient client;
        
        public RestClient(System.Net.Sockets.TcpClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Read an incoming REST request.
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns>
        /// The received request as a <c>RequestContext</c> or null
        /// when given endpoint does not exists or an error occurs. 
        /// </returns>
        public RequestContext? ReadRequest(in IMapping mapping)
        {
            // Read request
            // See: https://developer.mozilla.org/en-US/docs/Web/HTTP/Messages
            // And: https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener?view=net-5.0#methods
            // And: https://github.com/kienboec/CountStringFromWeb
            
            Method method = Method.Get;
            string? path = null;
            string? version = null;
            Dictionary<string, string> header = new Dictionary<string, string>();
            object? payload = null;
            string? requestParam = null;
            string? pathVariable = null;
            
            // Set timeout
            // See: https://stackoverflow.com/a/17216265/12347616
            client.ReceiveTimeout = 5000;
            StreamReader reader = new StreamReader(client.GetStream());
            string? line;
            var contentLength = 0;
            bool first = true;
            bool noMapping = false;
            // Read http header information until blank line before payload (when existing)
            try
            {
                while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
                {
                    if (line == null) continue;
                    line = line.Trim();
                    if (first)
                    {
                        var info = line.Split(' ');
                        method = MethodUtilities.GetMethod(info[0]);
                        path = info[1];
                        version = info[2];
                        // Check path
                        if (path.LastIndexOf('?') != -1)
                        {
                            requestParam = path.Substring(path.LastIndexOf('?') + 1);
                            path = path.Substring(0, path.LastIndexOf('?'));
                        }

                        if (!mapping.Contains(method, path))
                        {
                            pathVariable = path.Substring(path.LastIndexOf('/') + 1);
                            // Use Math.Max to counter negative values in paths like '/'
                            path = path.Substring(0, Math.Max(path.LastIndexOf('/'), 0));
                            // No mapping for this endpoint found
                            if (!mapping.Contains(method, path)) noMapping = true;

                        }

                        first = false;
                    }
                    else
                    {
                        var info = line.Split(':');
                        header.Add(info[0].Trim(), info[1].Trim());
                        if (info[0] == "Content-Length") contentLength = int.Parse(info[1]);
                    }
                }
            }
            catch (IOException) { return null; }
            
            if (path == null || version == null) return null;
            
            // Read http body (when existing)
            if (contentLength > 0 && header.ContainsKey("Content-Type"))
            {
                StringBuilder data = new StringBuilder(200);
                char[] buffer = new char[1024];
                int bytesReadTotal = 0;
                while (bytesReadTotal < contentLength)
                {
                    try
                    {
                        var bytesRead = reader.Read(buffer, 0, 1024);
                        bytesReadTotal += bytesRead;
                        if (bytesRead == 0) break;
                        data.Append(buffer, 0, bytesRead);
                    }
                    // IOException can occur when there is a mismatch of the 'Content-Length'
                    // because a different encoding is used
                    // Sending a 'plain/text' payload with special characters (äüö...) is
                    // an example of this
                    catch (IOException) { break; }
                }
                payload = data.ToString();
                RequestContext.ParsePayload(ref payload, header["Content-Type"]);
            }
            // Log request and return RequestContext if the requested endpoint exists
            var request = new RequestContext(method, path, version, header, payload, pathVariable, requestParam);
            Console.Out.WriteLine(request);
            return noMapping ? null : request;
        }

        /// <summary>
        /// Send a given REST response.
        /// </summary>
        /// <param name="response"></param>
        public void SendResponse(in Response response)
        {
            StreamWriter writer = new StreamWriter(client.GetStream()) { AutoFlush = true};
            writer.Write($"HTTP/1.1 {response.StatusCode} {response.StatusName}\r\n");
            writer.Write("Server: WebService_Lib\r\n");
            writer.Write("Connection: close\r\n");
            if (response.IsStatus)
            {
                // Send no payload
                writer.Write("\r\n");
                writer.Close();
            }
            else
            {
                // Send payload
                // See: https://riptutorial.com/dot-net/example/88/sending-a-post-request-with-a-string-payload-using-system-net-webclient
                // And: https://stackoverflow.com/a/4414118/12347616
                writer.Write($"Content-Type: {response.ContentType}\r\n");
                //var data = Encoding.UTF8.GetBytes(response.Payload!);
                //var payload = Encoding.UTF8.GetPreamble().Concat(data).ToArray();
                var payload = Encoding.UTF8.GetBytes(response.Payload!);
                var length = payload.Length;
                writer.Write($"Content-Length: {length}\r\n");
                writer.Write("\r\n");
                // Send proper string (and not 'System.Byte[}')
                // See: https://stackoverflow.com/a/10940923/12347616
                writer.Write(Encoding.UTF8.GetString(payload));
                writer.Close();
            }
        }

        /// <summary>
        /// Dispose the client.
        /// </summary>
        public void Dispose()
        {
            client.Close();
        }
    }
}