using System;
using System.Collections.Generic;
using System.Text;
using videoMaker.Domain.Models;

namespace videoMaker.Domain.Robots
{
    public static class Input
    {
        private static Content content = new Content();
        private static ConsoleKeyInfo keyPressed;
        private static readonly string[] options = { " Who is", "What is", "The history of" };
        private static int indexOption = 0;

        public static void Robot()
        {
            ReadInputs();
        }

        private static void ReadInputs()
        {
            content = new Content
            {
                SearchTerm = AskAndReturnSearchTearm(),
                Prefix = AskAndReturnPrefix()
            };

            Console.Clear();
            State.Save(content);

        }

        private static string AskAndReturnPrefix()
        {
            Console.WriteLine("Choose the preffix");

            do
            {
                Console.Clear();

                Console.WriteLine("1 -  Who is");
                Console.WriteLine("2 -  What is");
                Console.WriteLine("3 -  The history of");
                Console.WriteLine("4 -  CANCEL");

                keyPressed = Console.ReadKey();

            } while (NotPressedAvailableOption());

            if (keyPressed.Key == ConsoleKey.D4)
            {
                Console.Clear();
                ReadInputs();
            }

            int.TryParse(keyPressed.KeyChar.ToString(), out indexOption);
            return options[indexOption - 1];
        }

        private static bool NotPressedAvailableOption()
        {
            return keyPressed.Key != ConsoleKey.D1 &&
                        keyPressed.Key != ConsoleKey.D2 &&
                        keyPressed.Key != ConsoleKey.D3 &&
                        keyPressed.Key != ConsoleKey.D4;
        }

        private static string AskAndReturnSearchTearm()
        {
            Console.WriteLine("Type a Wikipedia search term:");
            var inputWord = Console.ReadLine();
            return inputWord;
        }
    }
}
