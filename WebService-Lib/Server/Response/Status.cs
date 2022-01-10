namespace WebService_Lib.Server
{
    /// <summary>
    /// Enum consisting of all supported http status codes.
    /// </summary>
    public enum Status
    {
        Ok = 200,
        Created = 201,
        Accepted = 202,
        NoContent = 204,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        Conflict = 409,
        InternalServerError = 500,
        NotImplemented = 501
    }
}