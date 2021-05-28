using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace Map
{
    // type aliases used in Map namespace
    using GPXCoordinateList = List<GPXReaderLib.Models.GPXCoordinates>;
    using GPXElevationList = List<double>;

    // Read GPS track from .gpx file. Utility class to determine coordinates of
    // the OSM map and the elevation data (altimetry) of the GPS track.
    public class GPXInput : MonoBehaviour
    {
        // path of the .gpx file
        [SerializeField] private String _gpxFileName;

        private AwesomeTiles.Tile _leftBottom;  // bottom left OSM tile
        private AwesomeTiles.Tile _rightTop;    // top right OSM tile
        private GPXCoordinateList _coordinates; // list of gps coordinates
        private GPXElevationList _elevation;    // altimetry data

        // structure to save the corner coordinates of the
        // OSM minimap.
        public struct CornerCoordinates
        {
            public double minLon;
            public double minLat;
            public double maxLon;
            public double maxLat;
        };

        // Initialize the private variables of the object before using it (acts
        // like a constructor).
        public void Initialize(int zoom)
        {
            // read file as a basic xml file
            XDocument myGPX = XDocument.Load(_gpxFileName);

            // manager for the .gpx file format
            XmlNamespaceManager r = new XmlNamespaceManager(new NameTable());
            r.AddNamespace("p", "http://www.topografix.com/GPX/1/1");

            // create gpx reader and retrieve coordinate and elevation data
            var gPXReader = new GPXReaderLib.GPXReader(myGPX, r);
            _coordinates = gPXReader.GetGPXCoordinates();
            _elevation = gPXReader.GetGPXAltimetry().Altimetries.Select(c => c.Elevation).ToList();

            // Find the corner OSM tiles (bottom left, top rigth) based on the
            // gpx coordinates and the desired zoom level.
            var coords = ComputeTrackCoordinates(_coordinates);
            _leftBottom = AwesomeTiles.Tile.CreateAroundLocation(coords.minLat,
                                                                 coords.minLon, zoom);
            _rightTop = AwesomeTiles.Tile.CreateAroundLocation(coords.maxLat,
                                                               coords.maxLon, zoom);
        }

        // Get gpx coordinate list
        public GPXCoordinateList GetGPXCoordinateList()
        {
            return _coordinates;
        }

        // Get gpx coordinates as a Vector3[]
        public Vector3[] GetGPXCoordinateVector3Array()
        {
            var list = new List<Vector3>();
            foreach (var coord in _coordinates)
            {
                list.Add(new Vector3((float)coord.Longitude,
                                     (float)coord.Latitude,
                                     0));
            }
            return list.ToArray();
        }

        // get elevation data
        public GPXElevationList GetGPXAltimetry()
        {
            return _elevation;
        }

        // get the corner (X, Y) coordinates of the  OSM tiles of the map.
        public CornerCoordinates ComputeMapCoordinateBounds()
        {
            var result = new CornerCoordinates();
            result.minLon = Math.Min(_leftBottom.X, _rightTop.X);
            result.maxLon = Math.Max(_leftBottom.X, _rightTop.X);
            result.minLat = Math.Min(_leftBottom.Y, _rightTop.Y);
            result.maxLat = Math.Max(_leftBottom.Y, _rightTop.Y);
            return result;
        }

        // get the corner (Lon, Lat) coordinates of the OSM tiles of the map.
        public CornerCoordinates ComputeTrackCoordinateBounds()
        {
            var result = new CornerCoordinates();
            result.minLon = Math.Min(_leftBottom.Left, _rightTop.Left);
            result.maxLon = Math.Max(_leftBottom.Right, _rightTop.Right);
            result.minLat = Math.Min(_leftBottom.Bottom, _rightTop.Bottom);
            result.maxLat = Math.Max(_leftBottom.Top, _rightTop.Top);
            return result;
        }

        // get the corner (Lon, Lat) coordinates of the GPX track.
        public CornerCoordinates ComputeTrackCoordinates(GPXCoordinateList coords)
        {
            var coord = new CornerCoordinates();
            coord.maxLon = coords.Max(c => c.Longitude);
            coord.maxLat = coords.Max(c => c.Latitude);
            coord.minLon = coords.Min(c => c.Longitude);
            coord.minLat = coords.Min(c => c.Latitude);
            return coord;
        }

        // get the central coordinates (Lon, Lat) of the whole minimap
        public Vector3 ComputeCenterCoordinates()
        {
            var res = new Vector3(0, 0, 0);
            var coords = ComputeTrackCoordinateBounds();
            res.x = (float)(coords.minLon + coords.maxLon) / 2;
            res.y = (float)(coords.minLat + coords.maxLat) / 2;
            return res;

        }
    }
}
