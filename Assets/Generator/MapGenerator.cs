using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

// Map generator that will create map chunks using FBM (perlin noise) with multiple layers
// Threading and creation of mapchunks uses code from Sebastian Lague youtube series https://www.youtube.com/watch?v=wbpMiKiSKm8&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3
public class MapGenerator : MonoBehaviour
{
	// 241 is a good choice as it is easily divisible by several factors making LODS easier
	public const int mapChunkSize = 241;
	
    // Adjust level of detail for Editor Mesh
	[Range(0,6)]
	public int editorLOD;

    // Unused in variables in current implementation
	public enum DrawMode{NoiseMap, ColourMap, Mesh}
	DrawMode drawMode = DrawMode.Mesh;
    float heightMultiplier;
    AnimationCurve heightCurve;


    // Values that impact all layers
    [Range(0, 100)]
    public float noiseScale;
    public int seed;
    public float minValue;
    public NoiseSettings[] noiseSettings;

    public GameObject waterPlane;
	
	public bool autoUpdate;

	public TerrainType[] terrainTypes;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    private void Start()
    {
        // Activate plane that acts as water
        waterPlane.gameObject.SetActive(true);
    }

    MapData GenerateMapData(Vector2 centre)
    {
        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, minValue, centre, noiseSettings);

        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];

        // Create colour map using preset terrain levels 
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < terrainTypes.Length; i++)
                {
                    if (currentHeight >= terrainTypes[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = terrainTypes[i].colour;
                        
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        return new MapData(noiseMap, colourMap);

    }

    public void RequestMapData(Action<MapData> callback, Vector2 centre)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(callback, centre);
        };

        new Thread(threadStart).Start();

    }

    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, lod, callback);
        };

        new Thread(threadStart).Start();

    }


    void MapDataThread(Action<MapData> callback, Vector2 centre)
    {
        MapData mapData = GenerateMapData(centre);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
        
    }

    void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateMesh(mapData.heightMap, heightMultiplier, heightCurve, lod );
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }

    }

    private void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }
    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateMesh(mapData.heightMap, heightMultiplier, heightCurve, editorLOD), TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }

    }

    public void RegenerateWorld()
    {
        FindObjectOfType<InfiniteTerrain>().Reset();
    }


    //Generic for both map and mesh data
    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}


[System.Serializable]
public struct TerrainType
{
	public string name;
	public float height;
	public Color colour;
}

[System.Serializable]
public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colourMap;

    public MapData(float[,] heightMap, Color[] colourMap)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
    }
}



