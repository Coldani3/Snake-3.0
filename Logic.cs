namespace snake_30
{
    public class Logic
    {
		private static readonly Logic Instance = new Logic();

		private Logic() {}
        public void Tick()
        {
            if (Program.Food.Count <= 0) 
			{
				this.GenerateFood();
			}
            Program.PlayerSnake.MoveForward();
            Program.DebugLog("End tick!");
        }

        public void GameOver()
        {
            //Program.TickRate = 0;
            Program.Running = false;
            //forgive me father for I have sinned
            System.Console.Clear();
            System.Console.SetCursorPosition(Program.WindowWidth / 2 - 5, Program.WindowHeight / 2);
            System.Console.ForegroundColor = System.ConsoleColor.White;
            System.Console.Write("Game over!");
            System.Console.ReadKey(true);
        }

        public void EatFoodAtLocation(int foodGameX, int foodGameY, SnakeController snakeEating)
        {
            foreach (Food food in Program.Food)
            {
                if (food.X == foodGameX && food.Y == foodGameY)
                {
                    Program.DebugLog("Growing!");
                    Program.Food.Remove(food);
                    snakeEating.Grow();
                    //make the snake move faster
                    Program.TickRate += 0.25f;
                    break;
                }
            }
        }

        public bool IsFoodAtLocation(int gameX, int gameY)
        {
            foreach (Food food in Program.Food)
            {
                if (food.X == gameX && food.Y == gameY) 
				{
					return true;
				}
            }

            return false;
        }

        public void GenerateFood()
        {
            Program.DebugLog("Generating food!");
            //generate between 2 and 3 foods
            int toGenerate = Program.RNG.Next(2, 4);
            for (int i = 0; i <= toGenerate; i++)
            {
                int randX = Program.RNG.Next(4, Program.WindowWidth - 4);
                int randY = Program.RNG.Next(4, Program.WindowHeight - 4);

                if (!Program.PlayerSnake.AnyPieceAtCoords(randX, randY) && !this.IsFoodAtLocation(randX, randY)) 
				{
                    Program.Food.Add(new Food(randX, randY));
				}
                //minus i so it effectively doesn't go down in this case
                else 
				{
					i--;
				}
            }
        }

		public static Logic GetInstance()
		{
			return Instance;
		}
    }
}