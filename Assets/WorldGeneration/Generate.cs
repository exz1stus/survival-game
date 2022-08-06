using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using BetterRandom;
public static class NoiseGenerator
{
    public static float [,] GenerateNoise (int width, int height, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2Int offset)
    {
        float[,] noise = new float[width, height];
        //System.Random rand = new System.Random(seed);
        FastRandom rand = new FastRandom(seed);

        Vector2[] octavesOffset = new Vector2[octaves];

        for (int i=0; i < octaves; i++)
        {
            float xOffset = rand.Next(-100000, 100000) + offset.x * (width / scale);
            float yOffset = rand.Next(-100000, 100000) + offset.y * (height / scale);

            octavesOffset[i] = new Vector2(xOffset / width, yOffset / height);
        }

        if (scale < 0) scale = 0.0001F;

        float halfWidth = width / 2.0F;
        float halfHeight = height / 2.0F;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                float superpositionCompensation = 0;

                for (int i=0; i<octaves; i++)
                {
                    float xResult = (x - halfWidth) / scale * frequency + octavesOffset[i].x * frequency;
                    float yResult = (y - halfHeight) / scale * frequency + octavesOffset[i].y * frequency;

                    float generateValue = Mathf.PerlinNoise(xResult, yResult);

                    noiseHeight += generateValue * amplitude;
                    noiseHeight -= superpositionCompensation;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                    superpositionCompensation = amplitude / 2;
                }

                noise[x, y] = Mathf.Clamp01(noiseHeight);
            }
        }
        return noise;
    }
    public static float[,] GenerateNoiseByScriptableObject(int width, int height, Noise noise, Vector2Int offset)
    {
        return GenerateNoise(width, height, noise.seed, noise.scale, noise.octaves, noise.persistence, noise.lacunarity, offset);
    }
}
[Serializable]
public struct Biome
{

    public string biomeName;

    public Vector2 heat;
    public Vector2 humidity;
    public Vector2 height;


    public ColorLevel[] ColorMap;
    public BiomeObjects[] biomeObjects;
}
[Serializable]
public struct BiomeObjects
{
    public float height;
    public GameObject[] biomeObjectsArr;
    //[Range(9000, 9999)] public int chanceToSpawn;
}
[Serializable]
public struct ColorLevel
{
    public TileBase tile;

    public float height;

    // public bool IsCollider;
}
public class Generate : MonoBehaviour
{
    [Header("World  Info")]

    [SerializeField] private GameObject tilemapPrefab;

    [SerializeField] private int chunksCounter = 0;

    public GameObject grid;

    [Header("Tiles Noise Configuration")]

    [SerializeField] private Noise heightNoise;

    [Header("Biome Noise Configuration")]

    [SerializeField] private Noise heatNoise;
    [SerializeField] private Noise humidityNoise;
    [SerializeField] private Noise biomeHeightNoise;

    [SerializeField] private Biome[] biomeConfig;

    private BoundsInt bounds;

    public Tilemap minimapTlmp;

    private int noiseDimensionalX;
    private int noiseDimensionalY;

    private float[,] heightNoiseMap;

    private float[,] humidityNoiseMap;

    private float[,] heatNoiseMap;

    private float[,] biomeHeightNoiseMap;

    private TileMapManager tileMapManager;

    //private System.Random rand;
    private FastRandom brand;
    private void Start()
    {
        noiseDimensionalX = EndlessTerrainTest.ChunkSize;
        noiseDimensionalY = EndlessTerrainTest.ChunkSize;

        bounds = new BoundsInt(0, 0, 0, noiseDimensionalX, noiseDimensionalY, 1);

        tileMapManager = FindObjectOfType<TileMapManager>();
        //rand = new System.Random(seed);
        brand = new FastRandom(heightNoise.seed);

        /**/
        /*for (int i = -15* noiseDimensionalX; i < 15* noiseDimensionalX; i+= noiseDimensionalX)
        {
            for (int j = -15* noiseDimensionalY; j < 15* noiseDimensionalY; j+= noiseDimensionalY)
            {
                GenerateTilemap(new Vector2Int(i,j));
            }   
        }*/
    }

