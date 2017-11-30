namespace gomoku
{
    public struct Tile
    {
        public bool IsOccupied { get; private set; }
        public Player Player { get; set; }

        public void SetOccupancy(Player playerNumber)
        {
            IsOccupied = true;
            Player = playerNumber;
        }
    }
}