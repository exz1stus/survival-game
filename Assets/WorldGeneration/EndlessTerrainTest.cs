using System.Collections.Generic;
using UnityEngine;
public class EndlessTerrainTest : MonoBehaviour
{
    private static float maxViewDist = 100;

    [SerializeField] private int chunkSize = 30;
    [Range(1,10)][SerializeField] private int renderDistance = 3;
    public static int ChunkSize { get; private set; }

    [SerializeField] private Transform viewer;

    public static Vector2 viewerPosition;

    private int chunkVisibleInViewDst;

    public static Generate generate;        

    private Dictionary<Vector2, TerrainChunk> terrainchunkDct = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    private void Start()
    {
        ChunkSize = chunkSize;
        maxViewDist = ChunkSize * renderDistance;

        chunkVisibleInViewDst = Mathf.RoundToInt(maxViewDist / chunkSize);
        generate = GetComponent<Generate>();
    }
    private void Update()
    {
        viewerPosition = viewer.position;           ////
        UpdateVisibleChunk();
    }
    void UpdateVisibleChunk()
    {
        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);
        for (int yOffset = -chunkVisibleInViewDst; yOffset <= chunkVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunkVisibleInViewDst; xOffset <= chunkVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                if (terrainchunkDct.ContainsKey(viewedChunkCoord))
                {
                    terrainchunkDct[viewedChunkCoord].UpdateTerrainChunk();
                    if (terrainchunkDct[viewedChunkCoord].IsVisible())
                    {
                        terrainChunksVisibleLastUpdate.Add(terrainchunkDct[viewedChunkCoord]);
                    }
                }
                else
                {
                    terrainchunkDct.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize));
                }
            }
        }
    }
    public class TerrainChunk
    {
        GameObject chunkObj;
        Vector2 position;
        Bounds bounds;
        
        public void SetVisible(bool visible)
        {
            chunkObj.SetActive(visible);
        }
        public TerrainChunk(Vector2 coord, int size)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            chunkObj = generate.GenerateTilemap(new Vector2Int((int)position.x, (int)position.y));
            SetVisible(false);
        }
        public void UpdateTerrainChunk()
        {
            float viewerDstfromNearEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDstfromNearEdge <= maxViewDist;
            SetVisible(visible);
        }
        public bool IsVisible()
        {
            return chunkObj.activeSelf;
        }
    }
}


