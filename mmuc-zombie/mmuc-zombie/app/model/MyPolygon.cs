using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls.Maps;
using mmuc_zombie.app.model;
using System.Device.Location;

    public class MyPolygon : MapPolygon
    {

        public static bool inPolygon(int nvert, MyLocation[] locs, double testx, double testy)
        {
            int i, j;
            bool c = false;
            for (i = 0, j = nvert - 1; i < nvert; j = i++)
            {
                if (((locs[i].longitude > testy) != (locs[j].longitude > testy)) &&
                 (testx < (locs[j].latitude - locs[i].latitude) * (testy - locs[i].longitude) / (locs[j].longitude - locs[i].longitude) + locs[i].latitude))
                    c = !c;
            }
            return c;
        }

        public GeoCoordinate middlePoint()
        {
            var mid = new GeoCoordinate();
            double lat = 0;
            double lon = 0;
            foreach (GeoCoordinate g in this.Locations)
            {
                lat += g.Latitude;
                lon += g.Longitude;
            }
            mid.Latitude = lat/this.Locations.Count;
            mid.Longitude = lon/this.Locations.Count;
            return mid;
        }
    
    }

         
    
