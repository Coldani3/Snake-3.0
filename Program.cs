using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace snake_30
{
    class Program
    {
        public static bool Running = true;
        public static int TickRate = 3;
        public static readonly int WindowHeight = 40;
        public static readonly int WindowWidth = 70;
        //Singleton with the RNG so I don't need to create multiple Random objects over and over throughout the program
        public static readonly Random RNG = new Random();
        public static Renderer Renderer = new Renderer();
        public static Logic Logic = new Logic();
        public static List<Food> Food = new List<Food>();
        //How many times a second the snake moves forwards.
        
        public static readonly Snake PlayerSnake = GenerateSnake();
        
        static void Main(string[] args)
        {
            Console.SetWindowSize(WindowWidth, WindowHeight);
            Console.CursorVisible = false;
            //Logic thread
            Task logicThread = new Task(() => { 
                while (Running)
                {
                    Logic.Tick();
                    Thread.Sleep(1000 / TickRate);
                }
            });
            //Logic thread
            Task renderThread = new Task(() => { 
                while (Running)
                {
                    Renderer.RenderAll();
                    Thread.Sleep(1000 / TickRate);
                }
            });
            //Input thread setup
            Task inputThread = new Task(() => {while (Running) HandleKeyInput(Console.ReadKey(true));});
            // Start threads
            logicThread.Start();
            renderThread.Start();
            inputThread.Start();

            while (Running);
            //end program
            Console.ReadKey(true);
        }

        static void HandleKeyInput(ConsoleKeyInfo info)
        {
            switch (info.Key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    PlayerSnake.ChangeDirection(Direction.Up);
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    PlayerSnake.ChangeDirection(Direction.Down);
                    break;
                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    PlayerSnake.ChangeDirection(Direction.Left);
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    PlayerSnake.ChangeDirection(Direction.Right);
                    break;
            }
        }

        static Snake GenerateSnake()
        {
            //make them between 1 third and three thirds
            //'1 +' is because RNG.Next's max value is exclusive rather than inclusive, and we want to
            //include the upper third
            int x = RNG.Next(Console.WindowWidth / 3, 1 + (Console.WindowWidth / 3) * 2);
            int y = RNG.Next(Console.WindowHeight / 3, 1 + (Console.WindowHeight / 3) * 2);
            return new Snake(x, y);
        }
    }
}
