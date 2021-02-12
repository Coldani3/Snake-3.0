using System;

namespace snake_30
{
    public class Food : IDrawable, ICoordinates
    {
        public int X { get; set; }
        public int Y { get; set; }
        public ConsoleColor DrawColor { get => ConsoleColor.Yellow; }
        public char DrawCharacter { get => 'O'; }   
    }
}