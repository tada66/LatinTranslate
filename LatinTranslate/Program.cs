using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Pastel;
using System.Drawing;

namespace LatinaTranslate
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Word: ");
            string word = Console.ReadLine();
            if (word.Length >= 4)
                word = word.Substring(word.Length - 4); //exctract last 4 letters
            Console.WriteLine("0 - unknown(search all)\n1 - femina(1. dek)\n2 - servus(2. dek)\n3 - exemplum(2. dek)\n4 - miles(3. dek)\n5 - maria(3. dek)\n6 - exercitus(4. dek)\n7 - cornu(4. dek)\n8 - res(5. dek)\n9 - test all");
            Console.Write("If known: ");
            int genderchoice;
            string ReadLineDek = Console.ReadLine();
            bool successfulparse = Int32.TryParse(ReadLineDek, out genderchoice);
            Console.ForegroundColor = ConsoleColor.Red;
            if (!successfulparse)
                Console.WriteLine("Attempted conversion of '{0}' failed.\nUsing defualt values", ReadLineDek ?? "<null>");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("INFO: " + word);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(detectionSubs(word, genderchoice));
            Console.ResetColor();
            Console.ReadKey();
        }

        public static string detectionSubs(string input, int gender)
        {
            if (gender != 0 && gender != 9)
            {
                switch (gender)
                {
                    case 1:
                        return detectionSubsF(input).result;
                    case 2:
                        return detectionSubsS(input).result;
                }
            }
            else
            {
                var result = detectionSubsF(input);
                string bestmatch = "";
                if (result.probability == 0)
                {
                    return (result.result);
                }
                else if (result.probability == 1)
                {
                    bestmatch = result.result;
                }
                result = detectionSubsS(input);
                if (result.probability == 0)
                {
                    return (result.result);
                }
                else if (result.probability == 1)
                {
                    bestmatch = result.result;
                }
                return bestmatch;

            }
            return "err Unknown word";
        }

        public static (string result, int probability) detectionSubsS(string input)
        {
            string bestmatch = "";
            using (StreamReader sr = new StreamReader("resources/Servus.txt"))
            {
                for (int i = 0; i <= 11; i++)
                {
                    string linecheck = sr.ReadLine();
                    if (!string.IsNullOrEmpty(linecheck) && input.Length >= linecheck.Length)
                    {
                        string shortened = input.Substring(input.Length - linecheck.Length);
                        if (shortened == linecheck)
                            bestmatch = bestmatch + "\n" + "Podle Servus (2. dek) " + NumToLat(i) + " -" + linecheck;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Critical Err - Input is too short");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Input length: " + input.Length.ToString());
                        Console.WriteLine("Check length: " + linecheck.Length.ToString());
                        Console.ResetColor();
                        ErrorLogger("Critical Err - detectionSubsM - input string length " + input.Length + " is too short, should be at least " + linecheck.Length);
                    }
                }
            }
            if (!string.IsNullOrEmpty(bestmatch))
                return ("match found: " + bestmatch, 0);
            else
                return ("no match with servus found", 1);
        }

        public static (string result, int probability) detectionSubsF(string input)
        {   //feminine gender detection
            string bestmatch = "";
            using (StreamReader sr = new StreamReader("resources/Femina.txt"))
            {
                for (int i = 0; i <= 11; i++)
                {
                    string linecheck = sr.ReadLine();
                    if (!string.IsNullOrEmpty(linecheck) && input.Length >= linecheck.Length)
                    {
                        string shortened = input.Substring(input.Length - linecheck.Length);
                        if (shortened == linecheck)
                            bestmatch = bestmatch + "\n" + "Podle Femina (1. dek) " + NumToLat(i) + " -" + linecheck;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Critical Err - Input is too short");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Input length: " + input.Length.ToString());
                        Console.WriteLine("Check length: " + linecheck.Length.ToString());
                        Console.ResetColor();
                        ErrorLogger("Critical Err - detectionSubsF - input string length " + input.Length + " is too short, should be at least " + linecheck.Length);
                    }
                }
            }
            if (!string.IsNullOrEmpty(bestmatch))
                return ("exact match found: " + bestmatch, 0);
            else
                return ("no match with femina found", 1);
        }

        public static string NumToLat(int num)
        {
            switch (num)    //gives back the latin name 
            {
                case 0:
                    return "nom. sg.";
                case 1:
                    return "gen. sg.";
                case 2:
                    return "dat. sg.";
                case 3:
                    return "ak. sg.";
                case 4:
                    return "vok. sg.";
                case 5:
                    return "abl. sg.";
                case 6:
                    return "nom. pl.";
                case 7:
                    return "gen. pl.";
                case 8:
                    return "dat. pl.";
                case 9:
                    return "ak. pl.";
                case 10:
                    return "vok. pl.";
                case 11:
                    return "abl. pl.";
            }
            ErrorLogger("NumToLat Failed - err Number is out of range");
            return "err Number is out of range";

        }

        public static void Logger(string lines)
        {
            using (StreamWriter file = new StreamWriter("applicationlog.txt", true))
            {
                file.WriteLine(lines);
            }
        }

        public static void ErrorLogger(string lines)
        {
            Directory.CreateDirectory("ErrorLogs");
            string filename = "ErrorLogs/ErrorLog-" + DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year + "_" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + ".txt"; //im sorry
            if (!File.Exists(filename))
            {
                using (StreamWriter file = new StreamWriter(filename))
                {
                    file.WriteLine(lines);
                }
            }
            else
            {
                using (StreamWriter file = new StreamWriter(filename, true))
                {
                    file.WriteLine(lines);
                }
            }
        }
    }
}