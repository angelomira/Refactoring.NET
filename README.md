### **Выполнение задания**

Начнём с глобальных изменений, мы переименуем неймспейс программы для более подходящего контекста, необязательное, визуальное изменение:

```csharp
namespace DbAccessPoint;
```

Нам дан один файл, в котором находятся весь программный код, это сразу же крупное нагромождение, а также нарушение пункта SRP из SOLID, мы используем "God object / God class" в данном случае, разделим программный код на соответствующую структуру и вычленим самые базовые классы, которые используются для передачи данных, а конкретнее:

1) создадим папку для всех классов с данными:

```bash
mkdir "/Data/"
```

2) перенесём класс пользовательских данных в соответствующую папку:

```csharp
namespace DbAccessPoint.Data
{
    public class UserData
    {
        public bool IsValid { get; set; }
        public string Role { get; set; } = "user";
    }
}

```

```ad-attention
title:$\textbf{Важно:}$
С данным классом существуют некоторые проблемы после версии .NET 8.0: мы будем работать с ним чуть позже.
```

3) создадим отдельную директорию под хранение результатов и подобных данных:

```bash
mkdir "/Data/Results/"
```

4) перенесём туда классы результатов запросов и т.п. в соответствующем виде (ниже приведён пример):

```csharp
namespace DbAccessPoint.Data.Results
{
    public class PermissionResult
    {
        public bool CanAccess { get; set; }
    }
}
```

Следующим и крайне важным этапом является работа с конфигурацией базы данных, как минимум её также необходимо перенести в отдельный файл и, самое главное, сделать доступ к этой самой конфигурации безопасной: для этого мы реализуем метод, который возвращает конфигурацию из файла DOTENV в папке проекта:

```csharp
using System.Text;

namespace DbAccessPoint.Database
{
    public static class DatabaseConfig
    {
        public static Dictionary<string, object> ReadConfig()
        {
            var connectionData = new StringBuilder(string.Empty);

            if (!File.Exists(".env"))
                throw new FileNotFoundException("There is no environment file in root to read database configuration.");

            using (var reader = new StreamReader(".env", Encoding.UTF8))
            {
                string? line = reader.ReadLine();

                while (line is not null)
                {
                    connectionData.Append($";{line.TrimEnd()}");
                }
            }

            return new Dictionary<string, object>
            {
                { "dbConnection", connectionData.ToString() },
                { "maxRetries", 3 }
            };
        }
    }
}
```

После этого, приступим к рефакторингу методов для валидации/проверки тех или иных данных, наподобие почты, авторизации и прочих условий, мы назовём его валидатором и он будет в том же неймспейсе базы данных, что и класс конфигуратора:

```csharp
using DbAccessPoint.Data.Results;

namespace DbAccessPoint.Database
{
    public static class DatabaseValidator
    {
        public static bool ValidateEmail(string email)
        {
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
    }
}
```

Чуть поясним выполненные изменения в табличном формате:

|     Функция/метод      | Изменения в объяснении                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
| :--------------------: | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
|   Валидация э-почты    | Метод проверки наличия элемента и так возвращает boolean тип данных, поэтому достаточно просто возвращать эту функцию.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
|    Валидация ввода     | Мы переименовали принимаемый параметр в функции на более подходящее название в нейминге контекста валидатора.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      |
|   Проверка на вход*    | Исправили табуляцию, улучшили (в контексте логики примера) работу с идентификаторами пользователя, небольшой рефакторинг табов и имён.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
| Валидация пользователя | Довольно крупный и немаловажный метод, мы заменили критические выводы в консоль (например, отсутствие юзернейма и т.п.) на ошибки: при таких кейсах, программа именно что должна выдавать ошибку.<br><br>Мы используем отдельный метод, который позволяет параллельно парсить данные и проверять их "наличие", в проверке и парсинге почты, отсутствует проверка на пустоту строчки, на что ругается сам встроенный линтер окружения .NET и поэтому мы также реинтегрируем нулл-чек на строку почты как объекта.<br><br>Используя уже реализованную функцию валидации э-почты в том же классе валидатора, мы ещё больше сокращаем программный код. |

