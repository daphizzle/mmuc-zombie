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
using Parse;
using System.Diagnostics;
using System.Collections.Generic;
using System.Device.Location;


    public class Location : MyParseObject
    {
      public double Latitude { get; set; }
      public double Longitude { get; set; }

      public Location() { }
      public Location(double latitude, double longitude)
      {
         Latitude = latitude;
         Longitude = longitude;
      }

      public static void updateLocation(string locationId,GeoCoordinate gc)
      {
          var driver = new Driver();
          driver.Objects.Get<Location>(locationId, r =>
          {
              if (r.Success)
              {
                  var location = r.Data;
                  location.Latitude = gc.Latitude;
                  location.Longitude = gc.Longitude;
                  location.update();
              }
          });

      }

    }

