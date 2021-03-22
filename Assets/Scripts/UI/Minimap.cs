using System;
using System.Collections.Generic;
using AwesomeTiles;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Minimap : MonoBehaviour
{
    [SerializeField] private int _zoom = 13;

    private readonly string[] _serverEndpoints = { "a", "b", "c" };
    public Tilemap _tilemap = new Tilemap();

    // Start is called before the first frame update
    void Start()
    {
        string tilesDataPath = Path.Combine(Application.persistentDataPath, "Assets/Resources");
        var leftBottom = AwesomeTiles.Tile.CreateAroundLocation(double.Parse("9.973525"), double.Parse("76.21296"), _zoom);
        var topRight = AwesomeTiles.Tile.CreateAroundLocation(double.Parse("10.07677"), double.Parse("76.28596"), _zoom);

        var minX = Math.Min(leftBottom.X, topRight.X);
        var maxX = Math.Max(leftBottom.X, topRight.X);
        var minY = Math.Min(leftBottom.Y, topRight.Y);
        var maxY = Math.Max(leftBottom.Y, topRight.Y);
        var tiles = new TileRange(minX, minY, maxX, maxY, _zoom);

        var tileTexList = new List<(int X, int Y, Texture2D texture)>();

        foreach (var tile in tiles)
        {
            var random = new System.Random();
            var savePath = Path.Combine(tilesDataPath, "Tiles", $"{_zoom}/{tile.X}/{tile.Y}.png");
            if (!File.Exists(savePath))
            {
                // Download new map tile
                var url =
                $"http://{_serverEndpoints[random.Next(0, 2)]}.tile.openstreetmap.org/{_zoom}/{tile.X}/{tile.Y}.png";
                WebRequests.GetTexture(url,
                    (string error) =>
                    {
                        // Net error.
                        Debug.Log("Error: " + error);
                    }, (Texture2D texture2D) =>
                    {
                        // Save Image
                        var savePath = Path.Combine(tilesDataPath, "Tiles", $"{_zoom}/{tile.X}/{tile.Y}.png");
                        ImageLoader.saveImage(savePath, texture2D.EncodeToPNG());
                        tileTexList.Add((tile.X, -tile.Y, texture2D));
                    });
            }
            else
            {
                // Load map tile png from file.
                Texture2D tex = new Texture2D(1, 1); // size does not matter.
                tex.LoadImage(ImageLoader.loadImage(savePath));
                tileTexList.Add((tile.X, -tile.Y, tex));
            }
        }

        Vector3Int[] positions = new Vector3Int[tiles.Count];
        TileBase[] tileArray = new TileBase[positions.Length];
        for (int i = 0; i < positions.Length; ++i)
        {
            var newTile = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
            var tex = tileTexList[i].texture;
            newTile.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
            positions[i] = new Vector3Int(tileTexList[i].X - tiles.XMin, tileTexList[i].Y + tiles.YMax, 0);
            tileArray[i] = newTile;
        }

        _tilemap.SetTiles(positions, tileArray);
        GetComponent<RawImage>().texture = TilemapToTexture2D.Convert(_tilemap);
    }


    // static public Texture2D CreateTexture2DFromTilemap(Tilemap tilemap)
    // {

    //     int tileCellSizeX = (int)tilemap.cellSize.x;
    //     int tileCellSizeY = (int)tilemap.cellSize.y;
    //     Texture2D output = new Texture2D(tilemap.GridWidth * tilePxSizeX, tilemap.GridHeight * tilePxSizeY, TextureFormat.ARGB32, false);
    //     output.filterMode = FilterMode.Point;
    //     output.SetPixels32(new Color32[output.width * output.height]);
    //     output.Apply();
    //     Texture2D atlasTexture = tilemap.Tileset.AtlasTexture;
    //     System.Action<Tilemap, int, int, uint> action = (Tilemap source, int gridX, int gridY, uint tileData) =>
    //     {
    //         gridX -= source.MinGridX;
    //         gridY -= source.MinGridY;
    //         Tile tile = tilemap.Tileset.GetTile(Tileset.GetTileIdFromTileData(tileData));
    //         if (tile != null)
    //         {
    //             Color[] srcTileColors = atlasTexture.GetPixels(Mathf.RoundToInt(tile.uv.x * atlasTexture.width), Mathf.RoundToInt(tile.uv.y * atlasTexture.height), tilePxSizeX, tilePxSizeY);
    //             output.SetPixels(gridX * tilePxSizeX, gridY * tilePxSizeY, tilePxSizeX, tilePxSizeY, srcTileColors);
    //         }
    //     };
    //     IterateTilemapWithAction(tilemap, action);
    //     output.Apply();
    //     return output;
    // }

    // Update is called once per frame
    void Update()
    {

    }
}
