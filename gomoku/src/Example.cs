
namespace Gomoku
{
    using System;
    class GomocupEngine : GomocupInterface
    {
        /// <summary>
        /// The max board.
        /// </summary>
        private const int MaxBoard = 100;

        /// <summary>
        /// The board.
        /// </summary>
        private int[,] board = new int[MaxBoard, MaxBoard];

        /// <inheritdoc />
        /// <summary>
        /// Gets the brain about.
        /// </summary>
        public override string BrainAbout
        {
            get
            {
                return
                    ("name=\"DeepMind\", author=\"Peau_c Samuel_r\", version=\"-1\", country=\"France\", www=\"www.epitech.eu\""
                    );

            }
        }

        /// <inheritdoc />
        /// <summary>
        /// The brain init.
        /// </summary>
        public override void BrainInit()
        {
            if (this.Width < 5 || this.Height < 5)
            {
                Console.WriteLine("ERROR size of the board");
                return;
            }
            if (this.Width > MaxBoard || this.Height > MaxBoard)
            {
                Console.WriteLine("ERROR Maximal board size is " + MaxBoard);
                return;
            }
            Console.WriteLine("OK");
        }

        /// <inheritdoc />
        /// <summary>
        /// The brain restart.
        /// </summary>
        public override void BrainRestart()
        {
            for (int x = 0; x < this.Width; x++) for (int y = 0; y < this.Height; y++) board[x, y] = 0;

            Console.WriteLine("OK");
        }

        /// <summary>
        /// The is free.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsFree(int x, int y)
        {
            return x >= 0 && y >= 0 && x < this.Width && y < this.Height && board[x, y] == 0;
        }

        /// <inheritdoc />
        /// <summary>
        /// The brain my.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        public override void BrainMy(int x, int y)
        {
            if (this.IsFree(x, y))
            {
                board[x, y] = 1;
            }
            else
            {
                Console.WriteLine("ERROR my move [{0},{1}]", x, y);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// The brain opponents.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        public override void BrainOpponents(int x, int y)
        {
            if (this.IsFree(x, y))
            {
                board[x, y] = 2;
            }
            else
            {
                Console.WriteLine("ERROR opponents's move [{0},{1}]", x, y);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// The brain block.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        public override void BrainBlock(int x, int y)
        {
            if (this.IsFree(x, y))
            {
                board[x, y] = 3;
            }
            else
            {
                Console.WriteLine("ERROR winning move [{0},{1}]", x, y);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// The brain takeback.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="T:System.Int32" />.
        /// </returns>
        public override int BrainTakeback(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < Width && y < this.Height && board[x, y] != 0)
            {
                board[x, y] = 0;
                return 0;
            }
            return 2;
        }

        /// <inheritdoc />
        /// <summary>
        /// The brain turn.
        /// </summary>
        public override void BrainTurn()
        {
           // do_mymove(x, y);
        }

        public override void BrainEnd()
        {
        }

        public override void BrainEval(int x, int y)
        {
        }
    }
}
