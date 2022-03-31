using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
	// Start is called before the first frame update
	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
	{
		float[,] noiseMap = new float[mapWidth, mapHeight];

		float maxPossibleHeight = 0;
		float amplitude = 1;

		System.Random prng = new System.Random(seed);
		Vector2[] octaveOffsets = new Vector2[octaves];

		float tmpAmplitude = 1;
		for (int i = 0; i < octaves; i++)
		{
			float offsetX = prng.Next(-100000, 100000) + offset.x;
			float offsetY = prng.Next(-100000, 100000) - offset.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);

			maxPossibleHeight += amplitude;
			maxPossibleHeight *= persistance;
		}

		if (scale <= 0)
		{
			scale = 0.0001f;
		}

		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;
		
		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
            {

				Vector2 warp = new Vector2(0.0f, 0.0f);
				float firstHeight = FBM(scale, octaves, persistance, lacunarity, octaveOffsets, halfWidth, halfHeight, y, x, warp);
				warp = new Vector2(5.2f, 1.3f);
				float secondHeight = FBM(scale, octaves, persistance, lacunarity, octaveOffsets, halfWidth, halfHeight, y, x, warp);
				warp = new Vector2(4.0f * firstHeight, 4.0f * secondHeight);
				//warp = new Vector2(0.0f, 0.0f);
				float noiseHeight = FBM(scale, octaves, persistance, lacunarity, octaveOffsets, halfWidth, halfHeight, y, x, warp);
                noiseMap[x, y] = noiseHeight;
            }
        }

		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				
				//noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
				float normalizedHeight = noiseMap[x, y]  /  maxPossibleHeight;
                //noiseMap[x, y] = Mathf.Clamp(normalizedHeight, -1, int.MaxValue);
				
                //noiseMap[x, y] = normalizedHeight;
            }

		}

		return noiseMap;
	}

    private static float FBM(float scale, int octaves, float persistance, float lacunarity, Vector2[] octaveOffsets, float halfWidth, float halfHeight, int y, int x, Vector2 warp)
    {
        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

		float xf = ((x - halfWidth)) / scale;
		float yf = ((y - halfHeight)) / scale;

		amplitude = Mathf.PerlinNoise(xf, yf) * 3;

		for (int i = 0; i < octaves; i++)
        {
            float sampleX = ((x - halfWidth + octaveOffsets[i].x) + warp[0]) / scale * frequency;
            float sampleY = ((y - halfHeight + octaveOffsets[i].y) + warp[1]) / scale * frequency;
			//float perlinValue = (Mathf.Abs(0.5f - Mathf.PerlinNoise(sampleX, sampleY)));
			float perlinValue = 0.5f - (Mathf.Abs(0.5f - Mathf.PerlinNoise(sampleX, sampleY)));
			//float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
            noiseHeight += perlinValue * amplitude;

            amplitude *= persistance;
            frequency *= lacunarity;
        }

        //if (noiseHeight > maxNoiseHeight)
        //{
        //    maxNoiseHeight = noiseHeight;
        //}
        //else if (noiseHeight < minNoiseHeight)
        //{
        //    minNoiseHeight = noiseHeight;
        //}

        return noiseHeight;
    }
}
