using System.Drawing;

namespace gomoku
{
    public struct Play
    {
        private Player Player;
        public Point Move { get; private set; }

        public Play(Player player, Point target)
        {
            Player = player;
            Move = target;
        }
    }
}