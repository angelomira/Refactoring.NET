using DbAccessPoint.Data.Results;
using DbAccessPoint.Data;
using DbAccessPoint.Services;

namespace DbAccessPoint.Database
{
    public static class DatabaseRequester
    {
        public static async Task<UserData> GetUserData(int userId)
        {
#if DEBUG
            Console.WriteLine("Fetching data for user ID: {0}", userId);
#endif
            await Task.Delay(100);

            return new UserData
            {
                IsValid = true,
                Role = "admin"
            };
        }

        public static async Task<PermissionResult> GetPermissions(string role)
        {
#if DEBUG
            Console.WriteLine("Checking permissions for role: {0}", role);
#endif
            await Task.Delay(100);

            return new PermissionResult
            {
                CanAccess = true
            };
        }

        public static async Task<bool> SetUserData(int userId, UserData payload)
        {
#if DEBUG
            Console.WriteLine("Setting up data for user ID: {0}", userId);
#endif
            await Task.Delay(100);

            /* Имитация работы с экземлярами базы данных. */

            return payload is not null;
        }

        public static async Task<bool> SetPermissions(string role, PermissionResult payload)
        {
#if DEBUG
            Console.WriteLine("Setting up permissions for role: {0}", role);
#endif
            await Task.Delay(100);

            /* Имитация работы с экземлярами базы данных. */

            return payload is not null;
        }

        /// <summary>
        /// Метод имитиации пользовательского ввода и процесса регистрации
        /// </summary>
        public static bool ProcessRegistry(Dictionary<string, object> regData)
        {
#if DEBUG
            Console.WriteLine("Processing user registration.");
#endif

            if (!DatabaseValidator.ValidateUser(regData))
            {
                throw new InvalidDataException("Can't validate and registry user with corrupt or incorrect user data.");
            }

            var user = new Dictionary<string, object>
            {
                { "username", regData["username"] },
                { "email", regData["email"] },
                { "createdAt", DateTime.Now }
            };

            /* Имитация записи в базу данных */
            Console.WriteLine("User {0} created successfully.", user["username"]);

            EmailServiceProvider.SendEmailWelcome(regData["email"].ToString()!);

#if DEBUG
            Console.WriteLine("Processing user registration.");
#endif
            return true;
        }
    }
}
