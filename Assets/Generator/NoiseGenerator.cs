using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
	// Start is called before the first frame update
	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
	{
		float[,] noiseMap = new float[mapWidth, mapHeight];

		System.Random prng = new System.Random(seed);
		Vector2[] octaveOffsets = new Vector2[octaves];
		for (int i = 0; i < octaves; i++)
		{
			float offsetX = prng.Next(-100000, 100000) + offset.x;
			float offsetY = prng.Next(-100000, 100000) + offset.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);
		}

		if (scale <= 0)
		{
			scale = 0.0001f;
		}

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;


		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
            {
				Vector2 warp = new Vector2(0.0f, 0.0f);
				float firstHeight = FBM(scale, octaves, persistance, lacunarity, octaveOffsets, ref maxNoiseHeight, ref minNoiseHeight, halfWidth, halfHeight, y, x, warp);
				warp = new Vector2(5.2f, 1.3f);
				float secondHeight = FBM(scale, octaves, persistance, lacunarity, octaveOffsets, ref maxNoiseHeight, ref minNoiseHeight, halfWidth, halfHeight, y, x, warp);
				warp = new Vector2(4.0f * firstHeight, 4.0f * secondHeight);
				warp = new Vector2(0.0f, 0.0f);
				float noiseHeight = FBM(scale, octaves, persistance, lacunarity, octaveOffsets, ref maxNoiseHeight, ref minNoiseHeight, halfWidth, halfHeight, y, x, warp);
                noiseMap[x, y] = noiseHeight;
            }
        }

		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
			}

		}

		return noiseMap;
	}

    private static float FBM(float scale, int octaves, float persistance, float lacunarity, Vector2[] octaveOffsets, ref float maxNoiseHeight, ref float minNoiseHeight, float halfWidth, float halfHeight, int y, int x, Vector2 warp)
    {
        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        for (int i = 0; i < octaves; i++)
        {
            float sampleX = ((x - halfWidth) + warp[0]) / scale * frequency + octaveOffsets[i].x;
            float sampleY = ((y - halfHeight) + warp[1]) / scale * frequency + octaveOffsets[i].y;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
            noiseHeight += perlinValue * amplitude;

            amplitude *= persistance;
            frequency *= lacunarity;
        }

        if (noiseHeight > maxNoiseHeight)
        {
            maxNoiseHeight = noiseHeight;
        }
        else if (noiseHeight < minNoiseHeight)
        {
            minNoiseHeight = noiseHeight;
        }

        return noiseHeight;
    }
}
