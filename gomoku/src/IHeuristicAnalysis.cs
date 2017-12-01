using System.Collections.Generic;

namespace Gomoku
{
    public interface IHeuristicAnalysis
    {
        List<Pattern> Patterns { get; set; }

        int Compute(GameState state);
    }
}