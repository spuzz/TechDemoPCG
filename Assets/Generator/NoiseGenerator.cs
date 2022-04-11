using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, Vector2 mapOffset, NoiseSettings[] noiseSettings)
	{
		float[,] noiseMap = new float[mapWidth, mapHeight];

		MapSettings mapSettings = new MapSettings(mapWidth, mapHeight, seed, scale, mapOffset);

		int count = 0;
		foreach (NoiseSettings noiseSetting in noiseSettings)
        {
			count++;

			System.Random prng = new System.Random(seed);
            Vector2[] octaveOffsets = new Vector2[noiseSetting.octaves];

            Vector2 origOffset = new Vector2(noiseSetting.offset.x + mapSettings.mapOffset.x, noiseSetting.offset.y + mapSettings.mapOffset.y);   
            for (int i = 0; i < noiseSetting.octaves; i++)
            {
                float offsetX = prng.Next(-100000, 100000) + origOffset.x;
                float offsetY = prng.Next(-100000, 100000) - origOffset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            for (int y = 0; y < mapSettings.mapHeight; y++)
            {
                for (int x = 0; x < mapSettings.mapWidth; x++)
                {
					float sampleX = ((x - mapSettings.halfWidth + octaveOffsets[count].x)) / 20;
					float sampleY = ((y - mapSettings.halfHeight + octaveOffsets[count].y)) / 20;
					float layerImpact = Mathf.PerlinNoise(sampleX, sampleY);
					Vector2 warp = new Vector2(0.0f, 0.0f);
					float firstHeight = (FBM(x, y, mapSettings, noiseSetting, octaveOffsets, warp));
					warp = new Vector2(5.2f, 1.3f);
					float secondHeight = (FBM(x, y, mapSettings, noiseSetting, octaveOffsets, warp));
					warp = new Vector2(2.0f * firstHeight, 2.0f * secondHeight);
					//warp = new Vector2(0.0f, 0.0f);
					noiseMap[x, y] += (FBM(x + warp.x, y + warp.y, mapSettings, noiseSetting, octaveOffsets, warp) * layerImpact);
                }
            }
        }

        return noiseMap;
	}

	private static float FBM(float x, float y, MapSettings mapSettings, NoiseSettings noiseSettings, Vector2[] octaveOffsets, Vector2 warp)
	{
		float amplitude = 1;
		float frequency = 1;
		float noiseHeight = 0;

		for (int i = 0; i < noiseSettings.octaves; i++)
		{
			float sampleX = ((x - mapSettings.halfWidth + octaveOffsets[i].x)) / noiseSettings.scale * frequency;
			float sampleY = ((y - mapSettings.halfHeight + octaveOffsets[i].y)) / noiseSettings.scale * frequency;
            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
            float ridgedValue = 1.0f - Mathf.Abs(perlinValue);
            float billowvalue = perlinValue * perlinValue;

            float noiseValue = Mathf.Lerp(perlinValue, billowvalue, Mathf.Max(0.0f, noiseSettings.sharpness));

            noiseValue += Mathf.Lerp(perlinValue, ridgedValue, Mathf.Abs(Mathf.Min(0.0f, noiseSettings.sharpness)));
            noiseHeight += noiseValue * amplitude;

			amplitude *= noiseSettings.persistance;
			frequency *= noiseSettings.lacunarity;
		}

        noiseHeight = Mathf.Max(0, noiseHeight - noiseSettings.minValue);
		return noiseHeight * noiseSettings.strength;
	}


}
public class MapSettings
{
	public int mapWidth;
	public int mapHeight;
	public int seed;
	public float scale;
	public float halfHeight;
	public float halfWidth;
	public Vector2 mapOffset;

	public MapSettings(int mapWidth, int mapHeight, int seed, float scale, Vector2 mapOffset)
    {
        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        this.mapWidth = mapWidth;
		this.mapHeight = mapHeight;
		this.seed = seed;
		this.scale = scale;
		this.mapOffset = mapOffset;
		halfWidth = mapWidth / 2f;
		halfHeight = mapHeight / 2f;
	}
}