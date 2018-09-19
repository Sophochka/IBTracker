using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace IBTracker.Utils
{
    public static class Logger
    {
        public enum Level
        {
            None, 
            Info,
            Debug,
        }

        private static readonly Stopwatch watch = new Stopwatch();
        private static Level logLevel;

        private static string Elapsed
        {
            get
            {
                return string.Format("{0:00}:{1:00}:{2:000}", watch.Elapsed.Minutes, watch.Elapsed.Seconds, watch.Elapsed.Milliseconds);
            }
        }

        public static void Init(Level level)
        {
            Console.Clear();
            logLevel = level;
            watch.Start();
        }

        public static void Info(string message)
        {
            if (logLevel < Level.Info) return;
            Console.WriteLine($"{Elapsed} I: {message}");
        }

        public static void Debug(string message)
        {
            if (logLevel < Level.Debug) return;
            Console.WriteLine($"{Elapsed} D: {message}");
        }
    }
}
