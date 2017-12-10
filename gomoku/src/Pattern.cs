namespace Gomoku
{
    using System.Collections.Generic;
    using System.Linq;

    public class Pattern
    {
        private readonly List<int> cells;

        private readonly int selfScore;

        private readonly int opponentScore;

        public Pattern(string pattern, int selfScore, int opponentScore)
        {
            this.cells = new List<int>();
            this.selfScore = selfScore;
            this.opponentScore = opponentScore;
            this.CurrentIndex = new int[4];
            this.CurrentIndexOpponent = new int[4];

            foreach (char c in pattern)
            {
                this.cells.Add((int)char.GetNumericValue(c));
            }
        }

        public int[] CurrentIndex { get; set; }

        public int[] CurrentIndexOpponent { get; set; }

        public void Validate(int cell, int index, ref int score)
        {
            if (cell == this.cells[this.CurrentIndex[index]])
            {
                this.CurrentIndex[index]++;
                if (this.CurrentIndex[index] == this.cells.Count())
                {
                    this.CurrentIndex[index] = 0;
                    score += this.selfScore;
                }
            }
            else
            {
                this.CurrentIndex[index] = cell == this.cells[0] ? 1 : 0;
            }
        }

        public void ValidateOpponent(int cell, int index, ref int score)
        {
            int c = (cell == 0) ? 0 : (cell == 2) ? 1 : 2;
            if (c == this.cells[this.CurrentIndexOpponent[index]])
            {
                this.CurrentIndexOpponent[index]++;
                if (this.CurrentIndexOpponent[index] == this.cells.Count())
                {
                    this.CurrentIndexOpponent[index] = 0;
                    score -= this.opponentScore;
                }
            }
            else
            {
                this.CurrentIndexOpponent[index] = c == this.cells[0] ? 1 : 0;
            }
        }
    }
}