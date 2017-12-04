namespace Gomoku
{
    using System.Collections.Generic;

    public class HeuristicAnalysis
    {
        private readonly int width;

        private readonly int height;

        public HeuristicAnalysis(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.Patterns = new List<Pattern>
            {
                new Pattern("11111", 100000, 10000),
                new Pattern("011110", 1000, 10000),
                new Pattern("011112", 70, 10000),
                new Pattern("211110", 70, 10000),
                new Pattern("0101110", 70, 10000),
                new Pattern("0111010", 70, 10000),
                new Pattern("0110110", 70, 10000),
                new Pattern("01110", 60, 60),
                new Pattern("010110", 60, 60),
                new Pattern("011010", 60, 60),

                new Pattern("011112", 60, 60),
                new Pattern("211110", 60, 60),
                new Pattern("101112", 60, 60),
                new Pattern("211101", 60, 60),
                new Pattern("110112", 60, 60),
                new Pattern("211011", 60, 60),
                new Pattern("111012", 60, 60),
                new Pattern("210111", 60, 60),
                new Pattern("111102", 60, 60),
                new Pattern("201111", 60, 60),

                new Pattern("001112", 50, 50),
                new Pattern("211100", 50, 50),
                new Pattern("010112", 50, 50),
                new Pattern("211010", 50, 50),
                new Pattern("011012", 50, 50),
                new Pattern("210110", 50, 50),
                new Pattern("10011", 50, 50),
                new Pattern("11001", 50, 50),
                new Pattern("10101", 50, 50),
                new Pattern("2011102", 50, 50),
                new Pattern("00110", 40, 40),
                new Pattern("01100", 40, 40),
                new Pattern("01010", 40, 40),
                new Pattern("010010", 40, 40),
                new Pattern("000112", 30, 30),
                new Pattern("211000", 30, 30),
                new Pattern("001012", 30, 30),
                new Pattern("210100", 30, 30),
                new Pattern("010012", 30, 30),
                new Pattern("210010", 30, 30),
                new Pattern("10001", 30, 30),
                new Pattern("2010102", 30, 30),
                new Pattern("2011002", 30, 30),
                new Pattern("2001102", 30, 30)
            };
        }

        public List<Pattern> Patterns { get; set; }

        public int Compute(int[,] board)
        {
            int score = 0;

            for (int x = 0; x < this.width; x++)
            {
                score += this.ComputeLine(board, x, 0, 0, 1);
                score += this.ComputeLine(board, x, 0, 1, 1);
            }

            for (int y = 0; y < this.height; y++)
            {
                score += this.ComputeLine(board, 0, y, 1, 0);
                if (y != 0)
                {
                    score += this.ComputeLine(board, 0, y, 1, 1);
                }
            }

            return score;
        }

        private int ComputeLine(int[,] board, int initX, int initY, int xInc, int yInc)
        {
            int score = 0;

            foreach (Pattern pattern in this.Patterns)
            {
                pattern.CurrentIndex = 0;
                pattern.CurrentIndexOpponent = 0;
            }

            // Check border
            foreach (Pattern pattern in this.Patterns)
            {
                // Check self score
                pattern.Validate(2);

                // Check opponent's score
                pattern.ValidateOpponent(1);
            }

            while (initX < this.width && initY < this.height)
            {
                foreach (Pattern pattern in this.Patterns)
                {
                    // Check self score
                    if (pattern.Validate(board[initY, initX]))
                    {
                        score += pattern.SelfScore;
                    }

                    // Check opponent's score
                    if (pattern.ValidateOpponent(board[initY, initX]))
                    {
                        score -= pattern.OpponentScore;
                    }
                }

                initX += xInc;
                initY += yInc;
            }

            // Check border
            foreach (Pattern pattern in this.Patterns)
            {
                // Check self score
                if (pattern.Validate(2))
                {
                    score += pattern.SelfScore;
                }

                // Check opponent's score
                if (pattern.ValidateOpponent(1))
                {
                    score -= pattern.OpponentScore;
                }
            }

            return score;
        }
    }
}