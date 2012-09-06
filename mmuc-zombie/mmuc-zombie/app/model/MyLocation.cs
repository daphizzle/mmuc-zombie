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
using System.Collections.Generic;
using System.Device.Location;
using Parse;

    public class MyLocation : MyParseObject
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string gameId { get; set; }
        public int number { get; set; }
        public MyLocation()
        {
        }
        public MyLocation(double lat, double lon)
        {
            latitude = lat;
            longitude = lon;
        }
        public GeoCoordinate toGeoCoordinate(){
            return new GeoCoordinate(latitude, longitude);
        }
        public void fromGeoCoordinate (GeoCoordinate g){
            latitude = g.Latitude;
            longitude = g.Longitude;
            }
        

    }

