using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Utility;

namespace World
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MoveArea : MonoBehaviour
    {
        private const float Offset = 0.5f;
        private const float YOffset = 0.05f;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        public void GenerateMesh(HashSet<Cell> cells, Vector2Int origin)
        {
            int numberOfQuads = cells.Count;

            List<Vector3> vertexBuffer = new List<Vector3>(numberOfQuads * 4);
            List<int> trisBuffer = new List<int>(numberOfQuads * 6);
            List<int> subBuffer = new List<int>(numberOfQuads * 6);

            foreach (Cell cell in cells)
            {
                var x = cell.Position.x - origin.x;
                var z = cell.Position.y - origin.y;
                vertexBuffer.Add(new Vector3(x - Offset, YOffset, z - Offset));
                vertexBuffer.Add(new Vector3(x - Offset, YOffset, z + Offset));
                vertexBuffer.Add(new Vector3(x + Offset, YOffset, z - Offset));
                vertexBuffer.Add(new Vector3(x + Offset, YOffset, z + Offset));
            }

            for (int i = 0; i < numberOfQuads; i++)
            {
                trisBuffer.Add(i * 4 + 0);
                trisBuffer.Add(i * 4 + 1);
                trisBuffer.Add(i * 4 + 2);
                trisBuffer.Add(i * 4 + 1);
                trisBuffer.Add(i * 4 + 3);
                trisBuffer.Add(i * 4 + 2);
                
                subBuffer.Add(i * 4 + 0);
                subBuffer.Add(i * 4 + 1);
                subBuffer.Add(i * 4 + 2);
                subBuffer.Add(i * 4 + 1);
                subBuffer.Add(i * 4 + 3);
                subBuffer.Add(i * 4 + 2);
            }

            Mesh mesh = new()
            {
                vertices = vertexBuffer.ToArray()
            };
            mesh.SetTriangles(trisBuffer, 0);
            mesh.RecalculateNormals();

            _meshFilter.mesh = mesh;
        }

        public void Show()
        {
            if (_meshRenderer == null) return;
            _meshRenderer.enabled = true;
        }

        public void Hide()
        {
            if (_meshRenderer == null) return;
            _meshRenderer.enabled = false;
        }
    }
}