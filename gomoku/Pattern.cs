namespace Gomoku
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Pattern
    {
        public Pattern(string pattern, int selfScore, int opponentScore)
        {
            this.Cells = new List<Cell>();
            this.SelfScore = selfScore;
            this.OpponentScore = opponentScore;
            this.CurrentIndex = 0;
            this.CurrentIndexOpponent = 0;

            Dictionary<char, Cell> charToCell = new Dictionary<char, Cell>
                                                    {
                                                        { '0', Cell.Free },
                                                        { '1', Cell.Self },
                                                        { '2', Cell.Opponent }
                                                    };

            foreach (char c in pattern)
            {
                this.Cells.Add(charToCell[c]);
            }
        }

        public List<Cell> Cells { get; }

        public int SelfScore { get; }

        public int OpponentScore { get; }

        public int CurrentIndex { get; set; }

        public int CurrentIndexOpponent { get; set; }

        public bool Validate(Cell cell)
        {
            if (cell == this.Cells[this.CurrentIndex])
            {
                this.CurrentIndex++;
            }
            else
            {
                this.CurrentIndex = 0;
            }

            if (this.CurrentIndex == this.Cells.Count())
            {
                this.CurrentIndex = 0;
                return true;
            }

            return false;
        }

        public bool ValidateOpponent(Cell cell)
        {
            if (cell.Reversed() == this.Cells[this.CurrentIndexOpponent])
            {
                this.CurrentIndexOpponent++;
            }
            else
            {
                this.CurrentIndexOpponent = 0;
            }

            if (this.CurrentIndexOpponent == this.Cells.Count())
            {
                this.CurrentIndexOpponent = 0;
                return true;
            }

            return false;
        }
    }
}