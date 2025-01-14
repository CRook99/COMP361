using UnityEngine;

public class Cell
{
    public Vector2Int Position { get; private set; }
    public bool Walkable { get; set; }

    public Cell(Vector2Int position, bool walkable = true)
    {
        Position = position;
        Walkable = walkable;
    }
}


