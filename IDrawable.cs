using System;

namespace snake_30 
{
    public interface IDrawable
    {
        ConsoleColor DrawColour { get => ConsoleColor.White; }
        char DrawCharacter { get; }
    }
}