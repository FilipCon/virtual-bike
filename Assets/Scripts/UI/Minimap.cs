using System;
using System.Collections.Generic;
using System.IO;
using AwesomeTiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{
    public class Minimap : MonoBehaviour
    {

        [SerializeField] public int _zoom = 13;      // default zoom level
        [SerializeField] private Camera _cam;        // minimap camera
        [SerializeField] private GPXInput _gpxInput; // gpx input util objectf

        public Tilemap tilemap;                      // unity tilemap to merge OSM tiles
        private readonly string[] _serverEndpoints = { "a", "b", "c" }; // OSM server addreses

        void Awake()
        {
            // Create a TileRange based on the corner coordinates of the GPX
            // track.
            _gpxInput.Initialize(_zoom); // initialize _gpxInput obj
            var bounds = _gpxInput.ComputeMapCoordinateBounds();
            var tiles = new TileRange((int)bounds.minLon, (int)bounds.minLat,
                                      (int)bounds.maxLon, (int)bounds.maxLat,
                                      _zoom);

            // init list of tex2D that forms the tiles in the tilemap
            var tileTexList = new List<(int X, int Y, Texture2D texture)>();

            // system path to save the png OSM tile images and later load them
            // from dir
            string tilesDataPath = Path.Combine(Application.persistentDataPath,
                                                "Assets/Resources");

            // for each tile in the tilerange, download from server and save to
            // file or load from file if it already exists/
            foreach (var tile in tiles)
            {
                var random = new System.Random();
                var savePath = Path.Combine(tilesDataPath, "Tiles",
                                            $"{_zoom}/{tile.X}/{tile.Y}.png");
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
                newTile.sprite = Sprite.Create(tex,
                                               new Rect(0, 0, tex.width, tex.height),
                                               new Vector2(0, 0),
                                               Math.Max(tex.width, tex.height));
                positions[i] = new Vector3Int(tileTexList[i].X - tiles.XMin,
                                              tileTexList[i].Y + tiles.YMax, 0);
                tileArray[i] = newTile;
            }
            tilemap.SetTiles(positions, tileArray);

            // move camera to minimap center
            // TODO move it to the first gps coordinate
            var center = ComputeTileMapCenter();
            _cam.transform.position = new Vector3(center.x / 2.0f,
                                                  center.y / 2.0f,
                                                  _cam.transform.position.z);
        }

        // compute unity tilemap center
        public Vector3 ComputeTileMapCenter()
        {
            Vector3Int origin = tilemap.WorldToCell(transform.position);
            var x1 = tilemap.size.x % 2 == 0 ? tilemap.size.x / 2 : tilemap.size.x / 2 + 1;
            var x2 = tilemap.size.x / 2;

            var y1 = tilemap.size.y % 2 == 0 ? tilemap.size.y / 2 : tilemap.size.y / 2 + 1;
            var y2 = tilemap.size.y / 2;

            Vector3Int center1 = new Vector3Int(x1, y1, 0);
            Vector3Int center2 = new Vector3Int(x2, y2, 0);
            return (tilemap.GetCellCenterWorld(center1) +
                        tilemap.GetCellCenterWorld(center2));

        }

        // compute unity tilemap (minimap) Bottom Left coordinates in the Unity
        // 3D world coordinates
        public Vector3 ComputeBottomLeftCellWorldCoords()
        {
            return tilemap.GetCellCenterWorld(new Vector3Int(0, 0, 0));
        }

        // compute unity tilemap (minimap) Top Right coordinates in the Unity
        // 3D world coordinates
        Vector3 ComputeTopRightCellWorldCoords()
        {
            return tilemap.GetCellCenterWorld(new Vector3Int(tilemap.size.x,
                                                             tilemap.size.y, 0));
        }

        // Compute the factor to scale the gpx coordiantes to fit the minimap
        public float ComputeTrackScale(Vector2 leftBottom, Vector2 rightTop)
        {
            Vector3 lb = ComputeBottomLeftCellWorldCoords();
            Vector3 rt = ComputeTopRightCellWorldCoords();

            // unity coordinates distance
            float worldDist = Vector3.Distance(rt, lb);

            // osm map distance
            float osmDist = Vector2.Distance(rightTop, leftBottom);

            return worldDist / osmDist;
        }
    }
}
