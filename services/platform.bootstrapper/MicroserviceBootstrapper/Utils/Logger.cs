
namespace MicroserviceBootstrapper.Utils
{
    public class Logger
    {
        private readonly string _componentName;

        public Logger(string componentName = null)
        {
            _componentName = componentName;
        }

        // Define console colors
        private const ConsoleColor InfoColor = ConsoleColor.Green;
        private const ConsoleColor WarnColor = ConsoleColor.Yellow;
        private const ConsoleColor ErrorColor = ConsoleColor.Red;
        private const ConsoleColor DebugColor = ConsoleColor.Blue;
        private const ConsoleColor SuccessColor = ConsoleColor.Cyan;
        private const ConsoleColor ResetColor = ConsoleColor.White;

        public void Info(string message)
        {
            WriteColored(message, InfoColor, "INFO");
        }

        public void Warn(string message)
        {
            WriteColored(message, WarnColor, "WARN");
        }

        public void Error(string message)
        {
            WriteColored(message, ErrorColor, "ERROR");
        }

        public void Error(string message, Exception exception)
        {
            var originalColor = Console.ForegroundColor;

            Console.ForegroundColor = ErrorColor;
            Console.Write($"[{DateTime.Now:HH:mm:ss}] ");
            if (!string.IsNullOrEmpty(_componentName))
                Console.Write($"[{_componentName}] ");
            Console.Write("[ERROR] ");

            Console.ForegroundColor = ResetColor;
            Console.WriteLine($"{message}");
            Console.ForegroundColor = ErrorColor;
            Console.WriteLine($"Exception: {exception.GetType().Name}");
            Console.ForegroundColor = ResetColor;
            Console.WriteLine($"Message: {exception.Message}");
            if (exception.StackTrace != null)
            {
                Console.ForegroundColor = ErrorColor;
                Console.WriteLine("Stack Trace:");
                Console.ForegroundColor = ResetColor;
                Console.WriteLine(exception.StackTrace);
            }

            Console.ForegroundColor = originalColor;
        }

        public void Debug(string message)
        {
            WriteColored(message, DebugColor, "DEBUG");
        }

        public void Success(string message)
        {
            WriteColored(message, SuccessColor, "SUCCESS");
        }

        private void WriteColored(string message, ConsoleColor color, string level)
        {
            // Save original color
            var originalColor = Console.ForegroundColor;

            // Write timestamp and level with color
            Console.ForegroundColor = color;
            Console.Write($"[{DateTime.Now:HH:mm:ss}] ");
            if (!string.IsNullOrEmpty(_componentName))
                Console.Write($"[{_componentName}] ");
            Console.Write($"[{level}] ");

            // Write message and reset color
            Console.ForegroundColor = ResetColor;
            Console.WriteLine(message);

            // Restore original color
            Console.ForegroundColor = originalColor;
        }

        // Optional: Progress indicator methods
        public void StartOperation(string operationName)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"[{DateTime.Now:HH:mm:ss}] ");
            Console.WriteLine($"Starting: {operationName}...");
            Console.ForegroundColor = originalColor;
        }

        public void EndOperation(string operationName, bool success = true)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = success ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write($"[{DateTime.Now:HH:mm:ss}] ");
            Console.WriteLine($"Completed: {operationName} - {(success ? "✓" : "✗")}");
            Console.ForegroundColor = originalColor;
        }
    }
}