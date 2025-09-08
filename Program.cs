using DbAccessPoint.Database;
using DbAccessPoint.Tests;

namespace DbAccessPoint
{
    public static class Program
    {
        public static void Main()
           => Init().GetAwaiter().GetResult();

        public static async Task Init()
        {
#if DEBUG
            Console.WriteLine("Starting application with testing.");

            DbAccessPoint_Tests.RunTests();

            Console.WriteLine("Initializing application.");
#endif

            try
            {
                await SetupMiddleware();
            } catch (Exception exception) 
            {
                Console.WriteLine("Caught an error with setup of middleware: {0}", exception.Message);
            }

#if DEBUG
            Console.WriteLine("Application started!");
#endif
        }

        public static async Task SetupMiddleware()
        {
#if DEBUG
            Console.WriteLine("Setting up middleware.");
#endif

            var authResult = await DatabaseValidator.CheckAuth("user123");

            if (!authResult.IsAuthenticated)
                throw new InvalidDataException(string.Format("Given user is not authorized in the middleware for id: {0}", authResult.UserId));

#if DEBUG
            Console.WriteLine("Given user is authorized.");
            Console.WriteLine("Now, fetching users data.");
#endif
            var userData = await DatabaseRequester.GetUserData(authResult.UserId);

            if (!userData.IsValid)
#pragma warning disable S3928
                throw new InvalidDataException(string.Format("User data which was accepted in middleware is invalid for id: {0}", authResult.UserId));

#if DEBUG
            Console.WriteLine("Checking permissions.");
#endif

            var permissions = await DatabaseRequester.GetPermissions(userData.Role);

#if DEBUG
            if (permissions.CanAccess) Console.WriteLine("Access granted to all resources");
#endif

            Console.WriteLine("Middleware setup completed.");
        }
    }
}