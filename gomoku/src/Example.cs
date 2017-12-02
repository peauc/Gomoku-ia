using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Gomoku;

internal class GomocupEngine : GomocupInterface
{
    public const int MAX_BOARD = 100;

    private readonly int[,] board = new int[MAX_BOARD, MAX_BOARD];

    private HeuristicAnalysis heuristicAnalysis;

    private List<Tuple<int, int>> movesAvailable;

    private List<Tuple<int, int>> movesUnavailable;

    public override string brain_about
    {
        get
        {
            return "name=\"DeepMind\", author=\"Peau_c Samuel_r\", version=\"-1\", country=\"France\", www=\"www.epitech.eu\"";
        }
    }

    public override void brain_init()
    {
        if (this.width < 5 || this.height < 5)
        {
            Console.WriteLine("ERROR size of the board");
            return;
        }
        if (this.width > MAX_BOARD || this.height > MAX_BOARD)
        {
            Console.WriteLine("ERROR Maximal board size is " + MAX_BOARD);
            return;
        }
        this.InitMoves();
        this.heuristicAnalysis = new HeuristicAnalysis(this.width, this.height);
        Console.WriteLine("OK");
    }

    public override void brain_restart()
    {
        for (int x = 0; x < this.width; x++)
            for (int y = 0; y < this.height; y++) this.board[y, x] = 0;

        this.InitMoves();
        this.heuristicAnalysis = new HeuristicAnalysis(this.width, this.height);

        Console.WriteLine("OK");
    }

    private bool IsFree(int x, int y)
    {
        return x >= 0 && y >= 0 && x < this.width && y < this.height && this.board[y, x] == 0;
    }

    public override void brain_my(int x, int y)
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

    public override void brain_opponents(int x, int y)
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

    //public override void brain_moves()
    //{
    //    Console.WriteLine("Displaying available moves :");
    //    this.DisplayMoves(this.movesAvailable);
    //    Console.WriteLine();
    //    Console.WriteLine("Displaying unavailable moves :");
    //    this.DisplayMoves(this.movesUnavailable);
    //    Console.WriteLine();
    //}

    public override void brain_turn()
    {
        Tuple<int, int> bestMove = this.AlphaBeta(1, int.MinValue, int.MaxValue, true, -1);
        if (bestMove.Item2 != -1)
        {
            do_mymove(this.movesAvailable[bestMove.Item2].Item1, this.movesAvailable[bestMove.Item2].Item2);
        }
    }

    public override void brain_end()
    {
    }

    public override void brain_eval(int x, int y)
    {
    }

    //public override void brain_display()
    //{
    //    for (int y = 0; y < this.height; y++)
    //    {
    //        for (int x = 0; x < this.width; x++)
    //        {
    //            Console.Write(this.board[y, x]);
    //        }
    //        Console.WriteLine();
    //    }
    //}

    private Tuple<int, int> AlphaBeta(int depth, int alpha, int beta, bool maximizingPlayer, int moveIndex)
    {
        if (depth == 0 || this.movesAvailable.Count == 0)
        {
            return new Tuple<int, int>(this.heuristicAnalysis.Compute(this.board), moveIndex);
        }

        if (maximizingPlayer)
        {
            Tuple<int, int> max = new Tuple<int, int>(int.MinValue, 0);
            for (int i = 0; i < this.movesAvailable.Count; i++)
            {
                Tuple<int, int> move = this.movesAvailable[0];
                this.ApplyMove(this.movesAvailable[0], 1);
                Tuple<int, int> ret = this.AlphaBeta(depth - 1, alpha, beta, false, i);
                //if (depth == 3)
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
                this.ApplyMove(this.movesAvailable[0], 1);
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

    private void InitMoves()
    {
        this.movesAvailable = new List<Tuple<int, int>>();
        this.movesUnavailable = new List<Tuple<int, int>>();

        for (int y = 0; y < this.height; y++)
        {
            for (int x = 0; x < this.width; x++)
            {
                if (this.board[y, x] == 0)
                {
                    this.movesAvailable.Add(new Tuple<int, int>(x, y));
                }
            }
        }
    }
}