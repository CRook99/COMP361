using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace World
{
    public class MapParser : MonoBehaviour
    {
        [SerializeField] private List<Layer> layers;

        private HashSet<Vector2Int> GetMapCells(Texture2D image)
        {
            HashSet<Vector2Int> mapCells = new HashSet<Vector2Int>();
            
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
        
        public void ReadMap(out Dictionary<Vector2Int, Cell> cellMap, out Dictionary<Vector2Int, Cell> coveredCells)
        {
            cellMap = new Dictionary<Vector2Int, Cell>();
            coveredCells = new Dictionary<Vector2Int, Cell>();
    
            HashSet<Vector2Int> cells = GetMapCells(layers[0].Image);
    
            foreach (Vector2Int cell in cells)
            {
                cellMap.Add(cell, new Cell(cell));
            }
    
            int numLayers = layers.Count;
            for (int i = 1; i < numLayers; i++)
            {
                CoverTypes cover = layers[i].Cover;
                Texture2D image = layers[i].Image;
                
                foreach (Vector2Int cell in GetMapCells(image))
                {
                    cellMap[cell].Walkable = false;
                    cellMap[cell].Cover = cover;
                    coveredCells.Add(cell, cellMap[cell]);
                }
            }
        }
        
        [ContextMenu("Generate map")]
        public void GenerateMap()
        {
            ClearMap();

            for (int i = 0; i < layers.Count; i++)
            {
                GameObject layer = new GameObject("Layer_" + i);
                layer.transform.parent = transform;
                
                foreach (Vector2Int cell in GetMapCells(layers[i].Image))
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(layers[i].Object, layer.transform);
                    obj.transform.position = new Vector3(cell.x, layers[i].Elevation, cell.y);
                }
            }
        }

        [ContextMenu("Clear map")]
        public void ClearMap()
        {
            List<GameObject> cells = new List<GameObject>();
            GatherChildren(transform, cells);

            for (int i = cells.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(cells[i]);
            }

            void GatherChildren(Transform parent, List<GameObject> children)
            {
                foreach (Transform child in parent)
                {
                    children.Add(child.gameObject);
                    GatherChildren(child, children);
                }
            }
        }
        
        [System.Serializable]
        public class Layer
        {
            public GameObject Object;
            public Texture2D Image;
            public int Elevation;
            public CoverTypes Cover;
        }
    }
}
