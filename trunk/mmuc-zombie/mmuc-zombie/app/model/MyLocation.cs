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

namespace mmuc_zombie.app.model
{
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
    }
}
