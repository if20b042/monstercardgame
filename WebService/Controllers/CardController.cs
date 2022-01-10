using System.Collections.Generic;
using WebService_Lib;
using WebService_Lib.Attributes;
using WebService_Lib.Attributes.Rest;
using WebService_Lib.Server;
using WebService_Lib.Cards;

namespace WebService.Controllers
{
    /// <summary>
    /// Controller used to test <c>WebService_Lib</c> functionality.
    /// </summary>
    [Controller]
    public class CardController
    {
        [Get("/cards")]
        public Response GetCards(Dictionary<string, object> payload, AuthDetails auth)
        {
            Stack s = Stack.getPlayerStack(auth.Username);
            return Response.Json(new Dictionary<string, object> { ["cards"] = s.ToList()}, Status.Ok);
        }

        [Get("/deck")]
        public Response GetCurrentDeck(Dictionary<string, object> payload, AuthDetails auth)
        {
            Stack s = Stack.getPlayerDeck(auth.Username);
            return Response.Json(new Dictionary<string, object> { ["deck"] = s.ToList()}, Status.Ok);
        }

        [Put("/deck")]
        public Response PutDeck(Dictionary<string, object> payload, AuthDetails auth)
        {
            Stack.setPlayerDeck(auth.Username, payload);
            Stack s = Stack.getPlayerDeck(auth.Username);
            return Response.Json(new Dictionary<string, object> { ["deck"] = s.ToList() }, Status.Ok);
        }
    }
}