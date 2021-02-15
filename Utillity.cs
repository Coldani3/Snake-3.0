namespace snake_30
{
    public class Utillity
    {
        public static int[] GetCoordDiffByDirection(Direction direction)
        {
            return new int[] { direction == Direction.Right ? 1 : (direction == Direction.Left ? -1 : 0), direction == Direction.Up ? 1 : (direction == Direction.Down ? -1 : 0)};
        }
    }
}