using System.Text;

namespace WowInventoryStats.Logger
{
    public enum LogType { Trace, Success, Warn, Failure, Error, Debug }
    
    public class ConsoleLogger
    {
        public ConsoleLogger(string? filePath = null)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                logFile = new StreamWriter(filePath);
            }
        }

        public void Log(LogType type, string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            var time = DateTime.Now.ToString("{ dd MMM yyyy H:mm:ss}");
            Console.Write(time + " ");
            
            (string level, ConsoleColor color) = LogTypeToColor[type];
            Console.ForegroundColor = color;
            Console.Write($"[{level}]: ");
            
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);

            if (logFile != null)
            {
                var fileLine = new StringBuilder(time + " ");
                fileLine.Append($"[{level}]: ");
                fileLine.Append(message);
                logFile.WriteLine(fileLine);
            }
        }

        private readonly StreamWriter? logFile;

        private static readonly Dictionary<LogType, Tuple<string, ConsoleColor>> LogTypeToColor = new()
        {
            { LogType.Trace, Tuple.Create("TRACE", ConsoleColor.White) },
            { LogType.Success, Tuple.Create("SUCCESS", ConsoleColor.Green) },
            { LogType.Warn, Tuple.Create("WARNING", ConsoleColor.Yellow) },
            { LogType.Failure, Tuple.Create("FAILURE", ConsoleColor.DarkRed) },
            { LogType.Error, Tuple.Create("ERROR", ConsoleColor.Red) },
            { LogType.Debug, Tuple.Create("DEBUG", ConsoleColor.Blue) },
        };
    }
}
