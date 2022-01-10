using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace WebService_Lib.Server.RestServer
{
    /// <summary>
    /// Summarises a http request.
    /// See: https://developer.mozilla.org/en-US/docs/Web/HTTP/Messages
    /// </summary>
    public class RequestContext
    {
        public Method Method;
        public string Path;
        public string Version;
        public Dictionary<string, string> Header;
        public object? Payload;
        public string? PathVariable;
        public string? RequestParam;
        

        public RequestContext(
            Method method, string path, string version, Dictionary<string, string> header,
            object? payload, string? pathVariable, string? requestParam
        )
        {
            Method = method;
            Path = path;
            Version = version;
            Header = header;
            Payload = payload;
            PathVariable = pathVariable;
            RequestParam = requestParam;
        }

        /// <summary>
        /// Parse supported payloads to corresponding forms used in <c>WebService_Lib</c>.
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="contentType"></param>
        public static void ParsePayload(ref object? payload, string contentType)
        {
            switch (contentType)
            {
                case "text/plain":
                    if (payload == null || payload.GetType() != typeof(string))
                    {
                        payload = "";
                    }
                    break;
                case "application/json":
                    if (payload == null || payload.GetType() != typeof(string))
                    {
                        payload = "{}";
                    }
                    // Support for single values
                    if ((payload as string)!.StartsWith("\""))
                    {
                        payload = "{\"value\":" + payload + "}";
                    }
                    // Support for arrays
                    if ((payload as string)!.StartsWith("["))
                    {
                        payload = "{\"array\":" + payload + "}";
                    }
                    try
                    {
                        payload = JsonConvert.DeserializeObject<Dictionary<string, object>>((string)payload);
                        payload = new Dictionary<string, object>((Dictionary<string,object>)payload, StringComparer.OrdinalIgnoreCase);
                    }
                    catch (Exception)
                    {
                        payload = null;
                    }
                    break;
                default:
                    payload = null;
                    break;
            }
        }

        /// <summary>
        /// Return RequestContext in a loggable form.
        /// </summary>
        /// <returns>String with a beautified RequestContext</returns>
        public override string ToString()
        {
            var output = new StringBuilder();
            var header = new StringBuilder();
            foreach (KeyValuePair<string, string> entry in Header)
            {
                header.AppendLine($"|| {entry.Key}: {entry.Value}");
            }
            output.AppendLine("//==========");
            output.AppendLine($"|| {Method} {Path} {Version}");
            if (PathVariable != null || RequestParam != null)
            {
                output.AppendLine("||----------");
                if (PathVariable != null) output.AppendLine($"|| PathVariable: {PathVariable}");
                else if (RequestParam != null) output.AppendLine($"|| RequestParam: {RequestParam}");
            }
            output.AppendLine("||----------");
            if (header.Length != 0)
            {
                output.AppendLine("|| Header:");
                output.Append(header);
            }
            if (Payload != null)
            {
                output.AppendLine("||----------");
                output.AppendLine("||Payload:");
                switch (Payload)
                {
                    case string plaintext:
                        output.AppendLine(plaintext);
                        break;
                    case Dictionary<string, object> json:
                        output.AppendLine(
                            "{" + string.Join(",", json.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}");
                        break;
                }
            }
            output.AppendLine("\\\\==========");
            return output.ToString();
        }
    }
}