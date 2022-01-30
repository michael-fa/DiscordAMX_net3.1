using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace dcamx.Utils
{
    public static class Log
    {
        public static void WriteLine(string _msg)
        {
            Console.WriteLine(_msg);
            if (_msg.Length > 0) File.AppendAllText("Logs/current.txt", _msg + "\n");
        }
        public static void Info(string _msg, dcamx.Scripting.Script _source = null)
        {
            if (_source != null) Console.WriteLine("[INFO] [" + _source._amxFile + "] " + _msg);
            else Console.WriteLine("[INFO] " + _msg);

            if (_msg.Length > 0) File.AppendAllText("Logs/current.txt", _msg + "\n");
        }

        public static void Error(string _msg, dcamx.Scripting.Script _source = null)
        {
            if (_source != null) Console.WriteLine("[ERROR] [" + _source._amxFile + "] " + _msg);
            else Console.WriteLine("[ERROR] " + _msg);
            if (_msg.Length > 0) File.AppendAllText("Logs/current.txt", _msg + "\n");
        }

        public static void Warning(string _msg, dcamx.Scripting.Script _source = null)
        {
            if (_source != null) Console.WriteLine("[WARNING] [" + _source._amxFile + "] " + _msg);
            else Console.WriteLine("[WARNING] " + _msg);
            if (_msg.Length > 0) File.AppendAllText("Logs/current.txt", _msg + "\n");
        }


        public static void Debug(string _msg, dcamx.Scripting.Script _source = null)
        {
#if DEBUG
            if (_source != null) Console.WriteLine("[DEBUG] [" + _source._amxFile + "] " + _msg);
            else Console.WriteLine("[DEBUG] " + _msg);
            if (_msg.Length > 0) File.AppendAllText("Logs/current.txt", _msg + "\n");
#endif
        }


        public static void Exception(Exception e)
        {
            Console.WriteLine("---------------------------------------\n[EXCEPTION] " + e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n---------------------------------------\n");
            File.AppendAllText("Logs/current.txt", "---------------------------------------\n[EXCEPTION] " + e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n-------------------------------------- -\n");
        }

        public static void Exception(Exception e, dcamx.Scripting.Script _source = null)
        {
            if (_source == null)
            {
                Console.WriteLine("---------------------------------------\n[EXCEPTION] " + e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n---------------------------------------\n");
                File.AppendAllText("Logs/current.txt", "---------------------------------------\n[EXCEPTION] [" + _source._amxFile + "] " + e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n-------------------------------------- -\n");

            }
            else
            {
                Console.WriteLine("---------------------------------------\n[EXCEPTION] [" + _source._amxFile + "] " + e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n---------------------------------------\n");
                File.AppendAllText("Logs/current.txt", "---------------------------------------\n[EXCEPTION] [" + _source._amxFile + "] " + e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n-------------------------------------- -\n");

            }
        }
    }
}