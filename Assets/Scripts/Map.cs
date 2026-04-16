using System.Collections.Generic;

public enum TileType
{
    Empty,
    Wall,
    Diggable,
    Key,
    Gate,
    Goal
}

public class Map
{
    public List<string> mapData = new List<string>()
    {
        "KD..#..#..",
        "D.....#.D.",
        ".#..#.....",
        ".D..D.###D",
        "#####.#...",
        "....#.##B#",
        "....#.#...",
        "....#.#.G.",
        "#####.####",
        ".P....D.D."
    };
}