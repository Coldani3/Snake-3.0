using System;

namespace snake_30
{
    public class Food : IDrawable, ICoordinates
    {
        public int X { get; set; }
        public int Y { get; set; }
        public ConsoleColor DrawColour { get => ConsoleColor.Yellow; }
        public char DrawCharacter { get => 'O'; }

        public Food(int gameStartX, int gameStartY)
        {
            this.X = gameStartX;
            this.Y = gameStartY;
        }
    }
}