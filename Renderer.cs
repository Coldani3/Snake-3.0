using System;

namespace snake_30
{
    public class Renderer
    {
        public Renderer()
        {

        }

        public void RenderAll()
        {

        }

        public void RenderSnake(Snake snake)
        {

        }

        public void Draw(int gameX, int gameY, IDrawable drawable)
        {
            Console.SetCursorPosition(gameX, GameYToScreenY(gameY));

            if (Console.ForegroundColor != drawable.DrawColour)
            {
                Console.ForegroundColor = drawable.DrawColour;
            }
            
            Console.Write(drawable.DrawCharacter);
        }

        public int GameYToScreenY(int gameY)
        {
            return Console.WindowHeight - gameY;
        }
    }
}