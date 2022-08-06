using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : MonoBehaviour
{
     public Dictionary<Vector2Int,Tilemap> tilemaps = new Dictionary<Vector2Int, Tilemap>();
    [SerializeField] private List<TileData> tileDatas;

    [SerializeField] private Camera cam;

    public Dictionary<TileBase, TileData> dataFromTiles;

    private void Awake()
    {
        cam = Camera.main;
        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (var tileData in tileDatas)
        {
            foreach(var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }
    private void Update()
    {
        

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Tilemap tlmp = tilemaps[new Vector2Int(Mathf.RoundToInt(mousePos.x / 100), Mathf.RoundToInt(mousePos.x / 100))];
            Vector3Int grid = tlmp.WorldToCell(mousePos);

            TileBase clickedTile = tlmp.GetTile(grid);

            Debug.Log("Speed" + dataFromTiles[clickedTile].walkingSpeed);


        }
    }
}
