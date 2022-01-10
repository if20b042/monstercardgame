namespace WebService_Lib
{
    /// <summary>
    /// Represents authentication user details.
    /// </summary>
    public class AuthDetails
    {
        private readonly string username;
        private readonly string token;
        private readonly string userID;
        public string Username => username;
        public string Token => token;

        public string UserID => userID;

        public AuthDetails(string username, string token, string userID = null)
        {
            this.username = username;
            this.token = token;
            this.userID = userID;
        }
    }
}