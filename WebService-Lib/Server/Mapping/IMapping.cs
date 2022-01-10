using System.Collections.Generic;

namespace WebService_Lib.Server
{
    /// <summary>
    /// Interface that defines an wrapper for endpoints.
    /// </summary>
    public interface IMapping
    {
        Dictionary<Method, Dictionary<string, Mapping.MethodCaller>> GetMappings { get; }
        
        /// <summary>
        /// Checks if an endpoint for the given path exists.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <returns>True if an endpoint is mapped, else false.</returns>
        bool Contains(Method method, string path);

        /// <summary>
        /// Invokes the method for the corresponding method and path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="authDetails"></param>
        /// <param name="payload"></param>
        /// <param name="method"></param>
        /// <param name="pathVariable"></param>
        /// <param name="requestParam"></param>
        /// <exception>
        /// Throws an exception when the given endpoint does not exist.
        /// </exception>
        /// <returns>Response as a Response object</returns>
        Response Invoke(
            Method method, string path, AuthDetails? authDetails, object? payload,
            string? pathVariable, string? requestParam
        );
    }
}