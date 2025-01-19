using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace Kibo.Utilities
{
    public static class NavMeshUtility
    {
        private static NavMeshTriangulation triangulation;
        private static bool triangulationNeedsUpdate = true;

        public static bool TriangulationNeedsUpdate
        {
            get => triangulationNeedsUpdate;
            set => triangulationNeedsUpdate = triangulationNeedsUpdate || value;
        }

        public static Vector3? GetRandomPointInBounds(Bounds bounds)
        {
            if (TriangulationNeedsUpdate)
            {
                triangulation = NavMesh.CalculateTriangulation();
                triangulationNeedsUpdate = false;
            }

            List<Vector3[]> validTriangles = new();
            for (int i = 0; i < triangulation.indices.Length; i += 3)
            {
                Vector3 corner1 = triangulation.vertices[triangulation.indices[i]];
                Vector3 corner2 = triangulation.vertices[triangulation.indices[i + 1]];
                Vector3 corner3 = triangulation.vertices[triangulation.indices[i + 2]];

                if (!(bounds.Contains(corner1) && bounds.Contains(corner2) && bounds.Contains(corner3))) continue;

                validTriangles.Add(new[] { corner1, corner2, corner3 });
            }

            if (validTriangles.Count == 0) return null;

            int triangleIndex = Random.Range(0, validTriangles.Count);
            Vector3[] selectedTriangle = validTriangles[triangleIndex];

            return GetRandomPointInTriangle(selectedTriangle);
        }

        private static Vector3 GetRandomPointInTriangle(Vector3[] triangle)
        {
            Assert.AreEqual(3, triangle.Length, $"Given {nameof(triangle)} must have 3 elements");

            float scale1 = Random.value, scale2 = Random.value;
            if (scale1 + scale2 > 1f)
            {
                scale1 = 1f - scale1;
                scale2 = 1f - scale2;
            }

            Vector3 side1 = triangle[1] - triangle[0];
            Vector3 side2 = triangle[2] - triangle[0];

            return triangle[0] + scale1 * side1 + scale2 * side2;
        }
    }
}