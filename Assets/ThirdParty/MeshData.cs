using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;

    public Vector2[] uvs;
    int triangleIndex;
    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];

        triangles = new int[(meshWidth - 1) * (meshHeight - 1)* 6];

    }
    public void AddTriangle(int vert1, int vert2, int vert3)
    {
        triangles[triangleIndex] = vert1;
        triangles[triangleIndex + 1] = vert2;
        triangles[triangleIndex + 2] = vert3;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;

    }
}
