namespace Minesweeper
{
    public class Tile
    {
        public bool HasBomb { get; set; }
        public bool IsRevealed { get; set; }
        public bool IsFlagged { get; set; }
        public int AdjacentBombCount { get; set; }
        

        public Tile(bool hasBomb)
        {
            HasBomb = hasBomb;
            IsRevealed = false;
            AdjacentBombCount = 0;
            IsFlagged = false;
        }
    }
}
