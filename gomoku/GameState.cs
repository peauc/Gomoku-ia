namespace Gomoku
{
    using System;

    public class GameState
    {
        public const int GomokuBoardSize = 19;

        public GameState()
        {
            this.Map = new Cell[GomokuBoardSize][];

            for (int i = 0; i < GomokuBoardSize; i++)
            {
                this.Map[i] = new Cell[GomokuBoardSize];
                for (int j = 0; j < GomokuBoardSize; j++)
                {
                    this.Map[i][j] = Cell.Free;
                }
            }
        }

        public Cell[][] Map { get; set; }

        public void PrintMap()
        {
            foreach (Cell[] cells in this.Map)
            {
                foreach (Cell cell in cells)
                {
                    Console.Write(cell.Value() + " ");
                }

                Console.WriteLine();
            }
        }
    }
}