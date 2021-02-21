using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace snake_30
{
    class Program
    {
        public static bool Running = true;
        public static bool Debug = false;
        public static float TickRate = 3;
        public static readonly int WindowHeight = 40;
        public static readonly int WindowWidth = 70;
        //Singleton with the RNG so I don't need to create multiple Random objects over and over throughout the program
        /*
        Doing this without a Singleton would mean repeated creation of Random objects in each of Logic and Renderer, which, while not
        particularly impactful of the performance of the program, would be repeating the same code more than once and lead to confusion.
        */
        public static readonly Random RNG = new Random();
        public static Renderer Renderer = new Renderer();
        public static Logic Logic = new Logic();
        public static List<Food> Food = new List<Food>();
        public static List<string> DebugLogStack = new List<string>();
        //How many times a second the snake moves forwards.
        
        public static readonly Snake PlayerSnake = GenerateSnake();
        
        static void Main(string[] args)
        {
            // IMPORTANT ASSIGNMENT NOTES:
            /*
            To my knowledge I have not used any external code sources, aside from perhaps referencing previous works of mine (which may
            have used external code sources). Only possible external source I might have used solely as a reminder is the following link 
            for the Tasks, but I did not copy any of the code from it.
            https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task?view=net-5.0
            At this point I've more or less memorised the basic usage of Task though.
            */
            Console.SetWindowSize(WindowWidth, WindowHeight);
            Console.CursorVisible = false;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            //Logic thread
            Task logicThread = new Task(() => { 
                while (Running)
                {
                    Logic.Tick();
                    Thread.Sleep((int) Math.Floor(1000 / TickRate));
                }
            });
            //Rendering thread
            Task renderThread = new Task(() => { 
                Console.Clear();
                while (Running)
                {
                    Renderer.RenderAll();
                    Thread.Sleep((int) Math.Floor(1000 / TickRate));
                }
            });
            //Input thread setup
            Task inputThread = new Task(() => {while (Running) HandleKeyInput(Console.ReadKey(true));});
            // Start threads
            logicThread.Start();
            renderThread.Start();
            inputThread.Start();

            while (Running);
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("End game!");
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
            int x = RNG.Next(Program.WindowWidth / 3, 1 + (Program.WindowWidth / 3) * 2);
            int y = RNG.Next(Program.WindowHeight / 3, 1 + (Program.WindowHeight / 3) * 2);
            return new Snake(x, y);
        }

        public static void DebugLog(string message)
        {
            if (DebugLogStack.Count > 8)
            {
                DebugLogStack.RemoveAt(0);
            }

            DebugLogStack.Add($"[{System.DateTime.Now.ToLocalTime()}] " + message);
        }
    }
}
