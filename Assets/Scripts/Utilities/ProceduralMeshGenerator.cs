using UnityEngine;

namespace NeonSurvivors.Utilities
{
    public static class ProceduralMeshGenerator
    {
        // ==================== MESH CREATION ====================

        public static Mesh CreateTriangle(float size)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Procedural_Triangle";

            float height = size * Mathf.Sqrt(3f) / 2f;

            Vector3[] vertices = new Vector3[]
            {
                new Vector3(0, 0, height * 2f / 3f),
                new Vector3(-size / 2f, 0, -height / 3f),
                new Vector3(size / 2f, 0, -height / 3f)
            };

            int[] triangles = new int[] { 0, 1, 2 };

            Vector3[] normals = new Vector3[]
            {
                Vector3.up, Vector3.up, Vector3.up
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh CreatePyramid(float size)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Procedural_Pyramid";

            float halfSize = size / 2f;

            Vector3[] vertices = new Vector3[]
            {
                // Base vertices
                new Vector3(-halfSize, 0, halfSize),
                new Vector3(halfSize, 0, halfSize),
                new Vector3(halfSize, 0, -halfSize),
                new Vector3(-halfSize, 0, -halfSize),
                // Top vertex
                new Vector3(0, size, 0)
            };

            int[] triangles = new int[]
            {
                // Base
                0, 1, 2,
                0, 2, 3,
                // Sides
                0, 4, 1,
                1, 4, 2,
                2, 4, 3,
                3, 4, 0
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh CreatePentagon(float size)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Procedural_Pentagon";

            int sides = 5;
            Vector3[] vertices = new Vector3[sides + 1];
            int[] triangles = new int[sides * 3];

            // Center vertex
            vertices[0] = Vector3.zero;

            // Perimeter vertices
            for (int i = 0; i < sides; i++)
            {
                float angle = i * (360f / sides) * Mathf.Deg2Rad;
                vertices[i + 1] = new Vector3(Mathf.Cos(angle) * size, 0, Mathf.Sin(angle) * size);

                // Create triangle
                int triIndex = i * 3;
                triangles[triIndex] = 0;
                triangles[triIndex + 1] = i + 1;
                triangles[triIndex + 2] = (i + 1) % sides + 1;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh CreateHexagon(float size)
        {
            return CreateRegularPolygon(6, size);
        }

        public static Mesh CreateStar(float size, int points = 5)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Procedural_Star";

            int vertexCount = points * 2;
            Vector3[] vertices = new Vector3[vertexCount + 1];
            int[] triangles = new int[points * 6];

            // Center vertex
            vertices[0] = Vector3.zero;

            float outerRadius = size;
            float innerRadius = size * 0.4f;

            // Create star vertices
            for (int i = 0; i < points; i++)
            {
                float angle = i * (360f / points) * Mathf.Deg2Rad;

                // Outer point
                vertices[i * 2 + 1] = new Vector3(
                    Mathf.Cos(angle) * outerRadius,
                    0,
                    Mathf.Sin(angle) * outerRadius
                );

                // Inner point
                float innerAngle = (i + 0.5f) * (360f / points) * Mathf.Deg2Rad;
                vertices[i * 2 + 2] = new Vector3(
                    Mathf.Cos(innerAngle) * innerRadius,
                    0,
                    Mathf.Sin(innerAngle) * innerRadius
                );
            }

            // Create triangles
            for (int i = 0; i < points; i++)
            {
                int triIndex = i * 6;
                int outerIndex = i * 2 + 1;
                int innerIndex = i * 2 + 2;
                int nextOuterIndex = ((i + 1) % points) * 2 + 1;

                // Triangle 1: center to outer to inner
                triangles[triIndex] = 0;
                triangles[triIndex + 1] = outerIndex;
                triangles[triIndex + 2] = innerIndex;

                // Triangle 2: center to inner to next outer
                triangles[triIndex + 3] = 0;
                triangles[triIndex + 4] = innerIndex;
                triangles[triIndex + 5] = nextOuterIndex;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh CreateArrow(float size)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Procedural_Arrow";

            float shaftWidth = size * 0.3f;
            float headWidth = size * 0.6f;
            float headLength = size * 0.4f;
            float shaftLength = size * 0.6f;

            Vector3[] vertices = new Vector3[]
            {
                // Arrow tip
                new Vector3(0, 0, size),
                // Arrow head sides
                new Vector3(-headWidth/2, 0, size - headLength),
                new Vector3(headWidth/2, 0, size - headLength),
                // Shaft top
                new Vector3(-shaftWidth/2, 0, size - headLength),
                new Vector3(shaftWidth/2, 0, size - headLength),
                // Shaft bottom
                new Vector3(-shaftWidth/2, 0, size - headLength - shaftLength),
                new Vector3(shaftWidth/2, 0, size - headLength - shaftLength)
            };

            int[] triangles = new int[]
            {
                // Arrow head
                0, 2, 1,
                // Arrow shaft
                3, 4, 5,
                5, 4, 6
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh CreateDiamond(float size)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Procedural_Diamond";

            float halfSize = size / 2f;

            Vector3[] vertices = new Vector3[]
            {
                // Top point
                new Vector3(0, size * 0.7f, 0),
                // Middle square
                new Vector3(-halfSize, 0, halfSize),
                new Vector3(halfSize, 0, halfSize),
                new Vector3(halfSize, 0, -halfSize),
                new Vector3(-halfSize, 0, -halfSize),
                // Bottom point
                new Vector3(0, -size * 0.3f, 0)
            };

            int[] triangles = new int[]
            {
                // Top pyramid
                0, 2, 1,
                0, 3, 2,
                0, 4, 3,
                0, 1, 4,
                // Bottom pyramid
                5, 1, 2,
                5, 2, 3,
                5, 3, 4,
                5, 4, 1
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh CreateOctahedron(float size)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Procedural_Octahedron";

            float halfSize = size / 2f;

            Vector3[] vertices = new Vector3[]
            {
                new Vector3(0, size, 0),        // Top
                new Vector3(-halfSize, 0, 0),   // Left
                new Vector3(0, 0, halfSize),    // Front
                new Vector3(halfSize, 0, 0),    // Right
                new Vector3(0, 0, -halfSize),   // Back
                new Vector3(0, -size, 0)        // Bottom
            };

            int[] triangles = new int[]
            {
                // Top pyramid
                0, 2, 1,
                0, 3, 2,
                0, 4, 3,
                0, 1, 4,
                // Bottom pyramid
                5, 1, 2,
                5, 2, 3,
                5, 3, 4,
                5, 4, 1
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh CreateIcosahedron(float size)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Procedural_Icosahedron";

            float t = (1f + Mathf.Sqrt(5f)) / 2f;
            float scale = size / Mathf.Sqrt(1 + t * t);

            Vector3[] vertices = new Vector3[]
            {
                new Vector3(-1, t, 0) * scale,
                new Vector3(1, t, 0) * scale,
                new Vector3(-1, -t, 0) * scale,
                new Vector3(1, -t, 0) * scale,
                new Vector3(0, -1, t) * scale,
                new Vector3(0, 1, t) * scale,
                new Vector3(0, -1, -t) * scale,
                new Vector3(0, 1, -t) * scale,
                new Vector3(t, 0, -1) * scale,
                new Vector3(t, 0, 1) * scale,
                new Vector3(-t, 0, -1) * scale,
                new Vector3(-t, 0, 1) * scale
            };

            int[] triangles = new int[]
            {
                0, 11, 5, 0, 5, 1, 0, 1, 7, 0, 7, 10, 0, 10, 11,
                1, 5, 9, 5, 11, 4, 11, 10, 2, 10, 7, 6, 7, 1, 8,
                3, 9, 4, 3, 4, 2, 3, 2, 6, 3, 6, 8, 3, 8, 9,
                4, 9, 5, 2, 4, 11, 6, 2, 10, 8, 6, 7, 9, 8, 1
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh CreateRegularPolygon(int sides, float size)
        {
            Mesh mesh = new Mesh();
            mesh.name = $"Procedural_Polygon_{sides}";

            Vector3[] vertices = new Vector3[sides + 1];
            int[] triangles = new int[sides * 3];

            vertices[0] = Vector3.zero;

            for (int i = 0; i < sides; i++)
            {
                float angle = i * (360f / sides) * Mathf.Deg2Rad;
                vertices[i + 1] = new Vector3(Mathf.Cos(angle) * size, 0, Mathf.Sin(angle) * size);

                int triIndex = i * 3;
                triangles[triIndex] = 0;
                triangles[triIndex + 1] = i + 1;
                triangles[triIndex + 2] = (i + 1) % sides + 1;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        // ==================== MATERIAL CREATION ====================

        public static Material CreateNeonMaterial(Color baseColor, float emissionIntensity = 2f)
        {
            // Try to use URP/Lit shader, fallback to Standard
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
                shader = Shader.Find("Standard");

            Material mat = new Material(shader);

            mat.SetColor("_BaseColor", baseColor);
            mat.SetColor("_Color", baseColor);

            // Enable emission
            mat.EnableKeyword("_EMISSION");
            Color emissionColor = baseColor * emissionIntensity;
            mat.SetColor("_EmissionColor", emissionColor);

            // Set metallic and smoothness for shiny look
            mat.SetFloat("_Metallic", 0.2f);
            mat.SetFloat("_Smoothness", 0.8f);

            return mat;
        }

        public static Material CreateUnlitNeonMaterial(Color baseColor)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
                shader = Shader.Find("Unlit/Color");

            Material mat = new Material(shader);
            mat.SetColor("_BaseColor", baseColor);
            mat.SetColor("_Color", baseColor);

            return mat;
        }

        // ==================== UTILITY ====================

        public static void ApplyMeshToGameObject(GameObject obj, Mesh mesh, Material material)
        {
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter == null)
                meshFilter = obj.AddComponent<MeshFilter>();

            MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                meshRenderer = obj.AddComponent<MeshRenderer>();

            meshFilter.mesh = mesh;
            meshRenderer.material = material;
        }

        public static GameObject CreateProceduralGameObject(string name, Mesh mesh, Material material, Vector3 position)
        {
            GameObject obj = new GameObject(name);
            obj.transform.position = position;

            ApplyMeshToGameObject(obj, mesh, material);

            return obj;
        }
    }
}
