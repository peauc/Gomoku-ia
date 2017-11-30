using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;

namespace gomoku
{
    public class Map
    {
        private Tile[,] TileList;
        private Point MapSize;
        public Map(int x, int y)
        {
            MapSize = new Point(x, y);
            TileList = new Tile[x, y];
        }

        public void Play(Play play)
        {
            //TileList[play.Move.Y, play.Move.X].Player;
        }
    }
}