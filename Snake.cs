using System;
using System.Collections.Generic;

namespace snake_30
{
    public class Snake
    {
        //Snake head will always be the first (0th) element
        //We can also cast these to the appropriate pieces
        public List<SnakeComponent> Pieces = new List<SnakeComponent>();

        public Snake()
        {

        }

        //Factory method to add more to the snake.
        public void Grow()
        {

        }

        public void ChangeDirection()
        {

        }
    }

    public abstract class SnakeComponent : IDrawable, ICoordinates
    {
        public int X { get; set; }
        public int Y { get; set; }
        public abstract ConsoleColor DrawColor { get; }
        public abstract char DrawCharacter { get; }
    }

    public class SnakeHead : SnakeComponent
    {
        public override ConsoleColor DrawColor { get => ConsoleColor.Green; }
        public override char DrawCharacter { get => '>'; }
    }

    public class SnakeBody : SnakeComponent
    {
        public override ConsoleColor DrawColor { get => ConsoleColor.Green; }
        public override char DrawCharacter { get => '■'; }
    }

    public enum Direction 
    {
        Up,
        Down,
        Left,
        Right,
    }
}