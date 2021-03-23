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
    public Tilemap tilemap;

    private readonly string[] _serverEndpoints = { "a", "b", "c" };

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
                        // Debug.Log("Error: " + error);
                    }, (Texture2D texture2D) =>
                    {
                        // Save Image
                        ImageLoader.SaveImage(savePath, texture2D.EncodeToPNG());
                        tileTexList.Add((tile.X, -tile.Y, texture2D));
                    });
            }
            else
            {
                // Load map tile png from file.
                var tex = new Texture2D(1, 1); // size does not matter.
                tex.LoadImage(ImageLoader.LoadImage(savePath));
                tileTexList.Add((tile.X, -tile.Y, tex));
            }
        }

        // build tilemap from osm tiles
        Vector3Int[] positions = new Vector3Int[tiles.Count];
        TileBase[] tileArray = new TileBase[positions.Length];
        for (int i = 0; i < positions.Length; ++i)
        {
            var newTile = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
            var tex = tileTexList[i].texture;
            newTile.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0), Math.Max(tex.width, tex.height));
            positions[i] = new Vector3Int(tileTexList[i].X - tiles.XMin, tileTexList[i].Y + tiles.YMax, 0);
            tileArray[i] = newTile;

            Debug.Log(positions[i].ToString());
        }
        tilemap.SetTiles(positions, tileArray);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
