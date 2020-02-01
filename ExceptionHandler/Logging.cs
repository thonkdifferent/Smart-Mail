using System;

namespace SmartMail.ExceptionHandler
{
    class Logging
    {
        public static void Log(string info, char severity)
        {
            switch (severity)
            {
                case 'i':
                    Info(info);
                    break;
                case 'w': Warning(info); break;
                case 'e': Error(info); break;
            }
        }
        private static void Error(string info)
        {
            Console.WriteLine($"[ERROR] {info}");
        }
        private static void Warning(string info)
        {
            Console.WriteLine($"[WARN] {info}");
        }
        private static void Info(string info)
        {
            Console.WriteLine($"[INFO] {info}");
        }
    }
}
