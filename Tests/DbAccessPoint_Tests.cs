using DbAccessPoint.Database;
using System.Diagnostics;

namespace DbAccessPoint.Tests
{
    public static class DbAccessPoint_Tests
    {
        public static void RunTests()
        {
#if DEBUG
            Console.WriteLine("Running tests.");
#endif

            Console.WriteLine("Testing user input validation:");

            Debug.Assert(DatabaseValidator.ValidateUserInput("abc"), "Expected 'abc' to be valid.");
            Debug.Assert(DatabaseValidator.ValidateUserInput("a"), "Expected 'a' to be invalid.");

            Console.WriteLine("Testing email validation:");

            Debug.Assert(DatabaseValidator.ValidateEmail("test@example.com"), "Expected 'test@example.com' to be valid.");

            try
            {
                Debug.Assert(DatabaseValidator.ValidateEmail("invalid-email"), "Expected 'invalid-email' to be invalid.");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Got an exception while testing invalid email: {0}", exception.Message);
            }

            Console.WriteLine("Testing user registration:");

            var testUser1 = new Dictionary<string, object>
            {
                { "username", "  JohnDoe  " },
                { "email", "john@example.com" }
            };

            var testUser2 = new Dictionary<string, object>
            {
                { "username", "Alice" },
                { "email", "invalid-email" }
            };

            var testUser3 = new Dictionary<string, object>
            {
                { "username", "Bob" },
                { "email", "bob@example.com" }
            };

            bool result1 = DatabaseRequester.ProcessRegistry(testUser1);
            Debug.Assert(result1, "Expected user1 registration to succeed.");

            try
            {
                bool result2 = DatabaseRequester.ProcessRegistry(testUser2);
                Debug.Assert(result2, "Expected user2 registration to fail due to invalid email.");
            } catch(Exception exception)
            {
                Console.WriteLine("Got an exception while trying to registry user with invalid email: {0}", exception.Message);
            }

            bool result3 = DatabaseRequester.ProcessRegistry(testUser3);
            Debug.Assert(result3, "Expected user3 registration to succeed.");

#if DEBUG
            Console.WriteLine("Tests completed.");
#endif
        }
    }
}
