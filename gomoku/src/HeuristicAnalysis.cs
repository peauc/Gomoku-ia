namespace Gomoku
{
    using System.Collections.Generic;
    using System.Threading;

    public class HeuristicAnalysis
    {
        private readonly int width;

        private readonly int height;

        private int[,] boardToCompute;

        private int xToCompute;

        private int yToCompute;

        private int[] computedScores;

        private ManualResetEvent[] doneEvents;

        public HeuristicAnalysis(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.Patterns = new List<Pattern>
            {
                new Pattern("11111", 100000, 10000),
                new Pattern("011110", 1000, 10000),
                new Pattern("011112", 70, 100),
                new Pattern("211110", 70, 100),
                new Pattern("0101110", 70, 100),
                new Pattern("0111010", 70, 100),
                new Pattern("0110110", 70, 100),
                new Pattern("01110", 60, 60),
                new Pattern("2011102", -10, -10),
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
                new Pattern("0110", 40, 40),
                new Pattern("201102", -40, -40),
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

            // Check vertically
            for (int x = 0; x < this.width; x++)
            {
                score += this.ComputeLine(board, x, 0, 0, 1, 0);
            }

            // Check diagonals
            for (int x = 0; x <= this.width - 5; x++)
            {
                score += this.ComputeLine(board, x, 0, 1, 1, 0);
                if (x != 0)
                {
                    score += this.ComputeLine(board, x, this.height - 1, 1, -1, 0);
                }
            }

            // Check horizontally
            for (int y = 0; y < this.height; y++)
            {
                score += this.ComputeLine(board, 0, y, 1, 0, 0);
            }

            // Check diagonals
            for (int y = 3; y < this.height; y++)
            {
                score += this.ComputeLine(board, 0, y, 1, -1, 0);
                if (y != 0)
                {
                    score += this.ComputeLine(board, 0, y, 1, 1, 0);
                }
            }

            return score;
        }

        public int Compute(int[,] board, int x, int y)
        {
            this.boardToCompute = board;
            this.xToCompute = x;
            this.yToCompute = y;
            this.computedScores = new int[4];
            this.doneEvents = new[]
            {
                new ManualResetEvent(false),
                new ManualResetEvent(false),
                new ManualResetEvent(false),
                new ManualResetEvent(false)
            };

            ThreadPool.QueueUserWorkItem(this.ComputeVertical);
            ThreadPool.QueueUserWorkItem(this.ComputeHorizontal);
            ThreadPool.QueueUserWorkItem(this.ComputeDiagonal1);
            ThreadPool.QueueUserWorkItem(this.ComputeDiagonal2);

            // ReSharper disable once CoVariantArrayConversion
            WaitHandle.WaitAll(this.doneEvents);

            return this.computedScores[0] + this.computedScores[1] + this.computedScores[2] + this.computedScores[3];
        }

        private void ComputeVertical(object ignored)
        {
            this.computedScores[0] = this.ComputeLine(this.boardToCompute, this.xToCompute, 0, 0, 1, 0);
            this.doneEvents[0].Set();
        }

        private void ComputeHorizontal(object ignored)
        {
            this.computedScores[1] = this.ComputeLine(this.boardToCompute, 0, this.yToCompute, 1, 0, 1);
            this.doneEvents[1].Set();
        }

        private void ComputeDiagonal1(object ignored)
        {
            int startX;
            int startY;

            if (this.xToCompute >= this.yToCompute)
            {
                startX = this.xToCompute - this.yToCompute;
                startY = 0;
            }
            else
            {
                startX = 0;
                startY = this.yToCompute - this.xToCompute;
            }

            this.computedScores[2] = this.ComputeLine(this.boardToCompute, startX, startY, 1, 1, 2);
            this.doneEvents[2].Set();
        }

        private void ComputeDiagonal2(object ignored)
        {
            int startX;
            int startY;

            if (this.xToCompute + this.yToCompute >= this.width)
            {
                startX = this.xToCompute - (this.width - this.yToCompute) + 1;
                startY = this.height - 1;
            }
            else
            {
                startX = 0;
                startY = this.yToCompute + this.xToCompute;
            }

            this.computedScores[3] = this.ComputeLine(this.boardToCompute, startX, startY, 1, -1, 3);
            this.doneEvents[3].Set();
        }

        private int ComputeLine(int[,] board, int initX, int initY, int xInc, int yInc, int index)
        {
            int score = 0;

            foreach (Pattern pattern in this.Patterns)
            {
                pattern.CurrentIndex[index] = 0;
                pattern.CurrentIndexOpponent[index] = 0;
            }

            // Check border
            foreach (Pattern pattern in this.Patterns)
            {
                pattern.Validate(2, index, ref score);
                pattern.ValidateOpponent(1, index, ref score);
            }

            while (initX < this.width && initY < this.height && initX >= 0 && initY >= 0)
            {
                foreach (Pattern pattern in this.Patterns)
                {
                    pattern.Validate(board[initY, initX], index, ref score);
                    pattern.ValidateOpponent(board[initY, initX], index, ref score);
                }

                initX += xInc;
                initY += yInc;
            }

            // Check border
            foreach (Pattern pattern in this.Patterns)
            {
                pattern.Validate(2, index, ref score);
                pattern.ValidateOpponent(1, index, ref score);
            }

            return score;
        }
    }
}