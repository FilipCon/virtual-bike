using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class Track : MonoBehaviour
    {
        [SerializeField] private Minimap _miniMap;     // minimap instance
        [SerializeField] private GPXInput _gpxInput;   // gpx input instance
        [SerializeField] private float _depthPosition; // camera depth position
        [SerializeField] private float _trackWidth;    // gps track
                                                       // (linerenderer) width

        private LineRenderer _lineRenderer; // gps track
        private float _scale;               // gps track scale
        private float _offsetLon;           // coordinate offset
        private float _offsetLat;           // coordinate offset

        void Start()
        {
            _gpxInput.Initialize(_miniMap._zoom); // init gpx input
            var coords = _gpxInput.ComputeTrackCoordinateBounds();
            _scale = _miniMap.ComputeTrackScale(new Vector2((float)coords.minLon,
                                                            (float)coords.minLat),
                                                new Vector2((float)coords.maxLon,
                                                            (float)coords.maxLat));
            // set track offset coordinates
            _offsetLon = (float)coords.minLon;
            _offsetLat = (float)coords.minLat;

            // create the gps track
            var coordinates = _gpxInput.GetGPXCoordinateVector3Array();
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.widthMultiplier = _trackWidth;
            _lineRenderer.positionCount = coordinates.Length;
            _lineRenderer.SetPositions(CalibrateCoordinates(coordinates));
        }

        // calibrate the GPS coordinates to fit the Unity 3D spaces. Scales the
        // coordinates to fit the minimap presented with the unity tilemap.
        private Vector3[] CalibrateCoordinates(Vector3[] coordinates)
        {
            var result = new List<Vector3>();
            var bl = _miniMap.ComputeBottomLeftCellWorldCoords();
            foreach (var coord in coordinates)
            {
                var x = _scale * (float)(coord.x - _offsetLon);
                var y = _scale * (float)(coord.y - _offsetLat);
                result.Add(new Vector3(x, y, _depthPosition) + bl);
            }
            return result.ToArray();
        }

        private void Update()
        {
            // update the track width in play mode
            _lineRenderer.widthMultiplier = _trackWidth;
        }

    }
}
