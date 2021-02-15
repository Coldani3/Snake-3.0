using System.Collections.Generic;

namespace snake_30
{
    public class Logic
    {
        public void Tick()
        {
            Program.PlayerSnake.MoveForward();
        }

        public void GameOver()
        {
            
        }

        public void EatFoodAtLocation(int foodGameX, int foodGameY, Snake snakeEating)
        {
            foreach (Food food in Program.Food)
            {
                if (food.X == foodGameX && food.Y == foodGameY)
                {
                    Program.Food.Remove(food);
                    snakeEating.Grow();
                    break;
                }
            }
        }

        public bool IsFoodAtLocation(int gameX, int gameY)
        {
            foreach (Food food in Program.Food)
            {
                if (food.X == gameX && food.Y == gameY) return true;
            }

            return false;
        }
    }
}