using WebService_Lib.Server;

namespace WebService_Lib
{
    /// <summary>
    /// Wrapper Class for a concrete <c>ISecurity</c> class.
    /// Do not use the concrete <c>ISecurity</c> class directly in your code,
    /// instead autowire the <c>AuthCheck</c> class when you want to
    /// invoke security methods.
    /// </summary>
    public class AuthCheck
    {
        private readonly ISecurity security;

        public AuthCheck(ISecurity security)
        {
            this.security = security;
        }

        /// <summary>
        /// Check if a given path is secured by <c>WebService_Lib</c>.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <returns>True if so, otherwise False</returns>
        public bool IsSecured(Method method, string path)
        {
            return this.security.SecurePaths()[method].Contains(path);
        }

        /// <summary>
        /// Authenticate the user through its token.
        /// This method is automatically called when basic Authorization
        /// headers are send to a secured path.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>True, if the token is valid, else False</returns>
        public bool Authenticate(string token)
        {
            return this.security.Authenticate(token);
        }

        /// <summary>
        /// Is used to register new users. When the registration is completed
        /// without errors, the methods <c>Authenticate</c> and <c>CheckDetails</c>
        /// should return <c>True</c>.
        /// Call it manually on your register endpoints.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>
        /// A tuple returning the registration status (successful/not successful) and the
        /// generated access token or <c>""</c> in an error case.
        /// </returns>
        public (bool, string) Register(string username, string password)
        {
            return this.security.Register(username, password);
        }

        /// <summary>
        /// Authenticate the user through its credentials.
        /// This method is NOT called when a payload with credentials is 
        /// send to a secured path.
        /// Call it manually on your login/session endpoints.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>
        /// True and the token, if the credentials are correct,otherwise
        /// False and null are returned
        /// </returns>
        public (bool, string?) Authenticate(string username, string password)
        {
            bool isAuthenticated = false;
            string? token = null;
            if (this.security.CheckCredentials(username, password))
            {
                isAuthenticated = true;
                token = this.security.GenerateToken(username);
                this.security.AddToken(token);
            }

            return (isAuthenticated, token);
        }

        /// <summary>
        /// Revoke manually token access.
        /// </summary>
        /// <param name="token"></param>
        public void RevokeAuthentication(string token)
        {
            this.security.RevokeToken(token);
        }

        /// <summary>
        /// Revoke manually token access through the username.
        /// Note: In order to this to to work, the <c>GenerateToken</c> method
        /// of your concrete <c>ISecurity</c> needs to build always the
        /// same token for the user.
        /// </summary>
        public void RevokeAuthenticationWithUsername(string username)
        {
            this.security.RevokeToken(this.security.GenerateToken(username));
        }

        /// <summary>
        /// Return details of an authenticated user through its token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public AuthDetails AuthDetails(string token)
        {
            return this.security.AuthDetails(token);
        }

    }
}