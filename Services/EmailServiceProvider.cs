namespace DbAccessPoint.Services
{
    public static class EmailServiceProvider
    {
        public static void SendEmail(string email, string text)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains('@'))
                throw new ArgumentNullException(nameof(email), "To send any email, recepient address must be in correct format.");

            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text), "Email must contain text to be sent to the recepient address.");

            /* Метод имитации полноправных почтовых сообщений. */
            Console.WriteLine("Send email to the \"{0}\" recepient with this text: {1}", email, text);
        }

        public static void SendEmailWelcome(string email)
        {
            /* Метод имитации полноправных почтовых сообщений. */
            SendEmail(email, "Welcome email, sending to you.");
        }
    }
}
