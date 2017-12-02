// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PisqPipe.cs" company="Epitech">
//
// </copyright>
// <summary>
//   The gomocup interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gomoku
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    /// <summary>
    /// The gomocup interface.
    /// </summary>
    public abstract class GomocupInterface
    {
        /// <summary>
        /// The width.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "More perf")]
        public int Width;

        /// <summary>
        /// The height.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "More perf than setter")]
        public int Height;

        /// <summary>
        /// The info timeout turn.
        /// </summary>
        private int infoTimeoutTurn = 30000; /* time for one turn in milliseconds */

        /// <summary>
        /// The infoTimeoutMatch.
        /// </summary>
        private int infoTimeoutMatch = 1000000000; /* total time for a game */

        /// <summary>
        /// The infoTimeLeft.
        /// </summary>
        private int infoTimeLeft = 1000000000; /* left time for a game */

        /// <summary>
        /// The infoMaxMemory.
        /// </summary>
        private int infoMaxMemory = 0; /* maximum memory in bytes, zero if unlimited */

        /// <summary>
        /// The infoGameType.
        /// </summary>
        private int infoGameType = 1; /* 0:human opponent, 1:AI opponent, 2:tournament, 3:network tournament */

        /// <summary>
        /// The info exact 5.
        /// </summary>
        private bool infoExact5 = false; /* false:five or more stones win, true:exactly five stones win */

        /// <summary>
        /// The info renju.
        /// </summary>
        private bool infoRenju = false; /* false:gomoku, true:renju */

        /// <summary>
        /// The info continuous.
        /// </summary>
        private bool infoContinuous = false; /* false:single game, true:continuous */

        /// <summary>
        /// The terminate.
        /// </summary>
        private int terminate; /* return from BrainTurn when terminate>0 */

        /// <summary>
        /// The start time.
        /// </summary>
        private int startTime; /* tick count at the beginning of turn */

        /// <summary>
        /// The data folder.
        /// </summary>
        private string dataFolder; /* folder for persistent files, can be null */

        /// <summary>
        /// The cmd.
        /// </summary>
        private string cmd;

        /// <summary>
        /// The event 1.
        /// </summary>
        private AutoResetEvent event1;

        /// <summary>
        /// The event 2.
        /// </summary>
        private ManualResetEvent event2;

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            new GomocupEngine().ProgramStart();
        }

        /// <summary>
        /// The GetCmdParam.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="param">
        /// The param.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetCmdParam(string command, out string param)
        {
            param = string.Empty;
            var pos = command.IndexOf(' ');
            if (pos < 0)
            {
                return command.ToLower();
            }

            param = command.Substring(pos + 1).TrimStart(' ');
            command = command.Substring(0, pos);
            return command.ToLower();
        }

        /// <summary>
        /// Gets the BrainAbout.
        /// </summary>
        public abstract string BrainAbout { get; } /* copyright, version, homepage */

        /// <summary>
        /// The BrainInit.
        /// </summary>
        public abstract void BrainInit(); /* create the board and call Console.WriteLine("OK"); or Console.WriteLine("ERROR Maximal board size is .."); */

        /// <summary>
        /// The brain restart.
        /// </summary>
        public abstract void BrainRestart(); /* delete old board, create new board, call Console.WriteLine("OK"); */

        /// <summary>
        /// The brain turn.
        /// </summary>
        public abstract void BrainTurn(); /* choose your move and call do_mymove(x,y); 0<=x<width, 0<=y<height */

        /// <summary>
        /// The brain my.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        public abstract void BrainMy(int x, int y); /* put your move to the board */

        /// <summary>
        /// The brain opponents.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        public abstract void BrainOpponents(int x, int y); /* put opponent's move to the board */

        /// <summary>
        /// The brain block.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        public abstract void BrainBlock(int x, int y); /* square [x,y] belongs to a winning line (when info_continuous is 1) */

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
        /// The <see cref="int"/>.
        /// </returns>
        public abstract int BrainTakeback(int x, int y); /* clear one square; return value: 0:success, 1:not supported, 2:error */

        /// <summary>
        /// The brain end.
        /// </summary>
        public abstract void BrainEnd();  /* delete temporary files, free resources */

        /// <summary>
        /// The brain eval.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        public virtual void BrainEval(int x, int y)
        {
        } /* display evaluation of square [x,y] */

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            this.startTime = Environment.TickCount;
            this.Stop();
            if (this.Width == 0)
            {
                this.Width = this.Height = 20;
                this.BrainInit();
            }
        }

        /// <summary>
        ///  send suggest
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        protected void Suggest(int x, int y)
        {
            Console.WriteLine("SUGGEST {0},{1}", x, y);
        }

        /// <summary>
        /// write move to the pipe and update internal data structures .
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        protected void DoMymove(int x, int y)
        {
            this.BrainMy(x, y);
            Console.WriteLine("{0},{1}", x, y);
        }

        /// <summary>
        ///  main function for AI console application.
        /// </summary>
        protected void ProgramStart()
        {
            try
            {
                int dummy = Console.WindowHeight;
                //ERROR, process started from the Explorer or command line
                Console.WriteLine("MESSAGE Gomoku AI should not be started directly. Please install gomoku manager (http://sourceforge.net/projects/piskvork). Then enter path to this exe file in players settings.");
            }
            catch (System.IO.IOException)
            {
                //OK, process started from the Piskvork manager
            }

            this.event1 = new AutoResetEvent(false);
            new Thread(this.ThreadLoop).Start();
            this.event2 = new ManualResetEvent(true);
            for (; ; )
            {
                this.GetLine();
                this.DoCommand();
            }
        }

        /// <summary>
        /// The do command.
        /// </summary>
        private void DoCommand()
        {
            string param, info;
            int x, y, who, e;

            switch (GetCmdParam(this.cmd, out param))
            {
                case "info":
                    switch (GetCmdParam(param, out info))
                    {
                        case "max_memory":
                            int.TryParse(info, out this.infoMaxMemory);
                            break;

                        case "timeout_match":
                            int.TryParse(info, out this.infoTimeoutMatch);
                            break;

                        case "timeout_turn":
                            int.TryParse(info, out this.infoTimeoutTurn);
                            break;

                        case "time_left":
                            int.TryParse(info, out this.infoTimeLeft);
                            break;

                        case "game_type":
                            int.TryParse(info, out this.infoGameType);
                            break;

                        case "rule":
                            if (int.TryParse(info, out e))
                            {
                                this.infoExact5 = (e & 1) != 0;
                                this.infoContinuous = (e & 2) != 0;
                                this.infoRenju = (e & 4) != 0;
                            }
                            break;

                        case "folder":
                            this.dataFolder = info;
                            break;

                        case "evaluate":
                            if (this.ParseCoord(info, out x, out y))
                            {
                                this.BrainEval(x, y);
                            }

                            break;
                            /* unknown info is ignored */
                    }
                    break;

                case "start":
                    if (!int.TryParse(param, out Width) || this.Width < 5)
                    {
                        this.Width = 0;
                        Console.WriteLine("ERROR bad START parameter");
                    }
                    else
                    {
                        this.Height = this.Width;
                        this.Start();
                        this.BrainInit();
                    }
                    break;

                case "rectstart":
                    if (!this.ParseCoord2(param, out Width, out Height) || this.Width < 5 || this.Height < 5)
                    {
                        this.Width = this.Height = 0;
                        Console.WriteLine("ERROR bad RECTSTART parameters");
                    }
                    else
                    {
                        this.Start();
                        this.BrainInit();
                    }
                    break;

                case "restart":
                    this.Start();
                    this.BrainRestart();
                    break;

                case "turn":
                    this.Start();
                    if (!this.ParseCoord(param, out x, out y))
                    {
                        Console.WriteLine("ERROR bad coordinates");
                    }
                    else
                    {
                        this.BrainOpponents(x, y);
                        this.Turn();
                    }
                    break;

                case "play":
                    this.Start();
                    if (!this.ParseCoord(param, out x, out y))
                    {
                        Console.WriteLine("ERROR bad coordinates");
                    }
                    else
                    {
                        this.DoMymove(x, y);
                    }
                    break;

                case "begin":
                    this.Start();
                    this.Turn();
                    break;

                case "about":
                    Console.WriteLine(this.BrainAbout);
                    break;

                case "end":
                    this.Stop();
                    this.BrainEnd();
                    Environment.Exit(0);
                    break;

                case "board":
                    this.Start();
                    for (; ; )
                    {
                        /* fill the whole board */
                        this.GetLine();
                        this.ParseThreeIntChk(this.cmd, out x, out y, out who);
                        switch (who)
                        {
                            case 1:
                                this.BrainMy(x, y);
                                break;

                            case 2:
                                this.BrainOpponents(x, y);
                                break;

                            case 3:
                                this.BrainBlock(x, y);
                                break;

                            default:
                                if (!this.cmd.Equals("done", StringComparison.InvariantCultureIgnoreCase))
                                    Console.WriteLine("ERROR x,y,who or DONE expected after BOARD");
                                break;
                        }
                        if (who > 3) break;
                    }
                    this.Turn();
                    break;

                case "takeback":
                    this.Start();
                    var t = "ERROR bad coordinates";
                    if (this.ParseCoord(param, out x, out y))
                    {
                        e = this.BrainTakeback(x, y);
                        if (e == 0)
                        {
                            t = "OK";
                        }
                        else if (e == 1)
                        {
                            t = "UNKNOWN";
                        }
                    }
                    Console.WriteLine(t);
                    break;

                default:
                    Console.WriteLine("UNKNOWN command");
                    break;
            }
        }

        /// <summary>
        /// main function for the working thread
        /// </summary>
        private void ThreadLoop()
        {
            for (; ; )
            {
                this.event1.WaitOne();
                this.BrainTurn();
                this.event2.Set();
            }
        }

        /// <summary>
        /// start thinking
        /// </summary>
        private void Turn()
        {
            this.terminate = 0;
            this.event2.Reset();
            this.event1.Set();
        }

        /// <summary>
        /// stop thinking
        /// </summary>
        private void Stop()
        {
            this.terminate = 1;
            this.event2.WaitOne();
        }

        /// <summary>
        /// Read a line from std.
        /// </summary>
        private void GetLine()
        {
            this.cmd = Console.ReadLine();
            if (this.cmd == null) Environment.Exit(0);
        }

        /// <summary>
        /// parse coordinates x,y
        /// </summary>
        /// <param name="param">
        /// The param.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool ParseCoord2(string param, out int x, out int y)
        {
            string[] p = param.Split(',');
            if (p.Length == 2 && int.TryParse(p[0], out x) && int.TryParse(p[1], out y) && x >= 0 && y >= 0)
                return true;
            x = y = 0;
            return false;
        }

        /// <summary>
        /// parse coordinates x,y.
        /// </summary>
        /// <param name="param">
        /// The param.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool ParseCoord(string param, out int x, out int y)
        {
            return this.ParseCoord2(param, out x, out y) && x < this.Width && y < this.Height;
        }

        /** parse coordinates x,y and player number z */

        /// <summary>
        /// The parse_3 int_chk.
        /// </summary>
        /// <param name="param">
        /// The param.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="z">
        /// The z.
        /// </param>
        private void ParseThreeIntChk(string param, out int x, out int y, out int z)
        {
            var p = param.Split(',');
            if (!(p.Length == 3 && int.TryParse(p[0], out x) && int.TryParse(p[1], out y) && int.TryParse(p[2], out z)
                  && x >= 0 && y >= 0 && x < this.Width && y < this.Height))
            {
                x = y = z = 0;
            }
        }

        /** return pointer to word after command if input starts with command, otherwise return NULL */
    }
}