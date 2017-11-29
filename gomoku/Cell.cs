namespace Gomoku
{
    using System;

    public enum Cell : ushort
    {
        Free,
        Self,
        Opponent
    }

    public static class CellExtensions
    {
        public static int Value(this Cell self)
        {
            switch (self)
            {
                case Cell.Free:
                    return 0;

                case Cell.Self:
                    return 1;

                case Cell.Opponent:
                    return 2;

                default:
                    throw new ArgumentOutOfRangeException(nameof(self), self, null);
            }
        }

        public static Cell Reversed(this Cell self)
        {
            switch (self)
            {
                case Cell.Free:
                    return Cell.Free;

                case Cell.Self:
                    return Cell.Opponent;

                case Cell.Opponent:
                    return Cell.Self;

                default:
                    throw new ArgumentOutOfRangeException(nameof(self), self, null);
            }
        }
    }
}