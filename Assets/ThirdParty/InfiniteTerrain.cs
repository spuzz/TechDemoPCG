using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteTerrain : MonoBehaviour
{
    public const float scale = 1f;
    public static float maxViewDist = 450;
    public Transform viewer;

    public static Vector2 viewerPos;
    public Material mapMaterial;
    static MapGenerator mapGenerator;

    public LODInfo[] LODLevels;
    int chunkSize;
    int chunksVisible;
    const float thresholdUpdateChunk = 25f;
    const float sqThresholdUpdateChunk = thresholdUpdateChunk * thresholdUpdateChunk;
    Vector2 viewerPosOld;

    public Dictionary<Vector2, TerrainChunk> terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> chunksLastUpdate = new List<TerrainChunk>();
    private void Start()
    {
        maxViewDist = LODLevels[LODLevels.Length - 1].visDistThreshold;
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisible = Mathf.RoundToInt(maxViewDist / chunkSize);
        mapGenerator = FindObjectOfType<MapGenerator>();
        UpdateVisibleChunks();
    }

    public void Reset()
    {
        foreach(TerrainChunk chunk in terrainChunkDict.Values)
        {
            chunk.deleteObject();
        }
        terrainChunkDict.Clear();
        chunksLastUpdate.Clear();

        maxViewDist = LODLevels[LODLevels.Length - 1].visDistThreshold;
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisible = Mathf.RoundToInt(maxViewDist / chunkSize);
        mapGenerator = FindObjectOfType<MapGenerator>();
        UpdateVisibleChunks();
    }

    private void Update()
    {
        viewerPos = new Vector2(viewer.position.x, viewer.position.z) / scale;
        if((viewerPosOld - viewerPos).sqrMagnitude > thresholdUpdateChunk)
        {
            viewerPosOld = viewerPos;
            UpdateVisibleChunks();
        }
        
    }


    void UpdateVisibleChunks()
    {
        foreach (TerrainChunk chunk in chunksLastUpdate)
        {
            chunk.SetVisible(false);
        }
        chunksLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPos.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPos.y / chunkSize);

        for (int yOffSet = -chunksVisible; yOffSet <= chunksVisible; yOffSet++)
        {
            for (int xOffSet = -chunksVisible; xOffSet <= chunksVisible; xOffSet++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffSet, currentChunkCoordY + yOffSet);
                if (terrainChunkDict.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDict[viewedChunkCoord].UpdateTerrainChunk();
     
                }
                else
                {
                    terrainChunkDict.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform, mapMaterial, LODLevels));
                }
            }
        }
    }

    public class TerrainChunk
    {
        Vector2 pos;
        GameObject meshObject;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        LODInfo[] lodLevels;
        LODMesh[] lodMeshes;
        int prevLODIndex = -1;

        MapData mapData;
        bool mapDataReceived;

       

        public TerrainChunk(Vector2 coord, int size, Transform parent, Material material, LODInfo[] lodLevels)
        {
            this.lodLevels = lodLevels;
            pos = coord * size;
            bounds = new Bounds(pos, Vector2.one * size);
            Vector3 posV3 = new Vector3(pos.x, 0, pos.y);
            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshObject.transform.position = posV3 * scale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * scale;

            SetVisible(false);

            lodMeshes = new LODMesh[lodLevels.Length];

            for(int i = 0; i < lodLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(lodLevels[i].lod, UpdateTerrainChunk);
            }

            mapGenerator.RequestMapData(OnMapDataReceived, pos);
        }

        public void deleteObject()
        {
            Destroy(meshObject);
        }

        void OnMapDataReceived(MapData mapData)
        {
            this.mapData = mapData;
            mapDataReceived = true;

            if(meshRenderer != null)
            {
                Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colourMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
                meshRenderer.material.mainTexture = texture;
                UpdateTerrainChunk();
            }

        }

        public void UpdateTerrainChunk()
        {
            if(mapDataReceived == true)
            {
                float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPos));
                bool visible = viewerDstFromNearestEdge <= maxViewDist;

                if (visible)
                {
                    int lodIndex = 0;
                    for (int i = 0; i < lodLevels.Length - 1; i++)
                    {
                        if (viewerDstFromNearestEdge > lodLevels[i].visDistThreshold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (prevLODIndex != lodIndex)
                    {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if (lodMesh.hasMesh)
                        {
                            meshFilter.mesh = lodMesh.mesh;
                            prevLODIndex = lodIndex;
                        }
                        else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(mapData);
                        }

                    }

                    chunksLastUpdate.Add(this);
                }
                SetVisible(visible);
            }

        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }

    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;
        System.Action updateCallback;


        public LODMesh(int lod, System.Action updateCallback)
        {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        void OnMeshDataReceived(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            hasMesh = true;

            updateCallback();
        }
        public void RequestMesh(MapData mapData)
        {
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
        }
    }

    [System.Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visDistThreshold;
    }
}
