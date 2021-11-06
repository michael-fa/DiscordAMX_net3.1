using System;
using System.Drawing;
using System.IO;
using Console = Colorful.Console;

namespace bot.Utilities
{
    public static class Log
    {
        public static void Print(string _msg, int loglevel, string _scriptName = "none")
        {
            bool isDebug = false;
            string built_message = null;
            if (_msg.Length < 1 || loglevel > 4 || loglevel < -1) return;
            switch (loglevel)
            {
                case 0: built_message = "[INFO] " + _msg; break;
                case 1: built_message = "[WARNING] " + _msg; break;
                case 2: built_message = "[ERROR] " + _msg; break;
                case 3:
#if DEBUG
                    isDebug = true;
                    built_message = "[DEBUG] " + _msg;
#endif
                    break;
                case 4: built_message = "[SCRIPT] <" + _scriptName + "> " + _msg; break;
            }
            if (loglevel == 3 && isDebug) Console.Write(built_message);
            else if (loglevel == 4) Console.WriteLine(built_message);
            //else Console.Write(built_message);


            try
            {
                if (built_message != null && built_message.Length > 0) File.AppendAllText("Logs/current.txt", built_message + "\n");
            }
            catch { }
        }

        public static void WriteLine(string _msg, Color col)
        {
            Console.WriteLine(_msg, col);
            if (_msg.Length > 0) File.AppendAllText("Logs/current.txt", _msg + "\n");
        }

        public static void WriteLine(string _msg)
        {
            Console.WriteLine(_msg);
            if (_msg.Length > 0) File.AppendAllText("Logs/current.txt", _msg + "\n");
        }

        public static void Write(string _msg, Color col)
        {
            Console.Write(_msg, col);
            if (_msg.Length > 0) File.AppendAllText("Logs/current.txt", _msg);
        }

        public static void Write(string _msg)
        {
            Console.Write(_msg);
            if (_msg.Length > 0) File.AppendAllText("Logs/current.txt", _msg);
        }

        public static void Print(Exception e)
        {
            Console.WriteLine("---------------------------------------\n[EXCEPTION] " + e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n---------------------------------------\n");
            File.AppendAllText("Logs/current.txt", "---------------------------------------\n[EXCEPTION] " + e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n-------------------------------------- -\n");
        }
    }
}