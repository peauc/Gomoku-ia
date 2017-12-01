namespace Gomoku
{
    using System.Collections.Generic;
    using System.Linq;

    public class Pattern
    {
        public Pattern(string pattern, int selfScore, int opponentScore)
        {
            this.Cells = new List<int>();
            this.SelfScore = selfScore;
            this.OpponentScore = opponentScore;
            this.CurrentIndex = 0;
            this.CurrentIndexOpponent = 0;

            foreach (char c in pattern)
            {
                this.Cells.Add((int)char.GetNumericValue(c));
            }
        }

        public List<int> Cells { get; }

        public int SelfScore { get; }

        public int OpponentScore { get; }

        public int CurrentIndex { get; set; }

        public int CurrentIndexOpponent { get; set; }

        public bool Validate(int cell)
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

        public bool ValidateOpponent(int cell)
        {
            if (((cell == 1) ? 2 : 1) == this.Cells[this.CurrentIndexOpponent])
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