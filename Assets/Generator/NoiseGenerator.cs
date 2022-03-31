using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
	// Start is called before the first frame update
	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, Vector2 mapOffset, NoiseSettings[] noiseSettings)
	{
		float[,] noiseMap = new float[mapWidth, mapHeight];

		MapSettings mapSettings = new MapSettings(mapWidth, mapHeight, seed, scale, mapOffset);

		foreach (NoiseSettings noiseSetting in noiseSettings)
        {
            System.Random prng = new System.Random(seed);
            Vector2[] octaveOffsets = new Vector2[noiseSetting.octaves];

            Vector2 origOffset = new Vector2(noiseSetting.offset.x + mapSettings.mapOffset.x, noiseSetting.offset.y + mapSettings.mapOffset.y);   
            for (int i = 0; i < noiseSetting.octaves; i++)
            {
                float offsetX = prng.Next(-100000, 100000) + origOffset.x;
                float offsetY = prng.Next(-100000, 100000) - origOffset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);

                //maxPossibleHeight += amplitude;
                //maxPossibleHeight *= persistance;
            }

            for (int y = 0; y < mapSettings.mapHeight; y++)
            {
                for (int x = 0; x < mapSettings.mapWidth; x++)
                {
                    noiseMap[x, y] = FBM(x, y, mapSettings, noiseSetting, octaveOffsets, Vector2.zero);
                }
            }
        }






        //for (int y = 0; y < mapHeight; y++)
        //{
        //	for (int x = 0; x < mapWidth; x++)
        //          {

        //		Vector2 warp = new Vector2(0.0f, 0.0f);
        //		float firstHeight = FBM(scale, octaves, persistance, lacunarity, octaveOffsets, halfWidth, halfHeight, y, x, warp);
        //		warp = new Vector2(5.2f, 1.3f);
        //		float secondHeight = FBM(scale, octaves, persistance, lacunarity, octaveOffsets, halfWidth, halfHeight, y, x, warp);
        //		warp = new Vector2(4.0f * firstHeight, 4.0f * secondHeight);
        //		//warp = new Vector2(0.0f, 0.0f);
        //		float noiseHeight = FBM(scale, octaves, persistance, lacunarity, octaveOffsets, halfWidth, halfHeight, y, x, warp);
        //              noiseMap[x, y] = noiseHeight;
        //          }
        //}

        //for (int y = 0; y < mapHeight; y++)
        //{
        //	for (int x = 0; x < mapWidth; x++)
        //	{

        //		//noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
        //		float normalizedHeight = noiseMap[x, y]  /  maxPossibleHeight;
        //              //noiseMap[x, y] = Mathf.Clamp(normalizedHeight, -1, int.MaxValue);

        //              //noiseMap[x, y] = normalizedHeight;
        //          }

        //}

        return noiseMap;
	}

	private static float FBM(int x, int y, MapSettings mapSettings, NoiseSettings noiseSettings, Vector2[] octaveOffsets, Vector2 warp)
	{
		float amplitude = 1;
		float frequency = 1;
		float noiseHeight = 0;


		//float xf = ((x - mapSettings.halfWidth)) / mapSettings.scale;
		//float yf = ((y - mapSettings.halfHeight)) / mapSettings.scale;

		//amplitude = Mathf.PerlinNoise(xf, yf) * 3;

		for (int i = 0; i < noiseSettings.octaves; i++)
		{
			float sampleX = ((x - mapSettings.halfWidth + octaveOffsets[i].x) + warp[0]) / mapSettings.scale * frequency;
			float sampleY = ((y - mapSettings.halfHeight + octaveOffsets[i].y) + warp[1]) / mapSettings.scale * frequency;
			//float perlinValue = (Mathf.Abs(0.5f - Mathf.PerlinNoise(sampleX, sampleY)));
			//float perlinValue = 0.5f - (Mathf.Abs(0.5f - Mathf.PerlinNoise(sampleX, sampleY)));
			float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
			noiseHeight += perlinValue * amplitude;

			amplitude *= noiseSettings.persistance;
			frequency *= noiseSettings.lacunarity;
		}

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