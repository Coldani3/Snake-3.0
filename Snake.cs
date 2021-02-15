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
            int newPieceX = 0;
            int newPieceY = 0;

            foreach (int[] coordinate in this.PreviousTailCoords)
            {
                //we are checking for food in previous coordinates because of the unlikely event food is spawned in one of those
                //coordinates after the snake eats a food (a 0.03% chance mind you but not out of the question, it's still many orders 
                //of magnitude more likely than Dream's disputed speedruns)
                if (AnyPieceAtCoords(coordinate[0], coordinate[1]) || Program.Logic.IsFoodAtLocation(coordinate[0], coordinate[1])) 
                    continue;
                else
                {
                    newPieceX = coordinate[0];
                    newPieceY = coordinate[1];
                    prevCoordAvailable = true;
                    //this.Pieces.Add(new SnakeBody(coordinate[0], coordinate[1]));
                }
            }

            if (!prevCoordAvailable)
            {
                Program.DebugLog("Failed last coordinate Grow!");
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

                if (this.AnyPieceAtCoords(newPieceX, newPieceY))
                {
                    Program.DebugLog("Failed orientation infer Grow!");
                    //failing that, go by the facing of the head. 
                    int facingInt = (int) head.Facing;    
                    int yChange = facingInt < 2 ? (facingInt == 0 ? 1 : -1) : 0;
                    int xChange = facingInt > 1 ? (facingInt - 2 == 0 ? -1 : 1) : 0;

                    if (!this.AnyPieceAtCoords(lastPiece.X + xChange, lastPiece.Y + yChange))
                    {
                        newPieceX = lastPiece.X + xChange;
                        newPieceY = lastPiece.Y + yChange;
                    }
                    else
                    {
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
                            Program.DebugLog("Failed clockwise Grow");
                        }
                        else
                        {
                            newPieceX = validCoords[0];
                            newPieceY = validCoords[1];
                        }
                    }
                }
            }

            Program.DebugLog("Added piece!");
            this.Pieces.Add(new SnakeBody(newPieceX, newPieceY));
        }

        public bool AnyPieceAtCoords(int gameX, int gameY)
        {
            foreach (SnakeComponent piece in this.Pieces) 
                if (piece.X == gameX && piece.Y == gameY) return true;

            return false;
        }

        public void ChangeDirection(Direction newDirection)
        {
            ((SnakeHead) this.Pieces[0]).Facing = newDirection;
        }
        
        public void MoveForward()
        {
            Program.DebugLog("Beginning move!");
            SnakeHead head = (SnakeHead) this.Pieces[0];
            int xChange = head.Facing == Direction.Right ? 1 : (head.Facing == Direction.Left ? -1 : 0);
            int yChange = head.Facing == Direction.Up ? 1 : (head.Facing == Direction.Down ? -1 : 0);

            int nextHeadX = head.X + xChange;
            int nextHeadY = head.Y + yChange;

            if (nextHeadX < Program.WindowWidth - 1 || nextHeadY < Program.WindowHeight - 1 || nextHeadX > 0 || nextHeadY > 0)
            {
                //check collisions
                //NOTE TO FUTURE ME!
                //It appears doing this loop with a foreach causes the Logic thread to crash
                //as Enumerators don't like it when the thing they are enumerating changes.
                for (int i = 0; i < Program.Food.Count; i++)
                {
                    Food food = Program.Food[i];
                    if (food.X == nextHeadX && food.Y == nextHeadY) 
                    {
                        Program.DebugLog("Food count [start]: " + Program.Food.Count);
                        Program.Logic.EatFoodAtLocation(nextHeadX, nextHeadY, this);
                        Program.DebugLog("Food count [end]: " + Program.Food.Count);
                    }
                }

                Program.DebugLog("Body pieces count (inclding head): " + this.Pieces.Count);

                //if (this.AnyPieceAtCoords(head.X, head.Y)) Program.Logic.GameOver();
                for (int i = 1; i < this.Pieces.Count; i++)
                {
                    Program.DebugLog("Checking for collision with body loop: " + i);
                    if (nextHeadX == this.Pieces[i].X && nextHeadY == this.Pieces[i].Y) Program.Logic.GameOver();
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
                        Program.DebugLog("Moving pieces");
                        this.Pieces[i].X = this.Pieces[i - 1].X;
                        this.Pieces[i].Y = this.Pieces[i - 1].Y;
                    }
                }

                Program.DebugLog("Moving!");
                //finally, move the head
                Program.DebugLog($"New Head X: {nextHeadX}, New Head Y: {nextHeadX}");
                head.X = nextHeadX;
                head.Y = nextHeadY;
            }
        }
    }

    public abstract class SnakeComponent : IDrawable, ICoordinates
    {
        public int X { get; set; }
        public int Y { get; set; }
        public abstract ConsoleColor DrawColour { get; }
        public abstract char DrawCharacter { get; }
    }

    public class SnakeHead : SnakeComponent
    {
        public override ConsoleColor DrawColour { get => ConsoleColor.Cyan; }
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
        public override ConsoleColor DrawColour { get => ConsoleColor.Cyan; }
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