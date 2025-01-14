using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapParser
{
    public List<Vector2Int> GetMapCells(Texture2D image)
    {
        List<Vector2Int> mapCells = new List<Vector2Int>();
        
        for (int y = 0; y < image.height; y++)
        {
            for (int x = 0; x < image.width; x++)
            {
                Color pixelColor = image.GetPixel(x, y);
                if (pixelColor != Color.white && pixelColor.a != 0)
                    mapCells.Add(new Vector2Int(x, y));
            }
        }

        return mapCells;
    }
}
