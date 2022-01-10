using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebService_Lib.Server
{
    /// <summary>
    /// Used to return responses in REST endpoints.
    /// </summary>
    public class Response
    {
        public bool IsStatus;
        public bool IsText;
        public bool IsJson;

        public uint StatusCode;
        public string StatusName;
        public string? Payload;
        public string? ContentType;

        private Response(uint status)
        {
            this.IsStatus = true;
            this.StatusCode = status;
            this.StatusName = ((Status) this.StatusCode).ToString().ToUpper();
        }

        private Response(Status status)
        {
            var code = (uint)status;
            this.IsStatus = true;
            this.StatusCode = code;
            this.StatusName = status.ToString().ToUpper();
        }

        private Response(Dictionary<string, object> json)
        {
            this.StatusCode = 200;
            this.StatusName = ((Status) this.StatusCode).ToString().ToUpper();
            this.IsJson = true;
            // Deserialize json
            // See: https://www.newtonsoft.com/json/help/html/SerializeDictionary.htm
            this.Payload = JsonConvert.SerializeObject(json);
            this.ContentType = "application/json";
        }
        
        private Response(Dictionary<string, object> json, Status status)
        {
            this.StatusCode = (uint)status;
            this.StatusName = ((Status) this.StatusCode).ToString().ToUpper();
            this.IsJson = true;
            this.Payload = JsonConvert.SerializeObject(json);
            this.ContentType = "application/json";
        }

        private Response(string plainText)
        {
            this.StatusCode = 200;
            this.StatusName = ((Status) this.StatusCode).ToString().ToUpper();
            this.IsText = true;
            this.Payload = plainText;
            this.ContentType = "text/plain; charset=utf-8";
        }
        
        private Response(string plainText, Status status)
        {
            this.StatusCode = (uint)status;
            this.StatusName = ((Status) this.StatusCode).ToString().ToUpper();
            this.IsText = true;
            this.Payload = plainText;
            this.ContentType = "text/plain; charset=utf-8";
        }

        /// <summary>
        /// Returns the given response with no content.
        /// </summary>
        /// <param name="status"></param>
        /// <returns>Returns the given response with no content.</returns>
        public static Response Status(Status status)
        {
            return new Response(status);
        }

        /// <summary>
        /// Returns a JSON response with a 200 status.
        /// </summary>
        /// <param name="json"></param>
        /// <returns>Returns a JSON response with a 200 status.</returns>
        public static Response Json(Dictionary<string, object> json)
        {
            return new Response(json);
        }
        
        /// <summary>
        /// Returns a JSON response with a custom status.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="customStatus"></param>
        /// <returns>Returns a JSON response with a custom status.</returns>
        public static Response Json(Dictionary<string, object> json, Status customStatus)
        {
            return new Response(json, customStatus);
        }

        /// <summary>
        /// Returns a plaintext response with a 200 status.
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns>Returns a JSON response with a 200 status.</returns>
        public static Response PlainText(string plainText)
        {
            return new Response(plainText);
        }
        
        /// <summary>
        /// Returns a plaintext response with a custom status.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="customStatus"></param>
        /// <returns>Returns a plaintext response with a custom status.</returns>
        public static Response PlainText(string plainText, Status customStatus)
        {
            return new Response(plainText, customStatus);
        }
    }
}