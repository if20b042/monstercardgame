using System.Collections.Generic;
using WebService_Lib;
using WebService_Lib.Server;
using WebService_Lib.DB;
using WebService_Lib.Attributes;
using WebService_Lib.Attributes.Rest;

namespace WebService.Security
{
    [WebService_Lib.Attributes.Security]
    public class SecurityConfig : ISecurity
    {
        public AuthDetails AuthDetails(string token)
        {
            var username = token.Substring(0, token.Length - 6);
            return new AuthDetails(username, token);
        }
        private readonly HashSet<string> tokens = new HashSet<string>();
        private readonly Dictionary<string, string> users = new Dictionary<string, string>();
        public bool Authenticate(string token) => tokens.Contains(token);
        public (bool, string) Register(string username, string password) 
        {
            if (Database.doesUserExist(username)) return (false, "already exists");
            Database.addUser(username, password);
            var token = GenerateToken(username);
            AddToken(token);
            return (true, token);
        }
        public string GenerateToken(string username) => username + "-token";
        public void AddToken(string token) => tokens.Add(token);
        public void RevokeToken(string token) => tokens.Remove(token);
        public Dictionary<Method, List<string>> SecurePaths() => new Dictionary<Method, List<string>>()
        {
            {Method.Delete, new List<string>(){}},
            {Method.Get, new List<string>(){"/testAuth", "/cards", "/deck","/users","/stats","/score"}},
            {Method.Patch, new List<string>(){}},
            {Method.Post, new List<string>(){"/packages", "/transactions/packages", "/battles"}},
            {Method.Put, new List<string>(){"/deck","/users"}}
        };
        public bool CheckCredentials(string username, string password)
        {
            return Database.userAuthenticate(username, password);
        }
    }
}