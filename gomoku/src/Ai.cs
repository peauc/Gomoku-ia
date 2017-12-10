namespace Gomoku
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    internal class GomocupEngine : GomocupInterface
    {
        private const int MaxBoard = 100;

        private const int Depth = 2;

        private readonly List<Tuple<int, int>> adjacentCells = new List<Tuple<int, int>>
                                                                   {
                                                                       new Tuple<int, int>(-1, -1),
                                                                       new Tuple<int, int>(0, -1),
                                                                       new Tuple<int, int>(1, -1),
                                                                       new Tuple<int, int>(-1, 0),
                                                                       new Tuple<int, int>(1, 0),
                                                                       new Tuple<int, int>(-1, +1),
                                                                       new Tuple<int, int>(0, +1),
                                                                       new Tuple<int, int>(1, +1)
                                                                   };

        private readonly int[,] board = new int[MaxBoard, MaxBoard];

        private readonly Stopwatch stopWatch = new Stopwatch();

        private HeuristicAnalysis heuristicAnalysis;

        private List<Tuple<int, int>> availableMoves;

        private List<Tuple<int, int>> disabledMoves;

        private Tuple<Tuple<int, int>, int> killerMove;

        public override string brain_about => "name=\"DeepMind\", author=\"Peau_c Samuel_r\", version=\"-1\", country=\"France\", www=\"www.epitech.eu\"";

        public override void brain_init()
        {
            if (this.width < 5 || this.height < 5)
            {
                Console.WriteLine("ERROR size of the board");
                return;
            }

            if (this.width > MaxBoard || this.height > MaxBoard)
            {
                Console.WriteLine("ERROR Maximal board size is " + MaxBoard);
                return;
            }

            this.InitMoves();
            this.heuristicAnalysis = new HeuristicAnalysis(this.width, this.height);
            Console.WriteLine("OK");
        }

        public override void brain_restart()
        {
            for (int x = 0; x < this.width; x++)
            {
                for (int y = 0; y < this.height; y++)
                {
                    this.board[x, y] = 0;
                }
            }

            this.InitMoves();
            this.heuristicAnalysis = new HeuristicAnalysis(this.width, this.height);

            Console.WriteLine("OK");
        }

        public override void brain_my(int x, int y)
        {
            if (this.IsFree(x, y))
            {
                this.board[y, x] = 1;
                this.availableMoves.Remove(new Tuple<int, int>(x, y));
                this.disabledMoves.Add(new Tuple<int, int>(x, y));
            }
            else
            {
                Console.WriteLine("ERROR my move [{0},{1}]", x, y);
            }
        }

        public override void brain_opponents(int x, int y)
        {
            if (this.IsFree(x, y))
            {
                this.board[y, x] = 2;
                this.availableMoves.Remove(new Tuple<int, int>(x, y));
                this.disabledMoves.Add(new Tuple<int, int>(x, y));
            }
            else
            {
                Console.WriteLine("ERROR opponent's move [{0},{1}]", x, y);
            }
        }

        public override void brain_block(int x, int y)
        {
            if (this.IsFree(x, y))
            {
                this.board[x, y] = 3;
            }
            else
            {
                Console.WriteLine("ERROR winning move [{0},{1}]", x, y);
            }
        }

        public override int brain_takeback(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < this.width && y < this.height && this.board[x, y] != 0)
            {
                this.board[x, y] = 0;
                return 0;
            }

            return 2;
        }

        public override void brain_end()
        {
        }

        public override void brain_eval(int x, int y)
        {
        }

        public override void brain_turn()
        {
            this.killerMove = null;
            this.stopWatch.Start();

            Tuple<int, int> bestMove = this.AlphaBeta(Depth, int.MinValue, int.MaxValue, true, this.heuristicAnalysis.Compute(this.board));

            this.stopWatch.Stop();
            this.stopWatch.Reset();

            if (bestMove.Item2 != -1)
            {
                this.do_mymove(this.availableMoves[bestMove.Item2].Item1, this.availableMoves[bestMove.Item2].Item2);
            }
        }

        private Tuple<int, int> AlphaBeta(int depth, int alpha, int beta, bool maximizingPlayer, int previousScore)
        {
            if (depth == 0 || this.availableMoves.Count == 0)
            {
                return new Tuple<int, int>(this.heuristicAnalysis.Compute(this.board, this.disabledMoves.Last().Item1, this.disabledMoves.Last().Item2) + previousScore, 0);
            }

            if (maximizingPlayer)
            {
                Tuple<int, int> max = new Tuple<int, int>(int.MinValue, 0);

                for (int i = 0; i < this.availableMoves.Count; i++)
                {
                    Tuple<int, int> mv = this.availableMoves.First();
                    int newScore = previousScore + this.heuristicAnalysis.Compute(this.board, mv.Item1, mv.Item2);
                    this.ApplyMove(this.availableMoves.First(), 1);
                    newScore += this.heuristicAnalysis.Compute(this.board, mv.Item1, mv.Item2);

                    if (!this.IsAlone(mv.Item1, mv.Item2))
                    {
                        Tuple<int, int> ret = this.AlphaBeta(depth - 1, alpha, beta, false, newScore);

                        this.killerMove = new Tuple<Tuple<int, int>, int>(this.availableMoves[ret.Item2], ret.Item2);

                        max = (max.Item1 >= ret.Item1) ? max : new Tuple<int, int>(ret.Item1, i);
                        alpha = (alpha >= max.Item1) ? alpha : max.Item1;
                    }

                    this.UnapplyMove();

                    if (alpha >= beta || this.stopWatch.ElapsedMilliseconds > 4800)
                    {
                        this.ResetMovesAvailable(i + 1);
                        break; // BETA cut-off
                    }
                }

                return max;
            }
            else
            {
                // minimizing player
                Tuple<int, int> min = new Tuple<int, int>(int.MaxValue, 0);
                int newScore;

                // Testing killer move
                if (this.killerMove != null && this.killerMove.Item2 != 0 && !this.IsAlone(this.killerMove.Item1.Item1, this.killerMove.Item1.Item2))
                {
                    this.board[this.killerMove.Item1.Item2, this.killerMove.Item1.Item1] = 1;
                    newScore = previousScore - this.heuristicAnalysis.Compute(
                                   this.board,
                                   this.killerMove.Item1.Item1,
                                   this.killerMove.Item1.Item2);

                    Tuple<int, int> ret = this.AlphaBeta(depth - 1, alpha, beta, true, newScore);
                    min = (min.Item1 <= ret.Item1) ? min : new Tuple<int, int>(ret.Item1, this.killerMove.Item2);
                    beta = (beta <= min.Item1) ? beta : min.Item1;
                    this.board[this.killerMove.Item1.Item2, this.killerMove.Item1.Item1] = 0;
                    if (alpha >= beta)
                    {
                        return min;
                    }
                }

                for (int i = 0; i < this.availableMoves.Count; i++)
                {
                    Tuple<int, int> mv = this.availableMoves.First();
                    newScore = previousScore - this.heuristicAnalysis.Compute(this.board, mv.Item1, mv.Item2);
                    this.ApplyMove(this.availableMoves.First(), 2);

                    if (!this.IsAlone(mv.Item1, mv.Item2))
                    {
                        Tuple<int, int> ret = this.AlphaBeta(depth - 1, alpha, beta, true, newScore);

                        min = (min.Item1 <= ret.Item1) ? min : new Tuple<int, int>(ret.Item1, i);
                        beta = (beta <= min.Item1) ? beta : min.Item1;
                    }

                    this.UnapplyMove();

                    if (alpha >= beta)
                    {
                        this.ResetMovesAvailable(i + 1);
                        break; // ALPHA cut-off
                    }
                }

                return min;
            }
        }

        private void ResetMovesAvailable(int i)
        {
            while (i < this.availableMoves.Count)
            {
                Tuple<int, int> mv = this.availableMoves.First();
                this.availableMoves.Remove(mv);
                this.availableMoves.Add(mv);
                i++;
            }
        }

        private void InitMoves()
        {
            this.availableMoves = new List<Tuple<int, int>>();
            this.disabledMoves = new List<Tuple<int, int>>();

            this.availableMoves.Add(new Tuple<int, int>(this.width / 2, this.height / 2));

            int x = (this.width / 2) - 1;
            int y = (this.height / 2) - 1;
            int xAmp = 2;
            int yAmp = 2;

            while (this.availableMoves.Count != this.height * this.width)
            {
                for (int xi = 0; xi < xAmp; xi++)
                {
                    this.availableMoves.Add(new Tuple<int, int>(x, y));
                    x++;
                }

                for (int yi = 0; yi < yAmp; yi++)
                {
                    this.availableMoves.Add(new Tuple<int, int>(x, y));
                    y++;
                }

                for (int xi = 0; xi < xAmp; xi++)
                {
                    this.availableMoves.Add(new Tuple<int, int>(x, y));
                    x--;
                }

                for (int yi = 0; yi < yAmp; yi++)
                {
                    this.availableMoves.Add(new Tuple<int, int>(x, y));
                    y--;
                }

                if (xAmp < this.width)
                {
                    xAmp += 2;
                    x--;
                }

                if (yAmp < this.height)
                {
                    yAmp += 2;
                    y--;
                }
            }
        }

        private bool IsAlone(int x, int y)
        {
            return !(from adjacentCell in this.adjacentCells let newX = x + adjacentCell.Item1 let newY = y + adjacentCell.Item2 where newX >= 0 && newX < this.width && newY >= 0 && newY < this.height && this.board[newY, newX] != 0 select newX).Any();
        }

        private void ApplyMove(Tuple<int, int> move, int player)
        {
            this.board[move.Item2, move.Item1] = player;
            this.disabledMoves.Add(move);
            this.availableMoves.Remove(move);
        }

        private void UnapplyMove()
        {
            Tuple<int, int> move = this.disabledMoves.Last();

            this.board[move.Item2, move.Item1] = 0;
            this.availableMoves.Add(move);
            this.disabledMoves.Remove(move);
        }

        private bool IsFree(int x, int y)
        {
            return x >= 0 && y >= 0 && x < this.width && y < this.height && this.board[y, x] == 0;
        }
    }
}