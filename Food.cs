using System;

namespace snake_30
{
    public class Food : IDrawable
    {
        public ConsoleColor DrawColor { get => ConsoleColor.Yellow; }
        public char DrawCharacter { get => 'O'; }   
    }
}