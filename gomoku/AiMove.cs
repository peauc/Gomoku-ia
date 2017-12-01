namespace Gomoku
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AiMove : Move
    {
        public AiMove(uint x, uint y)
            : base(x, y)
        {
        }

        public int HeuristicScore { get; set; }

        public void ComputeHeuristicScore(IHeuristicAnalysis ha, GameState state)
        {
            throw new NotImplementedException();
        }
    }
}