using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace World
{
    public class TacticsGrid : MonoBehaviour
    {
        [SerializeField] private List<Layer> layers;
        
        [ContextMenu("Generate map")]
        public void GenerateMap()
        {
            ClearMap();
            
            var parser = new MapParser();

            for (int i = 0; i < layers.Count; i++)
            {
                GameObject layer = new GameObject("Layer_" + i);
                layer.transform.parent = transform;
                
                foreach (Vector2Int cell in parser.GetMapCells(layers[i].Image))
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
    }

    [System.Serializable]
    public class Layer
    {
        public GameObject Object;
        public Texture2D Image;
        public int Elevation;
    }
}