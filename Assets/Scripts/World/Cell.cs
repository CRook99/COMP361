using System;
using Entities;
using UnityEngine;

public class Cell
{
  public Vector2Int Position { get; private set; }
  public bool Walkable { get; set; }
  public CoverTypes Cover { get; set; }

  // Neighbours are defined clockwise, from N
  // 0: N, 1: NE, 2: E etc.
  public Cell[] Neighbours { get; set; }

  public Cell N => Neighbours[0];
  public Cell NE => Neighbours[1];
  public Cell E => Neighbours[2];
  public Cell SE => Neighbours[3];
  public Cell S => Neighbours[4];
  public Cell SW => Neighbours[5];
  public Cell W => Neighbours[6];
  public Cell NW => Neighbours[7];

  public Cell(Vector2Int position, bool walkable = true)
  {
    Position = position;
    Walkable = walkable;
    Cover = CoverTypes.NoCover;

    Neighbours = new Cell[8];
  }

  public override string ToString()
  {
    return $"Cell ({Position.x}, {Position.y}) : Walkable = {Walkable} // Cover = {Cover}";
  }
}


