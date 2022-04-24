using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, float minValue, Vector2 mapOffset, NoiseSettings[] noiseSettings)
	{
		float[,] noiseMap = new float[mapWidth, mapHeight];

		MapSettings mapSettings = new MapSettings(mapWidth, mapHeight, seed, scale, minValue, mapOffset);

		int count = 0;
		foreach (NoiseSettings noiseSetting in noiseSettings)
        {
			if(noiseSetting.enabled == true)
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
						float firstHeight = (FBM(x + noiseSetting.warp1.x, y + noiseSetting.warp1.y, mapSettings, noiseSetting, octaveOffsets));
						float secondHeight = (FBM(x + noiseSetting.warp2.x, y + noiseSetting.warp2.y, mapSettings, noiseSetting, octaveOffsets));
						Vector2 warp = new Vector2(noiseSetting.warpScale * firstHeight, noiseSetting.warpScale * secondHeight);
						noiseMap[x, y] += (FBM(x + warp.x, y + warp.y, mapSettings, noiseSetting, octaveOffsets) * layerImpact);
					}
				}
			}

        }

        return noiseMap;
	}

	private static float FBM(float x, float y, MapSettings mapSettings, NoiseSettings noiseSettings, Vector2[] octaveOffsets)
	{
		float amplitude = 1;
		float frequency = 1;
		float noiseHeight = 0;

		for (int i = 0; i < noiseSettings.octaves; i++)
		{
			float sampleX = ((x - mapSettings.halfWidth + octaveOffsets[i].x)) / (noiseSettings.scale + mapSettings.scale) * frequency;
			float sampleY = ((y - mapSettings.halfHeight + octaveOffsets[i].y)) / (noiseSettings.scale + mapSettings.scale) * frequency;
            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
            float ridgedValue = 1.0f - Mathf.Abs(perlinValue);
            float billowvalue = perlinValue * perlinValue;

            float noiseValue = Mathf.Lerp(perlinValue, billowvalue, Mathf.Max(0.0f, noiseSettings.sharpness));

            noiseValue += Mathf.Lerp(perlinValue, ridgedValue, Mathf.Abs(Mathf.Min(0.0f, noiseSettings.sharpness)));
            noiseHeight += noiseValue * amplitude;

			amplitude *= noiseSettings.persistance;
			frequency *= noiseSettings.lacunarity;
		}
		noiseHeight *= noiseSettings.strength;
	    noiseHeight = Mathf.Max(0, noiseHeight - (noiseSettings.minValue + mapSettings.minValue));
		return noiseHeight;
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
	public float minValue;
	public Vector2 mapOffset;

	public MapSettings(int mapWidth, int mapHeight, int seed, float scale, float minValue, Vector2 mapOffset)
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
		this.minValue = minValue;
		halfWidth = mapWidth / 2f;
		halfHeight = mapHeight / 2f;
	}
}