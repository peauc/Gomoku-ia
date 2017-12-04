﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Gomoku
{
    using System;

    internal class GomocupEngine : GomocupInterface
    {
        /// <summary>
        /// The max board.
        /// </summary>
        private const int MaxBoard = 100;

        private readonly int[,] board = new int[MaxBoard, MaxBoard];

        private HeuristicAnalysis heuristicAnalysis;

        private List<Tuple<int, int>> movesAvailable;

        private List<Tuple<int, int>> movesUnavailable;

        public override string BrainAbout
        {
            get
            {
                return
                    "name=\"DeepMind\", author=\"Peau_c Samuel_r\", version=\"-1\", country=\"France\", www=\"www.epitech.eu\"";
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
            this.InitMoves();
            this.heuristicAnalysis = new HeuristicAnalysis(this.Width, this.Height);
            Console.WriteLine("OK");
        }

        /// <inheritdoc />
        /// <summary>
        /// The brain restart.
        /// </summary>
        public override void BrainRestart()
        {
            for (int x = 0; x < this.Width; x++) for (int y = 0; y < this.Height; y++) board[x, y] = 0;

            this.InitMoves();
            this.heuristicAnalysis = new HeuristicAnalysis(this.Width, this.Height);

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

        public override void BrainMy(int x, int y)
        {
            if (this.IsFree(x, y))
            {
                this.board[y, x] = 1;
                this.movesAvailable.Remove(new Tuple<int, int>(x, y));
                this.movesUnavailable.Add(new Tuple<int, int>(x, y));
            }
            else
            {
                Console.WriteLine("ERROR my move [{0},{1}]", x, y);
            }
        }

        public override void BrainOpponents(int x, int y)
        {
            if (this.IsFree(x, y))
            {
                this.board[y, x] = 2;
                this.movesAvailable.Remove(new Tuple<int, int>(x, y));
                this.movesUnavailable.Add(new Tuple<int, int>(x, y));
            }
            else
            {
                Console.WriteLine("ERROR opponent's move [{0},{1}]", x, y);
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
            if (x >= 0 && y >= 0 && x < this.Width && y < this.Height && this.board[x, y] != 0)
            {
                this.board[x, y] = 0;
                return 0;
            }
            return 2;
        }

        public override void BrainMoves()
        {
            Console.WriteLine("Displaying available moves :");
            this.DisplayMoves(this.movesAvailable);
            Console.WriteLine();
            Console.WriteLine("Displaying unavailable moves :");
            this.DisplayMoves(this.movesUnavailable);
            Console.WriteLine();
        }

        public override void BrainTurn()
        {
            Tuple<int, int> bestMove = this.AlphaBeta(2, int.MinValue, int.MaxValue, true, -1);
            if (bestMove.Item2 != -1)
            {
                this.DoMymove(this.movesAvailable[bestMove.Item2].Item1, this.movesAvailable[bestMove.Item2].Item2);
            }
        }

        public override void BrainDisplay()
        {
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    Console.Write(this.board[y, x]);
                }
                Console.WriteLine();
            }
        }

        public override void BrainHeuristic()
        {
            Console.WriteLine($"score = {this.heuristicAnalysis.Compute(this.board)}");
        }

        private Tuple<int, int> AlphaBeta(int depth, int alpha, int beta, bool maximizingPlayer, int moveIndex)
        {
            if (depth == 0 || this.movesAvailable.Count == 0)
            {
                var yay = new Tuple<int, int>(this.heuristicAnalysis.Compute(this.board), moveIndex);
                //if (moveIndex == 0 && depth == 0)
                //{
                //    this.BrainDisplay();
                //    Console.WriteLine($"score = {yay.Item1}");
                //}
                return yay;
            }

            if (maximizingPlayer)
            {
                Tuple<int, int> max = new Tuple<int, int>(int.MinValue, 0);
                for (int i = 0; i < this.movesAvailable.Count; i++)
                {
                    Tuple<int, int> move = this.movesAvailable[0];
                    this.ApplyMove(this.movesAvailable[0], 1);
                    Tuple<int, int> ret = this.AlphaBeta(depth - 1, alpha, beta, false, i);
                    if (depth == 2)
                        Console.WriteLine($"Move [{move.Item1},{move.Item2}] = {ret.Item1}");
                    max = (max.Item1 >= ret.Item1) ? max : ret;
                    alpha = (alpha >= max.Item1) ? alpha : max.Item1;
                    this.UnapplyMove();
                    if (beta <= alpha)
                    {
                        break; // BETA cut-off
                    }
                }

                return max;
            }
            else
            {
                Tuple<int, int> min = new Tuple<int, int>(int.MaxValue, 0);
                for (int i = 0; i < this.movesAvailable.Count; i++)
                {
                    this.ApplyMove(this.movesAvailable[0], 2);
                    Tuple<int, int> ret = this.AlphaBeta(depth - 1, alpha, beta, true, i);
                    min = (min.Item1 <= ret.Item1) ? min : ret;
                    beta = (beta <= min.Item1) ? beta : min.Item1;
                    this.UnapplyMove();
                    if (beta <= alpha)
                    {
                        break; // ALPHA cut-off
                    }
                }

                return min;
            }
        }

        private void InitMoves()
        {
            this.movesAvailable = new List<Tuple<int, int>>();
            this.movesUnavailable = new List<Tuple<int, int>>();

            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    if (this.board[y, x] == 0)
                    {
                        this.movesAvailable.Add(new Tuple<int, int>(x, y));
                    }
                }
            }
        }

        private void DisplayMoves(List<Tuple<int, int>> moves)
        {
            foreach (Tuple<int, int> move in moves)
            {
                Console.WriteLine($"x = {move.Item1}; y = {move.Item2}");
            }
        }

        private void ApplyMove(Tuple<int, int> move, int player)
        {
            //Console.WriteLine($"Applying move [{move.Item1},{move.Item2}]");
            this.board[move.Item2, move.Item1] = player;
            this.movesUnavailable.Add(move);
            this.movesAvailable.Remove(move);
        }

        private void UnapplyMove()
        {
            Tuple<int, int> move = this.movesUnavailable.Last();

            //Console.WriteLine($"Un-applying move [{move.Item1},{move.Item2}]");
            this.board[move.Item2, move.Item1] = 0;
            this.movesAvailable.Add(move);
            this.movesUnavailable.Remove(move);
        }

        public override void BrainEnd()
        {
        }

        public override void BrainEval(int x, int y)
        {
        }
    }
}