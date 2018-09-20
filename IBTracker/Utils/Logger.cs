using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

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

        private const int ProgressLength = 6;

        private static readonly Stopwatch watch = new Stopwatch();
        private static Level logLevel;

        private static int? progressTotal = null;
        private static double progressScale;

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

        public static void Info(string message, bool crlf = false)
        {
            EndProgress();
            if (logLevel < Level.Info) return;
            if (crlf) Console.WriteLine();
            Console.WriteLine($"{Elapsed} I: {message}");
        }

        public static void Debug(string message)
        {
            EndProgress();
            if (logLevel < Level.Debug) return;
            Console.WriteLine($"{Elapsed} D: {message}");
        }

        public static void BeginProgress(int total) 
        {
            EndProgress();
            if (progressTotal != null || total == 0) return;
            progressScale = (Console.WindowWidth - ProgressLength) / total;
            progressTotal = Convert.ToInt16(total * progressScale);
            Console.CursorVisible = false;
        }

        public static void EndProgress() 
        {
            if (progressTotal == null) return;
            Console.WriteLine();
            progressTotal = null;
            Console.CursorVisible = true;
        }

        public static void SetProgress(int pos) 
        {
            if (progressTotal == null) return;
            pos = Convert.ToInt16(pos * progressScale);

            var builder = new StringBuilder();
            builder.Append("[");
            builder.Append(new string('.', pos));
            builder.Append(new string(' ', progressTotal.Value - pos));
            builder.Append("]");

            var bar = builder.ToString();
            builder.Clear();
            builder.Append(bar.Substring(0, bar.Length / 2));
            builder.Append(string.Format(" {0,3:###}% ", pos * 100 / progressTotal.Value));
            builder.Append(bar.Substring(bar.Length / 2));

            Console.CursorLeft = 0;
            Console.Write(builder.ToString());
        }        
    }
}
