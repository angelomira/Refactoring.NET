using DbAccessPoint.Data.Results;

namespace DbAccessPoint.Database
{
    public static class DatabaseValidator
    {
        public static bool ValidateEmail(string email)
        {
            #if DEBUG
            Console.WriteLine("Validating email.");
            #endif

            return email.Contains('@');
        }

        public static bool ValidateUserInput(string input)
        {
            return !string.IsNullOrEmpty(input) && input.Length > 2;
        }

#pragma warning disable IDE0060
        public static async Task<AuthResult> CheckAuth(string username)
#pragma warning restore IDE0060
        {
            await Task.Delay(100);

            return new AuthResult
            {
                IsAuthenticated = true,
                UserId = new Random().Next(int.MaxValue),
            };
        }

        public static bool ValidateUser(Dictionary<string, object> userData)
        {
            Console.WriteLine($"Validating user: {userData["username"]}.");

            if (!userData.TryGetValue("username", out object? username) || string.IsNullOrEmpty(username.ToString()))
                throw new KeyNotFoundException("Username is required for validation and auth purposes.");

            userData["username"] = username.ToString()!
                                           .Trim()
                                           .ToLower(System.Globalization.CultureInfo.CurrentCulture);

            if (!userData.TryGetValue("email", out object? email) || !email.ToString()!.Contains('@'))
                throw new KeyNotFoundException("Email is required for validation and auth purposes.");

            if (!ValidateEmail(email.ToString()!)) 
                return false;

            return true;
        }
    }
}
