namespace Gomoku
{
    public class Move
    {
        public Move(uint x, uint y)
        {
            this.X = x;
            this.Y = y;
        }

        public uint X { get; set; }

        public uint Y { get; set; }

        public string ToBoardNotation()
        {
            return $"{'a' + this.Y}{this.X}";
        }
    }
}