using System.Collections.Generic;
using WebService_Lib;
using WebService_Lib.Attributes;
using WebService_Lib.Attributes.Rest;
using WebService_Lib.Server;
using WebService_Lib.Cards;
using WebService_Lib.DB;

namespace WebService.Controllers
{
    /// <summary>
    /// Controller used to test <c>WebService_Lib</c> functionality.
    /// </summary>
    [Controller]
    public class PackageController
    {
        [Post("/packages")]
        public Response CreatePackage(Dictionary<string, object> payload, AuthDetails auth)
        {
            if (auth.Username != "admin")
                return Response.Status(Status.Forbidden);
            Database.savePackage(payload);
            return Response.Json(new Dictionary<string, object> { ["success"] = true}, Status.Ok);
        }

        [Post("/transactions/packages")]
        public Response BuyPackage(AuthDetails auth)
        {
            string res = Database.buyPackage(auth.Username);
            return Response.Json(new Dictionary<string, object> { ["msg"] = res}, Status.Ok);
        }
    }
}