    private Biome IdentifyBiome(float height, float humidity, float heat)
    {
        foreach (var biome in biomeConfig)
        {
            if(biome.height.x </*=*/ height&& biome.height.y >= height &&
                biome.heat.x </*=*/ heat && biome.heat.y >= heat&&
                biome.humidity.x </*=*/ humidity && biome.humidity.y >= humidity)
            {
                return biome;
            }
        }

        //Debug.Log("blin" + " " + height.ToString() + " " + humidity.ToString() + " " + heat.ToString());
        return biomeConfig[0];
    }

    public GameObject GenerateTilemap(Vector2Int offset)
    {
        GameObject tilemapL = Instantiate(tilemapPrefab, new Vector3(offset.x, offset.y, 0), Quaternion.identity , grid.transform);
        tilemapL.transform.position = new Vector3(offset.x, offset.y, 0);

        Tilemap localTmap = tilemapL.GetComponent<Tilemap>();

        heightNoiseMap = new float[noiseDimensionalX, noiseDimensionalY];
        humidityNoiseMap = new float[noiseDimensionalX, noiseDimensionalY];
        biomeHeightNoiseMap = new float[noiseDimensionalX, noiseDimensionalY];
        heatNoiseMap = new float[noiseDimensionalX, noiseDimensionalY];

        heightNoiseMap = NoiseGenerator.GenerateNoiseByScriptableObject(noiseDimensionalX, noiseDimensionalY, heightNoise, offset);
        humidityNoiseMap = NoiseGenerator.GenerateNoiseByScriptableObject(noiseDimensionalX, noiseDimensionalY, humidityNoise, offset);
        biomeHeightNoiseMap = NoiseGenerator.GenerateNoiseByScriptableObject(noiseDimensionalX, noiseDimensionalY, biomeHeightNoise, offset);
        heatNoiseMap = NoiseGenerator.GenerateNoiseByScriptableObject(noiseDimensionalX, noiseDimensionalY, heatNoise, offset);

        chunksCounter++;
        StartCoroutine(TilePlacer(tilemapL, offset));
        return tilemapL;
    }
    private IEnumerator TilePlacer(GameObject localTmapGO, Vector2Int offset)
    {
        Tilemap collMap = localTmapGO.transform.GetChild(0).GetComponent<Tilemap>();
        Tilemap localTmap = localTmapGO.GetComponent<Tilemap>();

        Tile/*Base*/[] tileArray = new Tile/*Base*/[noiseDimensionalX*noiseDimensionalY];

        FastRandom r = new FastRandom((offset.x + offset.y)*heightNoise.seed);

        int i = 0;

        for (int y = 0; y < noiseDimensionalY; y++)
        {
            for (int x = 0; x < noiseDimensionalX; x++)
            {
                Biome currentBiome = IdentifyBiome(biomeHeightNoiseMap[x, y], humidityNoiseMap[x, y], heatNoiseMap[x, y]);
                //tiles
                foreach(var colorMap in currentBiome.ColorMap)
                {
                    if (heightNoiseMap[x, y] <= colorMap.height)
                    {
                        tileArray[i] = (Tile)colorMap.tile;
                        //tileArray[i].color = new Color(biomeHeightNoiseMap[x, y], humidityNoiseMap[x, y], heatNoiseMap[x, y]);
                        break;
                    }
                }
                //objs

                /*int objectIndex =*/ //Debug.Log(Mathf.PerlinNoise((float)(x + offset.x) / noiseDimensionalX  + heightNoise.seed * 2, 
                                                                    //(float)(y + offset.y) / noiseDimensionalY + heightNoise.seed * 2) > 0.5);

                foreach(var biomeObj in currentBiome.biomeObjects)
                {
                    if(heightNoiseMap[x, y] < biomeObj.height && r.Next(0,100) >= 97)
                    {
                        int objIndex = r.Next(0, biomeObj.biomeObjectsArr.Length);

                        GameObject obj = Instantiate(biomeObj.biomeObjectsArr[objIndex], new Vector3(x + localTmap.transform.position.x + 0.5f, y + localTmap.transform.position.y + 0.5f, 0), Quaternion.identity, localTmap.transform);

                        obj.transform.parent = localTmap.transform;
                        break;
                    }
                }

                i++;
            }
        }
        localTmap.SetTilesBlock(bounds, tileArray);

        yield return null;
    }
}