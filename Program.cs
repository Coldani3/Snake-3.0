using System;
using System.Threading;
using System.Threading.Tasks;

namespace snake_30
{
    class Program
    {
        public static bool Running = true;
        public static Renderer Renderer = new Renderer();
        public static Logic Logic = new Logic();
        public static int TickRate = 10;
        
        static void Main(string[] args)
        {
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
            logicThread.Start();
            renderThread.Start();
            inputThread.Start();
            //end program
            Console.ReadKey(true);
        }

        static void HandleKeyInput(ConsoleKeyInfo info)
        {
            switch (info.Key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    break;
                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    break;
            }
        }
    }
}
