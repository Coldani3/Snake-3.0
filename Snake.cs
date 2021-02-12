using System;
using System.Collections.Generic;

namespace snake_30
{
    public class Snake
    {
        //Snake head will always be the first (0th) element
        //We can also cast these to the appropriate pieces
        public List<SnakeComponent> Pieces;
        //Used to inform extensions of the snake where they should appear first. First coord is the first tried addition,
        //second is the second tried. Failing that, it will search for any available spots.
        public int[][] PreviousTailCoords = new int[2][];

        public Snake(int headStartGameX, int headStartGameY)
        {
            //initialise head and pt it in its place
            this.Pieces = new List<SnakeComponent>() {new SnakeHead(headStartGameX, headStartGameY)};
        }

        //Factory method to add more to the snake.
        public void Grow()
        {

        }

        private void UpdatePreviousTailCoords()
        {

        }

        public void ChangeDirection(Direction newDirection)
        {

        }
        
        public void MoveForward()
        {
            SnakeHead head = (SnakeHead) this.Pieces[0];
            int xChange = head.Facing == Direction.Right ? 1 : (head.Facing == Direction.Left ? -1 : 0);
            int yChange = head.Facing == Direction.Up ? 1 : (head.Facing == Direction.Down ? -1 : 0);

            //check collisions
            foreach (Food food in Program.Food)
            {
                if (food.X == head.X + xChange && food.Y == head.Y + yChange)
                {
                    Program.Logic.GameOver();
                }
            }

            //first, update the previous coords
            //shift the closest previous coord to the latest
            this.PreviousTailCoords[1] = this.PreviousTailCoords[0];
            //and now, set the new closest previous coord
            this.PreviousTailCoords[0] = new int[] { this.Pieces[this.Pieces.Count - 1].X, this.Pieces[this.Pieces.Count - 1].Y };

            //move the last body piece to the position of the next last and so on, finally move the head.
            //it seems counter intuitive, I know, but if you do it head first, all the pieces bunch up together,
            //as the latter pieces have to move to the former pieces, and if the former piece has already moved,
            //the latter will just be exactly where the former piece moved to, as I discovered during the creation 
            //of the previous iteration.
            //theoretically you could reference a copy but that's really messy, prone to bugs and inefficient
            if (this.Pieces.Count > 1)
            {
                for (int i = this.Pieces.Count - 1; i > 0; i--)
                {
                    this.Pieces[i].X = this.Pieces[i - 1].X;
                    this.Pieces[i].Y = this.Pieces[i - 1].Y;
                }
            }

            //finally, move the head
            head.X = head.X + xChange;
            head.Y = head.Y + yChange;
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
        public Direction Facing = (Direction) Program.RNG.Next(4);
        public SnakeHead(int startGameX, int startGameY)
        {
            this.X = startGameX;
            this.Y = startGameY;
        }

        private char GetChar()
        {
            switch (Facing)
            {
                case Direction.Up:
                    return '^';
                case Direction.Right:
                    return '>';
                case Direction.Down:
                    return 'V';
                case Direction.Left:
                    return '<';
                default:
                    //Assume up in worst case scenario
                    goto case Direction.Up;
            }
        }
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