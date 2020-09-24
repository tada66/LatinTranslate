using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Pastel;

namespace LatinaTranslate
{
    class Program
    {
        static void Main(string[] args)
        {
            if(!File.Exists("resources/Femina.txt"))
            {
                ErrorLogger("Fatal err - resources folder not found");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Fatal Error - resources not found");
                Console.ForegroundColor = ConsoleColor.Yellow;      
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                Environment.Exit(0);
            }
            Console.Write("Word: ");
            string word = Console.ReadLine();
            //Logger("word is " + word, false, filename);
            word = InputCleanup(word);
            Console.WriteLine("0 - unknown(smart search)\n1 - test and list all\n\n---MANUAL OVERRIDE---\n2 - femina(1. dek)\n3 - servus(2. dek)\n4 - verbum(2. dek)\n5 - miles(3. dek)\n6 - carmen(3. dek)\n7 - mare(3. dek)\n8 - exercitus(4. dek)\n9 - cornu(4. dek)\n10 - res(5. dek)");
            Console.Write("If known: ");
            string ReadLineDek = Console.ReadLine();
            bool successfulparse = Int32.TryParse(ReadLineDek, out int genderchoice);
            Console.ForegroundColor = ConsoleColor.Red;
            if (!successfulparse)
                Console.WriteLine("Attempted conversion of '{0}' failed.\nUsing defualt values", ReadLineDek ?? "<null>");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("INFO: " + word);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(DetectionSubs(word, genderchoice));
            Console.ResetColor();
            Console.ReadKey();
        }

        public static string DetectionSubs(string input, int gender)
        {
            switch (gender)
            {
                case 0:
                    return SmartSearch(input);
                case 1:
                    TryEverything(input);
                    return "";
                case 2:
                    return DetectionSubsF(input).result;
                case 3:
                    return DetectionSubsS(input).result;
                case 4:
                    return DetectionSubsV(input).result;
                case 5:
                    return DetectionSubsM(input).result;
                case 6:
                    return DetectionSubsC(input).result;
                case 7:
                    return DetectionSubsMa(input).result;
                case 8:
                    return DetectionSubsE(input).result;
                case 9:
                    return DetectionSubsCo(input).result;
                case 10:
                    return DetectionSubsR(input).result;
            }
            ErrorLogger(DateTime.Now.Millisecond.ToString() + "Fucky wucky at DetectionSubs (number too big?)");
            return "err Unknown word";
        }

        public static string SmartSearch(string input)
        {
            var result = DetectionSubsF(input);
            string bestmatch = "";
            if (result.probability == 0)
                return (result.result);
            else if (result.probability == 1)
                bestmatch = result.result;
            result = DetectionSubsS(input);
            if (result.probability == 0)
                return (result.result);
            else if (result.probability == 1)
                bestmatch = result.result;
            result = DetectionSubsV(input);
            if (result.probability == 0)
                return (result.result);
            else if (result.probability == 1)
                bestmatch = result.result;      //servus and verbum gets fucky wucky 
            return bestmatch;
        }

        public static void TryEverything(string input)
        {
            string result = "";
            Console.ForegroundColor = ConsoleColor.Magenta;
            result = result + DetectionSubsF(input).result + "\n";
            if(DetectionSubsF(input).probability == 0)
                Console.WriteLine(DetectionSubsF(input).result);
            Console.ForegroundColor = ConsoleColor.Blue;
            result = result + DetectionSubsS(input).result + "\n";
            if (DetectionSubsS(input).probability == 0)
                Console.WriteLine(DetectionSubsS(input).result);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            result = result + DetectionSubsV(input).result + "\n";
            if (DetectionSubsV(input).probability == 0)
                Console.WriteLine(DetectionSubsV(input).result);
            result = result + DetectionSubsV(input).result + "\n";
            if (DetectionSubsV(input).probability == 0)
                Console.WriteLine(DetectionSubsM(input).result);
            Console.ResetColor();
        }

