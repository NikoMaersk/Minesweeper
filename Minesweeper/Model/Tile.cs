namespace Minesweeper
{
    public class Tile
    {
        public bool HasMine { get; set; }
        public bool IsRevealed { get; set; }
        public bool hasFlag { get; set; }
        public int AdjacentMineCount { get; set; }
        

        public Tile(bool hasMine)
        {
            HasMine = hasMine;
            IsRevealed = false;
            AdjacentMineCount = 0;
            hasFlag = false;
        }
    }
}
