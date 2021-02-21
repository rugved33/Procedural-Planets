using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace
{
    ShapeGenerator shapeGenerator;
    Mesh mesh;

    // Number vertices along 1 edge of a face
    int resolution;

    // Local axes for a face
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;

    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh,int resolution, Vector3 localUp)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 2 * 3];
        int triIndex = 0;
        Vector2[] uv = (mesh.uv.Length == vertices.Length) ? mesh.uv : new Vector2[vertices.Length];

        // Iterate through vertices
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                // Calculate current vertex index
                int i = x + (y * resolution);

                // Calculate percentage of how far along on each side of the face we have moved
                Vector2 percent = new Vector2(x, y) / (resolution - 1);

                // Calculate coordinates of vertex on the cube in parent space
                Vector3 pointOnUnitCube = localUp + ((percent.x - 0.5f) * 2 * axisA) + ((percent.y - 0.5f) * 2 * axisB);

                // Calculate point as projected on a sphere (i.e. equal distance from center of sphere)
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                // Set vertex coordinates at index
                float unscaledElevation = shapeGenerator.CalculateUnscaledElevation(pointOnUnitSphere);
                vertices[i] = pointOnUnitSphere * shapeGenerator.GetScaledElevation(unscaledElevation);
                uv[i].y = unscaledElevation;

                // Only create triangles if current vertex does not lie on far extreme edges of the face
                if (x != resolution - 1 && y != resolution -1)
                {
                    // Triangle 1 with vertices in clockwise direction
                    triangles[triIndex++] = i;
                    triangles[triIndex++] = i + resolution + 1;
                    triangles[triIndex++] = i + resolution;

                    // Triangle 2
                    triangles[triIndex++] = i;
                    triangles[triIndex++] = i + 1;
                    triangles[triIndex++] = i + resolution + 1;
                }
            }
        }

        // Generate the mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv = uv;
    }

    public void UpdateUVs(ColorGenerator colorGenerator)
    {
        Vector2[] uv = mesh.uv;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + (y * resolution);
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + ((percent.x - 0.5f) * 2 * axisA) + ((percent.y - 0.5f) * 2 * axisB);
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                uv[i].x = colorGenerator.BiomePercentFromPoint(pointOnUnitSphere);
            }
        }

        mesh.uv = uv;
    }
}