```ad-quote
title:$\textbf{Пояснение:}$
Мы также переименовали большую часть методов и переменных под контекст именно что валидатора, т.е. процесса валидации данных в одном специальном классе; также мы удалили лишние дебаги в консоль из-за общего информационного нагромождения, например, принты при валидации данных не являются обязательными.
```

В случае необходимости реинтеграции дебаггера для валидаций и прочих технических процессов, необходимо переписывать программу с учётом специальных флагов окружения в среде разработки .NET, как например флаг DEBUG:

```csharp
public static bool ValidateEmail(string email)
{
    #if DEBUG
    Console.WriteLine("Validating email: {0}", email);
    #endif

    return email.Contains('@');
}
```

Перейдём к методам, которые работают с данными, сторонними API и прочим подобным, начнём с базовых и самых простых, а именно с отправки сообщения на электронную почту. Мы создадим отдельный почтовый сервис-провайдер, который будет отвечать за отправку почтовых писем, мы также реализуем глобальный метод для отправки универсальных писем и будем отсылаться в методе "welcome-" писем на главный родительный метод отправки.

```csharp
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
```

Перейдём к финальным запросам, на получение пользовательских данных и прав доступа; мы интегрировали дебаг-принты при условиях окружения, исправили табуляцию, нейминги. Самое главное, что мы семантически добавили запросы для редактирования данных, чисто для имитации, но правильности общей структуры и логики кода. Но, самым большим изменением является метод регистрации пользователя, используя все выполненные до этого изменения и правила рефакторинга, данный метод невероятно уменьшился в размере и стал безопасным, т.е., подразумевать ошибки и правильную логику работы там, где раньше этого не было. Мы также поменяли логику регистрации юзера для будущего тестирования, т.е. функция возвращает успешность результата регистрации.

```csharp
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

```

Тестирование, по основам проектирования программного кода, CI/CD и просто инфраструктуре и организации разработки, должно быть всегда отдельно; для этого существуют локальные эко-системы, но в рамках этого задания мы сделаем тестирование частью программного кода. Но, мы будем использовать специальные методы тестирования и отладки, встроенные в инструментарий языка программирования C# и .NET окружения.

```csharp
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

```

Пришла пора исправить и переписать ядро программы, стартовую точку и прочие центральные процессы всей программной реализации. Мы также выведем все элементы отладки под специальное условие окружения, но также, сделаем процесс тестирования через эту самую отладку.

Мы переписали стартовую точку программы в рекуррентный таск-скедулер, тем самым программа номинально будет работать до ручной или ошибочной (из-за ошибки) точки остановки и сервер будет держаться теоретически бесконечно. Из-за этого, мы можем превратить все будущие методы программы в асинхронные через вызов тасков и, также убрать nested-code для вызова асинхронного кода в программе сетапа миддлвейра. В исходном коде наблюдалось чрезмерное количество вложенных условных операторов, что является примером антипаттерна arrow-code. Такой стиль нарушает читаемость, усложняет сопровождение и противоречит принципам чистого кода. В ходе рефакторинга метод был переработан: вложенные конструкции заменены на ранний выход (guard clauses) и вынесение логики в отдельные методы

```csharp
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
```

В процессе рефакторинга, помимо устранения антипаттернов типа god-object, arrow-Code, в коде неявно были реализованы и отдельные паттерны проектирования. Так, класс датабаз-реквестинга выполняет роль паттерна "фасад" (см. ФАСАД), скрывающего детали работы с базой. Класс датабаз-валидатор представляет собой реализацию паттерна "валидатор" (см. ВАЛИДАТОР), а вынесение логики проверки во множество отдельных методов близко к паттерну "стратегия" (см. СТРАТЕГИЯ). Вынесение сервис-провайдера почты в отдельный сервис соответствует концепции сервисных слоёв (service layers). Также в коде применены guard-clauses для повышения читаемости и управляемости логики. Таким образом, рефакторинг не только улучшил структуру, но и приблизил проект к общепринятым архитектурным практикам и паттернам.