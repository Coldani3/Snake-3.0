using System;
using System.Collections.Generic;

namespace snake_30
{
    //Facade that represents the entire snake
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
            //initialise head and put it in its place
            this.Pieces = new List<SnakeComponent>() {new SnakeHead(headStartGameX, headStartGameY)};
        }

        //Factory method to add more to the snake.
        public void Grow()
        {
            bool prevCoordAvailable = false;

            foreach (int[] coordinate in this.PreviousTailCoords)
            {
                //we are checking for food in previous coordinates because of the unlikely event food is spawned in one of those
                //coordinates after the snake eats a food (a 0.03% chance mind you but not out of the question, it's still many orders 
                //of magnitude more likely than Dream's disputed speedruns)
                if (AnyPieceAtCoords(coordinate[0], coordinate[1]) || Program.Logic.IsFoodAtLocation(coordinate[0], coordinate[1])) 
                    continue;
                else
                {
                    this.Pieces.Add(new SnakeBody(coordinate[0], coordinate[1]));
                }
            }

            if (!prevCoordAvailable)
            {
                int newPieceX;
                int newPieceY;
                //first, attempt to infer the orientation of the last and second to last piece and continue on from that.
                SnakeComponent lastPiece = this.Pieces[this.Pieces.Count - 1];
                SnakeComponent sToLastPiece = this.Pieces[this.Pieces.Count - 2];
                SnakeHead head = (SnakeHead) this.Pieces[0];
                //we can assume it will not be diagonal as the algorithm never tries to go diagonally elsewhere
                if (lastPiece.X == sToLastPiece.X)
                {
                    //they are on the same X
                    newPieceX = lastPiece.X;
                    newPieceY = lastPiece.Y - (lastPiece.Y - sToLastPiece.Y);
                }
                else
                {
                    newPieceX = lastPiece.X - (lastPiece.X - sToLastPiece.X);
                    newPieceY = lastPiece.Y;
                }

                //failing that, go by the facing of the head.                

                //as a last resort, go for the first available position going clockwise
                //if nothing else works, idk lol I'll think of something
                int[][] relCoordsToTry = new int[4][] {new int[2] {0, 1}, new int[2] {1, 0}, new int[2] {0, -1}, new int[2] {-1, 0}};
                int[] validCoords = null;

                foreach (int[] toTry in relCoordsToTry)
                {
                    if (!this.AnyPieceAtCoords(lastPiece.X + toTry[0], lastPiece.Y + toTry[1]))
                    {
                        validCoords = toTry;
                        break;
                    }
                }

                if (validCoords == null)
                {
                    //the thing I'll think of
                }
                else
                {
                    newPieceX = validCoords[0];
                    newPieceY = validCoords[1];
                }
            }
        }

        public bool AnyPieceAtCoords(int gameX, int gameY)
        {
            foreach (SnakeComponent piece in this.Pieces)
            {
                if (piece.X == gameX && piece.Y == gameY) return true;
            }

            return false;
        }

        public void ChangeDirection(Direction newDirection)
        {
            ((SnakeHead) this.Pieces[0]).Facing = newDirection;
        }
        
        public void MoveForward()
        {
            SnakeHead head = (SnakeHead) this.Pieces[0];
            int xChange = head.Facing == Direction.Right ? 1 : (head.Facing == Direction.Left ? -1 : 0);
            int yChange = head.Facing == Direction.Up ? 1 : (head.Facing == Direction.Down ? -1 : 0);

            int nextHeadX = head.X + xChange;
            int nextHeadY = head.Y + yChange;

            if (!(nextHeadX > Program.WindowWidth || nextHeadY > Program.WindowHeight || nextHeadX < 0 || nextHeadY < 0))
            {
                //check collisions
                foreach (Food food in Program.Food)
                {
                    if (food.X == nextHeadX && food.Y == nextHeadY) Program.Logic.EatFoodAtLocation(nextHeadX, nextHeadY, this);
                }

                for (int i = 1; i < this.Pieces.Count; i++)
                {
                    if (head.X == this.Pieces[i].X && head.Y == this.Pieces[i].Y) Program.Logic.GameOver();
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
        public override ConsoleColor DrawColor { get => ConsoleColor.Cyan; }
        public override char DrawCharacter { get => this.GetChar(); }
        //State of the direction the head is facing
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
        public override ConsoleColor DrawColor { get => ConsoleColor.Cyan; }
        public override char DrawCharacter { get => '■'; }

        public SnakeBody(int startGameX, int startGameY)
        {
            this.X = startGameX;
            this.Y = startGameY;
        }
    }

    public enum Direction 
    {
        Up,
        Down,
        Left,
        Right,
    }
}