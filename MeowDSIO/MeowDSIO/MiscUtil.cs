using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO
{
    public static class MiscUtil
    {
        //Yes that's a GameBoy Color reference
        public static void PrintDX(string txt, ConsoleColor? foreColor = null, ConsoleColor? backColor = null)
        {
            ConsoleColor? oldForeColor = null, oldBackColor = null;

            if (foreColor.HasValue)
            {
                oldForeColor = Console.ForegroundColor;
                Console.ForegroundColor = foreColor.Value;
            }
                
            if (backColor.HasValue)
            {
                oldBackColor = Console.BackgroundColor;
                Console.BackgroundColor = backColor.Value;
            }

            Console.Write(txt);

            if (foreColor.HasValue)
                Console.ForegroundColor = oldForeColor.Value;
                
            if (backColor.HasValue)
                Console.BackgroundColor = oldBackColor.Value;
        }

        public static void PrintlnDX(string txt, ConsoleColor? foreColor = null, ConsoleColor? backColor = null)
        {
            PrintDX(txt + Environment.NewLine, foreColor, backColor);
        }

        public static bool ConsolePrompYesNo(string question = null)
        {
            if (question != null)
            {
                PrintlnDX($"{question} (y/n)", ConsoleColor.Yellow);
            }

            bool? result = null;
            do
            {
                var k = Console.ReadKey(true).KeyChar;
                if ("Yy".Contains(k))
                {
                    result = true;
                }
                else if ("Nn".Contains(k))
                {
                    result = false;
                }
            }
            while (result == null);

            return result.Value;
        }
    }
}
