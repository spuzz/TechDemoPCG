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

		// Calculate final height from each noise level
		foreach (NoiseSettings noiseSetting in noiseSettings)
        {
			if(noiseSetting.enabled == true)
            {
				count++;

				// randomise noise location based on input seed
				System.Random prng = new System.Random(seed);

				Vector2[] octaveOffsets = new Vector2[noiseSetting.octaves];

				Vector2 origOffset = new Vector2(noiseSetting.offset.x + mapSettings.mapOffset.x, noiseSetting.offset.y + mapSettings.mapOffset.y);
				for (int i = 0; i < noiseSetting.octaves; i++)
				{
					float offsetX = prng.Next(-100000, 100000) + origOffset.x;
					float offsetY = prng.Next(-100000, 100000) - origOffset.y;
					octaveOffsets[i] = new Vector2(offsetX, offsetY);
				}

				// Calculate noise for each coord in map
				for (int y = 0; y < mapSettings.mapHeight; y++)
				{
					for (int x = 0; x < mapSettings.mapWidth; x++)
					{

						// Warp the noise height by modifying the coordinates by feeding in 2 other layers of perlin noise
						float firstHeight = (FBM(x + noiseSetting.warp1.x, y + noiseSetting.warp1.y, mapSettings, noiseSetting, octaveOffsets));
						float secondHeight = (FBM(x + noiseSetting.warp2.x, y + noiseSetting.warp2.y, mapSettings, noiseSetting, octaveOffsets));
						Vector2 warp = new Vector2(noiseSetting.warpScale * firstHeight, noiseSetting.warpScale * secondHeight);
						
						// Calculate height using multi step perlin noise (fractal brownian motion)
						noiseMap[x, y] += (FBM(x + warp.x, y + warp.y, mapSettings, noiseSetting, octaveOffsets));
					}
				}
			}

        }

        return noiseMap;
	}

	// Fractal Brownian Motion 
	private static float FBM(float x, float y, MapSettings mapSettings, NoiseSettings noiseSettings, Vector2[] octaveOffsets)
	{
		// Impact of each octave
		float amplitude = 1;

		// level of detail of each octave
		float frequency = 1;

		float noiseHeight = 0;

		// calculate noise for each octave 
		for (int i = 0; i < noiseSettings.octaves; i++)
		{
			// Calculate perlin noise input using offset map location and divided by our map scale factor to determine the range of perlin noise. 
			float sampleX = ((x - mapSettings.halfWidth + octaveOffsets[i].x)) / (noiseSettings.scale + mapSettings.scale) * frequency;
			float sampleY = ((y - mapSettings.halfHeight + octaveOffsets[i].y)) / (noiseSettings.scale + mapSettings.scale) * frequency;

			// Perlin noise output is modified to range of (-1, 1)
            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

			// Calculate ridge and billow noise which modify the output from the original perlin noise
            float ridgedValue = 1.0f - Mathf.Abs(perlinValue);
            float billowvalue = perlinValue * perlinValue;
            float noiseValue = Mathf.Lerp(perlinValue, billowvalue, Mathf.Max(0.0f, noiseSettings.sharpness));
            noiseValue += Mathf.Lerp(perlinValue, ridgedValue, Mathf.Abs(Mathf.Min(0.0f, noiseSettings.sharpness)));

			
            noiseHeight += noiseValue * amplitude;

			/// update the amplitude and frequency for the next octave
			amplitude *= noiseSettings.persistance;
			frequency *= noiseSettings.lacunarity;
		}

		// Multiple the noise output by this noise layer strength to give some layer higher impact
		noiseHeight *= noiseSettings.strength;

		// Apply a minimum value that will sink anything below that height into the ocean level
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