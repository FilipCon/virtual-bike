using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Xml.Linq;
using System.Xml;
using UnityEngine.UI;

namespace Map
{
    public class Graph : MonoBehaviour
    {
        [SerializeField] private String _gpxFileName;
        [SerializeField] private Sprite _circeSprite;

        [SerializeField] private RectTransform graphContainer;

        float customLat = 36.08f;
        float customLon = -115.19f;
         
        void Start()
        {

            XDocument myGPX = XDocument.Load(_gpxFileName);

            XmlNamespaceManager r = new XmlNamespaceManager(new NameTable());
            r.AddNamespace("p", "http://www.topografix.com/GPX/1/1");

            var gPXReader = new GPXReaderLib.GPXReader(myGPX, r);

            var coordinates = gPXReader.GetGPXCoordinates();
            var altimetry = gPXReader.GetGPXAltimetry();

            List<float> latitudeList = new List<float>();
            List<float> longitudeList = new List<float>();
            foreach (var coord in coordinates) {
                latitudeList.Add((float)coord.Latitude);
                longitudeList.Add((float)coord.Longitude);
            }

            // plot the centered coordinates
            foreach (var coord in coordinates)
            {

                float scale = 1000;
                var lat = scale * ((float)coord.Latitude - latitudeList.Min()); // x
                var lon = scale * ((float)coord.Longitude - longitudeList.Min()); // y

                CreateCirce(new Vector2(lon, lat));

            }


            // We finally know how many vertices we have so we can construct the matrix
            // CreateEdges(Tracks);
        }


        private void CreateCirce(Vector2 anchoredPosition)
        {
            GameObject gob = new GameObject("circle", typeof(Image));
            gob.transform.SetParent(graphContainer, false);
            gob.GetComponent<Image>().sprite = _circeSprite;
            //Get the Renderer component
            var renderer = gob.GetComponent<CanvasRenderer>();
            renderer.SetColor(Color.red);

            var rectTransform = gob.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(10, 10);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
        }

    }

}
