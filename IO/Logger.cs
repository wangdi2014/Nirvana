﻿using System;

namespace IO
{
    public static class Logger
    {
        // can be redirected to any logger
        public static Action<string> LogLine { get; set; }
        static Logger() => LogLine = Console.WriteLine;
        public static void Log(Exception e) => LogLine($"{e.GetType()}: {e.Message}\n{e.StackTrace}");
    }
}