        public static (string result, int probability) DetectionSubsR(string input)
        {
            string bestmatch = "";
            using (StreamReader sr = new StreamReader("resources/Res.txt"))
            {
                for (int i = 0; i <= 11; i++)
                {
                    string linecheck = sr.ReadLine();
                    if (!string.IsNullOrEmpty(linecheck) && input.Length >= linecheck.Length)
                    {
                        string shortened = input.Substring(input.Length - linecheck.Length);
                        if (shortened == linecheck)
                            bestmatch = bestmatch + "\n" + "Podle Res (2. dek) " + NumToLat(i) + " -" + linecheck;
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
                return ("no match with res found", 1);
        }

        public static (string result, int probability) DetectionSubsCo(string input)
        {
            string bestmatch = "";
            using (StreamReader sr = new StreamReader("resources/Cornu.txt"))
            {
                for (int i = 0; i <= 11; i++)
                {
                    string linecheck = sr.ReadLine();
                    if (!string.IsNullOrEmpty(linecheck) && input.Length >= linecheck.Length)
                    {
                        string shortened = input.Substring(input.Length - linecheck.Length);
                        if (shortened == linecheck)
                            bestmatch = bestmatch + "\n" + "Podle Cornu (2. dek) " + NumToLat(i) + " -" + linecheck;
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
                return ("no match with cornu found", 1);
        }

        public static (string result, int probability) DetectionSubsE(string input)
        {
            string bestmatch = "";
            using (StreamReader sr = new StreamReader("resources/Exercitus.txt"))
            {
                for (int i = 0; i <= 11; i++)
                {
                    string linecheck = sr.ReadLine();
                    if (!string.IsNullOrEmpty(linecheck) && input.Length >= linecheck.Length)
                    {
                        string shortened = input.Substring(input.Length - linecheck.Length);
                        if (shortened == linecheck)
                            bestmatch = bestmatch + "\n" + "Podle Exercius (2. dek) " + NumToLat(i) + " -" + linecheck;
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
                return ("no match with exercitus found", 1);
        }

        public static (string result, int probability) DetectionSubsMa(string input)
        {
            string bestmatch = "";
            using (StreamReader sr = new StreamReader("resources/Mare.txt"))
            {
                for (int i = 0; i <= 11; i++)
                {
                    string linecheck = sr.ReadLine();
                    if (!string.IsNullOrEmpty(linecheck) && input.Length >= linecheck.Length)
                    {
                        string shortened = input.Substring(input.Length - linecheck.Length);
                        if (shortened == linecheck)
                            bestmatch = bestmatch + "\n" + "Podle mare (2. dek) " + NumToLat(i) + " -" + linecheck;
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
                return ("no match with mare found", 1);
        }

        public static (string result, int probability) DetectionSubsS(string input)
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

        public static (string result, int probability) DetectionSubsC(string input)
        {
            string bestmatch = "";
            using (StreamReader sr = new StreamReader("resources/Carmen.txt"))
            {
                for (int i = 0; i <= 11; i++)
                {
                    string linecheck = sr.ReadLine();
                    if (!string.IsNullOrEmpty(linecheck) && input.Length >= linecheck.Length)
                    {
                        string shortened = input.Substring(input.Length - linecheck.Length);
                        if (shortened == linecheck)
                            bestmatch = bestmatch + "\n" + "Podle Carmen (2. dek) " + NumToLat(i) + " -" + linecheck;
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
                return ("no match with carmen found", 1);
        }

        public static (string result, int probability) DetectionSubsM(string input)
        {
            string bestmatch = "";
            using (StreamReader sr = new StreamReader("resources/Miles.txt"))
            {
                for (int i = 0; i <= 11; i++)
                {
                    string linecheck = sr.ReadLine();
                    if (!string.IsNullOrEmpty(linecheck) && input.Length >= linecheck.Length)
                    {
                        string shortened = input.Substring(input.Length - linecheck.Length);
                        if (shortened == linecheck)
                            bestmatch = bestmatch + "\n" + "Podle Miles (2. dek) " + NumToLat(i) + " -" + linecheck;
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
                return ("no match with miles found", 1);
        }

        public static (string result, int probability) DetectionSubsV(string input)
        {
            string bestmatch = "";
            using (StreamReader sr = new StreamReader("resources/Verbum.txt"))
            {
                for (int i = 0; i <= 11; i++)
                {
                    string linecheck = sr.ReadLine();
                    if (!string.IsNullOrEmpty(linecheck) && input.Length >= linecheck.Length)
                    {
                        string shortened = input.Substring(input.Length - linecheck.Length);
                        if (shortened == linecheck)
                            bestmatch = bestmatch + "\n" + "Podle Verbum (2. dek) " + NumToLat(i) + " -" + linecheck;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Critical Err - Input is too short");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Input length: " + input.Length.ToString());
                        Console.WriteLine("Check length: " + linecheck.Length.ToString());
                        Console.ResetColor();
                        ErrorLogger("Critical Err - detectionSubsV - input string length " + input.Length + " is too short, should be at least " + linecheck.Length);
                    }
                }
            }
            if (!string.IsNullOrEmpty(bestmatch))
                return ("match found: " + bestmatch, 0);
            else
                return ("no match with verbum found", 1);
        }

        public static (string result, int probability) DetectionSubsF(string input)
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
                    return "Nominativ. sg.";
                case 1:
                    return "Genitiv. sg.";
                case 2:
                    return "Dativ. sg.";
                case 3:
                    return "Akuzativ. sg.";
                case 4:
                    return "Vokativ. sg.";
                case 5:
                    return "Ablativ. sg.";
                case 6:
                    return "Nominativ. pl.";
                case 7:
                    return "Genitiv. pl.";
                case 8:
                    return "Dativ. pl.";
                case 9:
                    return "Akuzativ. pl.";
                case 10:
                    return "Vokativ. pl.";
                case 11:
                    return "Ablativ. pl.";
            }
            ErrorLogger("NumToLat Failed - err Number is out of range");
            return "err Number is out of range";
        }

        public static string InputCleanup(string input)
        {
            if(input.Length > 4)
                input = input.Substring(input.Length - 4);
            input = input.ToLower();
            return input;
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