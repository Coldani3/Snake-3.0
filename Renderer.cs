using System;
using System.Collections.Generic;

namespace snake_30
{
    public class Renderer
    {
        //if first value of int[] is not ClearMultiCharFlag, it is the x and y of the char to clear.
        //otherwise, it is {ClearMultiCharFlag, x, y, and number of chars to clear}
        private List<int[]> ToClear = new List<int[]>();
        private readonly int ClearMultiCharFlag = Int32.MaxValue - 10;
        //creating strings every frame is expensive so we cache them. first is length, second is the clear string
        private Dictionary<int, string> ClearStringCache = new Dictionary<int, string>();

        public Renderer()
        {

        }

        public void RenderAll()
        {
            if (Program.TickRate > 0)
            {
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
            string playerScoreText = $"{Program.PlayerSnake.Pieces.Count - 1}";
            Console.SetCursorPosition(Console.WindowWidth - 5, 1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(playerScoreText);
            this.ToClear.Add(new int[] {ClearMultiCharFlag, Console.WindowWidth - 5, 1, playerScoreText.Length});
        }

        public void RenderDebugLog()
        {
            for (int i = 0; i < Program.DebugLogStack.Count; i++)
            {
                int drawY = Console.WindowHeight - Program.DebugLogStack.Count + i;
                Console.SetCursorPosition(0, drawY);
                Console.Write(Program.DebugLogStack[i]);
                this.ToClear.Add(new int[] {ClearMultiCharFlag, 0, drawY, Program.DebugLogStack[i].Length});
            }
        }

        public void ClearScreen()
        {
            //NOTE: if it's not clearing properly, check the line you added to the list was the SCREEN Y and not the game y
            foreach (int[] toClearLine in this.ToClear)
            {
                if (toClearLine[0] == ClearMultiCharFlag)
                { 
                    Console.SetCursorPosition(toClearLine[1], toClearLine[2]);
                    String clearLine;
                    if (!ClearStringCache.ContainsKey(toClearLine[3])) 
                    {
                        clearLine = new String(' ', toClearLine[3]);
                        ClearStringCache.Add(toClearLine[3], clearLine);
                    }
                    else
                    {
                        clearLine = ClearStringCache[toClearLine[3]];
                    }
                    Console.Write(clearLine);
                }
                else 
                {
                    Console.SetCursorPosition(toClearLine[0], toClearLine[1]);
                    Console.Write(' ');
                }
            }

            this.ToClear.Clear();
        }

        public void Draw(int gameX, int gameY, IDrawable drawable)
        {
            int drawY = GameYToScreenY(gameY);
            Console.SetCursorPosition(gameX, drawY);

            if (Console.ForegroundColor != drawable.DrawColour) Console.ForegroundColor = drawable.DrawColour;
            
            Console.Write(drawable.DrawCharacter);
            this.ToClear.Add(new int[] {gameX, drawY});
        }

        public int GameYToScreenY(int gameY)
        {
            return Console.WindowHeight - gameY;
        }
    }
}