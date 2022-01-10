using System.Collections.Generic;
using WebService_Lib.Server;

namespace WebService_Lib
{
    /// <summary>
    /// If you want to secure endpoints with WebService_Lib, make sure
    /// to make a Security class implementing <c>ISecurity</c> annotated
    /// with the <c>Security</c> attribute.
    /// </summary>
    public interface ISecurity
    {
        /// <summary>
        /// Is called on secured endpoints to determine if a request with an
        /// authorization header can access the resource.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>True, when the token is valid, else False</returns>
        public bool Authenticate(string token);

        /// <summary>
        /// Is used to register new users. When the registration is completed
        /// without errors, the methods <c>Authenticate</c> and <c>CheckDetails</c>
        /// should return <c>True</c>.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>
        /// A tuple returning the registration status (successful/not successful) and the
        /// generated access token or <c>""</c> in an error case.
        /// </returns>
        public (bool, string) Register(string username, string password);

        /// <summary>
        /// Generate a token that can be used to access secured resources through
        /// the Authorization header.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GenerateToken(string username);

        /// <summary>
        /// Add a token to a persistent or temporary store, so that the
        /// Authenticate method can access it. A added token can be used
        /// to access secured resources through the Authorization header.
        /// </summary>
        /// <param name="token"></param>
        public void AddToken(string token);

        /// <summary>
        /// Revoke token, so that requests with it can not access secured resources
        /// anymore.
        /// </summary>
        /// <param name="token"></param>
        public void RevokeToken(string token);

        /// <summary>
        /// Return details of an authenticated user through its token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public AuthDetails AuthDetails(string token);

        /// <summary>
        /// Used internally to determine which resources should be secured.
        /// </summary>
        /// <returns>
        /// List of strings that contain the paths to the resources that
        /// should be secured.
        /// </returns>
        public Dictionary<Method, List<string>> SecurePaths();

        /// <summary>
        /// Webservice_Lib's <c>AuthCheck</c> handler uses this method to
        /// Authenticate users with their full credentials.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>
        /// True, when the username-password combination is valid, else False
        /// </returns>
        public bool CheckCredentials(string username, string password);
    }
}