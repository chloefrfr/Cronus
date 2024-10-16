using Serilog;

namespace Larry.Source.Utilities
{
    public class Logger
    {
        static Logger()
        {
            Log.Logger = new LoggerConfiguration()
                            
                            .WriteTo.Console()
                            .CreateLogger();
        }

        public static void Information(string message)
        {
            Log.Information(message);
        }

        public static void Warning(string message)
        {
            Log.Warning(message);
        }

        public static void Error(string message, Exception ex = null)
        {
            Log.Error(ex, message);
        }

        public static void Debug(string message)
        {
            Log.Debug(message);
        }

        public static void Fatal(string message)
        {
            Log.Fatal(message);
        }

        public static void Close()
        {
            Log.CloseAndFlush();
        }
    }
}
