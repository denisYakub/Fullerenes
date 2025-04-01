using System.Diagnostics;

using PrintToConsoleDebug = System.Diagnostics.Debug;

namespace Fullerenes.Server.CustomLogger
{
    public static class Print
    {
        [Conditional("LoggerMode")]
        public static void PrintToConsole(string msg)
        {
            PrintToConsoleDebug.WriteLine(msg);
        }
        [Conditional("LoggerMode")]
        public static void PrintToFile(string filepath, string msg)
        {
            using StreamWriter sw = new(filepath, true);
            sw.WriteLine(msg);
        }
    }
}
