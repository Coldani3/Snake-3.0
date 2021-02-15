using System;
using System.Collections.Generic;

namespace snake_30
{
    public class Renderer
    {
        public Renderer()
        {

        }

        public void RenderAll()
        {
            if (Program.TickRate > 0)
            {
                Console.Clear();
                RenderFood(Program.Food);
                RenderSnake(Program.PlayerSnake);
                RenderScore();

                if (Program.Debug)
                {
                    RenderDebugLog();
                }
            }
            else
            {
                Console.SetCursorPosition(Program.WindowWidth / 2, Program.WindowHeight);
                Console.Write("Game over!");
            }
        }

        public void RenderSnake(Snake snake)
        {
            foreach (SnakeComponent piece in snake.Pieces)
            {
                this.Draw(piece.X, piece.Y, piece);
            }
        }

        public void RenderFood(List<Food> food)
        {
            foreach (Food foodItem in food)
            {
                this.Draw(foodItem.X, foodItem.Y, foodItem);
            }
        }

        //honestly I probably should have considered multi character components when designing the interfaces but it's not a
        //big deal as this is the one exception to the structure
        public void RenderScore()
        {
            Console.SetCursorPosition(Console.WindowWidth - 5, 0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(Program.PlayerSnake.Pieces.Count - 1);
        }

        public void RenderDebugLog()
        {
            for (int i = 0; i < Program.DebugLogStack.Count; i++)
            {
                Console.SetCursorPosition(0, Console.WindowHeight - Program.DebugLogStack.Count + i);
                Console.Write(Program.DebugLogStack[i]);
            }
        }

        public void Draw(int gameX, int gameY, IDrawable drawable)
        {
            Console.SetCursorPosition(gameX, GameYToScreenY(gameY));

            if (Console.ForegroundColor != drawable.DrawColour) Console.ForegroundColor = drawable.DrawColour;
            
            Console.Write(drawable.DrawCharacter);
        }

        public int GameYToScreenY(int gameY)
        {
            return Console.WindowHeight - gameY;
        }
    }
}