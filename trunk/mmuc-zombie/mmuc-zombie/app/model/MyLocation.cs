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
using System.Diagnostics;

    public class MyLocation : MyParseObject
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string gameId { get; set; }
        public int number { get; set; }
        public MyLocation()
        {
        }

        public void update(Action<Response<MyLocation>> callback)
        {
            var parse = new Driver();
            parse.Objects.Update<MyLocation>(this.Id).
                Set(u => u.latitude, latitude).
                Set(u => u.longitude, longitude).
                Set(u => u.gameId, gameId).
                Set(u => u.number, number).
                Execute(r =>
                {
                    if (r.Success)
                    {
                        Debug.WriteLine("MyLocation : " + Id + " successfull updated");
                    }
                    //else
                    //{
                    //    Debug.WriteLine("User : " + Id + " error while updating. " + r.Error.Message);
                    //}

                });
        
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

