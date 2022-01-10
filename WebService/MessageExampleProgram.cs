using WebService_Lib;

namespace WebService
{
    /// <summary>
    /// Simple example program that shows 'WebService_Lib' functionality
    /// through message endpoints.
    /// </summary>
    internal static class MessageExampleProgram
    {
        static void Main()
        {
            // Start 'WebService_Lib' through its 'SimpleWebService'.
            // This will scan the assembly for 'MessageController' and 'MesssageHandler'
            // and automatically instantiate both of them.
            // The controller will provide the endpoints which are then used
            // in the internal REST server.
            var service = new SimpleWebService();
            service.Start();
        }
    }
}