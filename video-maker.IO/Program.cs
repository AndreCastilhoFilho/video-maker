using System;

namespace video_maker.IO
{
    class Program
    {
        static void Main(string[] args)
        {
            ReadInputs();

        }

        private static void ReadInputs()
        {
            Console.WriteLine("Type a Wikipedia search term:");
            var inputWord = Console.ReadLine();

            Console.WriteLine("Choose the preffix");

            ConsoleKeyInfo keyPressed;

            string[] options = { " Who is", "What is", "The history of" };

            do
            {
                Console.Clear();

                Console.WriteLine("1 -  Who is");
                Console.WriteLine("2 -  What is");
                Console.WriteLine("3 -  The history of");
                Console.WriteLine("4 -  CANCEL");

                keyPressed = Console.ReadKey();

            } while (keyPressed.Key != ConsoleKey.D1 && keyPressed.Key != ConsoleKey.D2 && keyPressed.Key != ConsoleKey.D3 && keyPressed.Key != ConsoleKey.D4);

            if (keyPressed.Key == ConsoleKey.D4)
            {
                Console.Clear();
                ReadInputs();
            }
            else
            {
                Console.Clear();
                int.TryParse(keyPressed.KeyChar.ToString(), out var indexOption);
                Console.WriteLine($"You Choosed: { options[indexOption - 1] + " " + inputWord}");
            }

            Console.ReadKey(true);
        }
    }
}
