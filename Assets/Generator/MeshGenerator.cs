using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        int meshLevelOfDetail = (levelOfDetail == 0)?1: levelOfDetail * 2;
        int vertPerLine = (width - 1) / meshLevelOfDetail + 1;

        MeshData meshData = new MeshData(vertPerLine, vertPerLine);
        int vertexIndex = 0;

        for (int j = 0; j < height; j+= meshLevelOfDetail) 
        {
            for (int i = 0; i < width; i+= meshLevelOfDetail)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + i, heightCurve.Evaluate(heightMap[i,j]) * heightMultiplier, topLeftZ - j);
                meshData.uvs[vertexIndex] = new Vector2(i / (float)width, j / (float)height);

                if( i< width - 1 && j < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + vertPerLine + 1, vertexIndex + vertPerLine);
                    meshData.AddTriangle(vertexIndex + vertPerLine + 1, vertexIndex, vertexIndex + 1 );
                }

                vertexIndex++;
            }
        }

        return meshData;
    }
}
