using System;
using System.Collections.Generic;

namespace snake_30
{
    public class Renderer
    {
        private List<int> LinesDrawnTo = new List<int>();
        public Renderer()
        {

        }

        public void RenderAll()
        {
            if (Program.TickRate > 0)
            {
                //Console.Clear();
                ClearScreen();
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
            this.LinesDrawnTo.Add(0);
        }

        public void RenderDebugLog()
        {
            for (int i = 0; i < Program.DebugLogStack.Count; i++)
            {
                int drawY = Console.WindowHeight - Program.DebugLogStack.Count + i;
                Console.SetCursorPosition(0, drawY);
                Console.Write(Program.DebugLogStack[i]);
                this.LinesDrawnTo.Add(drawY);
            }
        }

        public void ClearScreen()
        {
            //NOTE: if it's not clearing properly, check the line you added to the list was the SCREEN Y and not the game y
            foreach (int toClearLine in this.LinesDrawnTo)
            {
                Console.SetCursorPosition(0, toClearLine);
                Console.Write(new String(' ', Program.WindowWidth));
            }
        }

        public void Draw(int gameX, int gameY, IDrawable drawable)
        {
            int drawY = GameYToScreenY(gameY);
            Console.SetCursorPosition(gameX, drawY);

            if (Console.ForegroundColor != drawable.DrawColour) Console.ForegroundColor = drawable.DrawColour;
            
            Console.Write(drawable.DrawCharacter);
            this.LinesDrawnTo.Add(drawY);
        }

        public int GameYToScreenY(int gameY)
        {
            return Console.WindowHeight - gameY;
        }
    }
}