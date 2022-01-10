using System.Collections.Generic;
using System.Threading;
using WebService_Lib;
using WebService_Lib.Attributes;
using WebService_Lib.Attributes.Rest;
using WebService_Lib.Server;
using WebService_Lib.Cards;
using WebService_Lib.DB;
using System;

namespace WebService.Controllers
{
    /// <summary>
    /// Controller used to test <c>WebService_Lib</c> functionality.
    /// </summary>
    [Controller]
    public class UserController
    {
        [Autowired]
        private readonly AuthCheck auth = null!;
        public static List<string> usersWantingToBattle = new List<string>();
        public static Dictionary<string, string> battleIds = new Dictionary<string, string>();

        [Get("/score")]
        public Response GetScoreboard(AuthDetails auth)
        {
            Dictionary<string, int> stats = Database.getScoreboard();
            return Response.Json(new Dictionary<string, object> { ["board"] = stats}, Status.Ok);
        }

        [Get("/stats")]
        public Response GetUserStats(AuthDetails auth)
        {
            Dictionary<string, object> stats = Database.getUserStats(auth.Username);
            return Response.Json(stats, Status.Ok);
        }

        [Get("/users")]
        public Response GetUserProfile(PathVariable<string> pathVariable, AuthDetails auth)
        {
            if (pathVariable == null || pathVariable.Value.Length == 0 || auth.Username != pathVariable.Value) return Response.Status(Status.BadRequest);
            Dictionary<string, object> profile = Database.getUserProfile(auth.Username);
            return Response.Json(profile, Status.Ok);
        }

        [Put("/users")]
        public Response SetUserProfile(Dictionary<string, object> payload, PathVariable<string> pathVariable, AuthDetails auth)
        {
            if (pathVariable == null || pathVariable.Value.Length == 0 || auth.Username != pathVariable.Value) return Response.Status(Status.BadRequest);
            Database.setUserProfile(auth.Username, (string) payload["name"], (string) payload["bio"], (string) payload["image"]);
            return Response.Json(new Dictionary<string, object> { ["status"] = true }, Status.Ok);
        }

        [Post("/users")]
        public Response Register(Dictionary<string, object> payload)
        {
            if (payload == null) return Response.Status(Status.BadRequest);
            if (!payload.ContainsKey("username") || !payload.ContainsKey("password") ||
                payload["username"] is not string ||  payload["password"] is not string)
                return Response.Status(Status.BadRequest);
            var result 
                = auth.Register((payload["username"] as string)!, (payload["password"] as string)!);
            return !result.Item1 ? Response.Json(new Dictionary<string, object>() { ["error"] = "user_already_exists" }, Status.Conflict) : Response.Json(new Dictionary<string, object>() { ["token"] = result.Item2 }, Status.Created);
        }

        [Post("/sessions")]
        public Response Login(Dictionary<string, object> payload)
        {
            if (payload == null) return Response.Status(Status.BadRequest);
            if (!payload.ContainsKey("username") || !payload.ContainsKey("password") ||
                !(payload["username"] is string) || !(payload["password"] is string))
                return Response.Status(Status.BadRequest);
            var result = auth.Authenticate((payload["username"] as string)!, (payload["password"] as string)!);
            return !result.Item1 ? Response.Json(new Dictionary<string, object>() { ["error"] = "invalid_credentials" }, Status.Unauthorized) : Response.Json(new Dictionary<string, object>() { ["token"] = result.Item2 }, Status.Created);
        }

        [Get("/testAuth")]
        public Response Secret(Dictionary<string, object> payload, AuthDetails auth)
        {
            return Response.Json(new Dictionary<string, object> { ["username"] = auth.Username, ["token"] = auth.Token }, Status.Ok);
        }

        [Post("/battles")]
        public Response StartBattle(Dictionary<string, object> payload, AuthDetails auth)
        {
            Stack s = Stack.getPlayerDeck(auth.Username);
            if (s.cardCount() == 0)
                return Response.Json(new Dictionary<string, object> { ["error"] = "no_cards_in_deck" }, Status.NoContent);
            string id = "", battlelog = "";
            if (usersWantingToBattle.Count == 0) { 
                usersWantingToBattle.Add(auth.Username);
                while (!battleIds.ContainsKey(auth.Username))
                {
                    Thread.Sleep(1000);
                }
                battlelog = battleIds[auth.Username];
                battleIds.Remove(auth.Username);
            } else {
                id = Guid.NewGuid().ToString();
                string username = usersWantingToBattle[0];
                usersWantingToBattle.Remove(username);
                battlelog = Battle.battle(id, auth.Username, username, Stack.getPlayerDeck(auth.Username), Stack.getPlayerDeck(username));
                battleIds.Add(username, battlelog);
            }
            return Response.Json(new Dictionary<string, object> { ["battlelog"] = battlelog }, Status.Ok);
        }
    }
}