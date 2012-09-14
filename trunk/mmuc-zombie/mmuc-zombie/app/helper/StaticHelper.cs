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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Phone.Shell;
using System.Device.Location;
using System.Diagnostics;

namespace mmuc_zombie.app.helper
{
    public class StaticHelper
    {
        


        public static T GetParentOfType<T>(DependencyObject item) where T : DependencyObject
        {
            if (item == null) throw new ArgumentNullException("item");
            T result = null;
            var parent = VisualTreeHelper.GetParent(item);
            if (parent == null) return result;
            else if (parent.GetType().IsSubclassOf(typeof(T)))
            {
                result = (T)parent;
            }
            else result = GetParentOfType<T>(parent);
            return result;
        }


        public static MyPolygon drawPolygon(List<MyLocation> list, Color clr)
        {   
            
            MyPolygon newPolygon = new MyPolygon();
            // Defines the polygon fill details
            newPolygon.Locations = new LocationCollection();
            newPolygon.Fill = new SolidColorBrush(clr);
            newPolygon.Stroke = new SolidColorBrush(Colors.Red);
            newPolygon.StrokeThickness = 3;
            newPolygon.Opacity = 0.5;
            var newlist = list.OrderBy(x => x.number).ToList();
            foreach (MyLocation l in newlist)
            {
                newPolygon.Locations.Add(new System.Device.Location.GeoCoordinate(l.latitude, l.longitude));
            }
            return newPolygon;
        }
      
         public static List<GeoCoordinate> drawCircle(GeoCoordinate origin,int radius)
        {  
          var earthRadius = 6371*1000;
      
          //latitude in radians
            var lat = (double)(origin.Latitude*Math.PI)/180; 
        
            //longitude in radians
            var lon = (double)(origin.Longitude*Math.PI)/180; 
        
            //angular distance covered on earth's surface
            double d = (double)(radius)/earthRadius;  
        
            var points = new List<GeoCoordinate>();
            for (int i = 0; i <= 360; i++) 
            { 
                var point = new GeoCoordinate();       
                var bearing = (double)i * Math.PI / 180; //rad
                point.Latitude = (double) Math.Asin(Math.Sin(lat)*Math.Cos(d) + Math.Cos(lat)*Math.Sin(d)*Math.Cos(bearing));
                point.Longitude = (double)((lon + Math.Atan2(Math.Sin(bearing)*Math.Sin(d)*Math.Cos(lat), Math.Cos(d)-Math.Sin(lat)*Math.Sin(point.Latitude))) * 180) / Math.PI;
                point.Latitude = (double)(point.Latitude * 180) / Math.PI;
                points.Add(point);
            }
            return points;
    
        }

         public static bool pointInPolygon(List<MyLocation> list, MyLocation loc)
        
        //     public static bool pnpoly(int nvert, double[] vertx, double[] verty, double testx, double testy)
        {
            int i, j;
            bool c = false;
            for (i = 0, j = list.Count-1; i < list.Count; j = i++)
            {
                if (((list[i].longitude > loc.longitude) != (list[j].longitude > loc.longitude)) &&
                 (loc.latitude < (list[j].latitude - list[i].latitude) * (loc.longitude - list[i].longitude) / (list[j].longitude - list[i].longitude) + list[i].latitude))
                    c = !c;
            }
            return c;
        
         }
         public static MyPolygon inGameArea(List<MyLocation> list)
         {

             MyPolygon newPolygon = new MyPolygon();
             // Defines the polygon fill details
             newPolygon.Locations = new LocationCollection();
             newPolygon.Fill = new SolidColorBrush(Colors.White);
             newPolygon.Stroke = new SolidColorBrush(Colors.Red);
             newPolygon.StrokeThickness = 6;
             newPolygon.Opacity = 0.2;
             var newlist = list.OrderBy(x => x.number).ToList();
             foreach (MyLocation l in newlist)
             {
                 newPolygon.Locations.Add(new System.Device.Location.GeoCoordinate(l.latitude, l.longitude));
             }
             return newPolygon;
             
         }

          public static MyLocation randomPointInRectangle(List<MyLocation> gameArea,GeoCoordinate a,GeoCoordinate b)
          {
              MyLocation loc = new MyLocation();
              double randomX = Constants.random.NextDouble();
               double randomY = Constants.random.NextDouble();
               loc.latitude = a.Latitude + (randomX * (b.Latitude - a.Latitude));
               loc.longitude = a.Longitude + (randomY * (b.Longitude - a.Longitude));
               while (!pointInPolygon(gameArea,loc))
               {
                   randomX = Constants.random.NextDouble();
                   randomY = Constants.random.NextDouble();
                   loc.latitude = a.Latitude + (randomX * (b.Latitude - a.Latitude));
                   loc.longitude = a.Longitude + (randomY * (b.Longitude - a.Longitude));
               }
               return loc; 

            }
          public static MyPolygon rectangleInsidePolygon(List<MyLocation> list)
          {
             
              MyLocation mid = middlePoint(list);
              var minDistance = list[0].toGeoCoordinate().GetDistanceTo(mid.toGeoCoordinate());
              foreach (MyLocation g in list)
              {
                  Double distance = g.toGeoCoordinate().GetDistanceTo(mid.toGeoCoordinate());
                  if (distance < minDistance)
                      minDistance = distance;
              }
              MyPolygon newPolygon = new MyPolygon();
              // Defines the polygon fill details
              newPolygon.Fill = new SolidColorBrush(Colors.Red);
              newPolygon.Stroke = new SolidColorBrush(Colors.Red);
              newPolygon.StrokeThickness = 6;
              newPolygon.Opacity = 0.7;
              newPolygon.Locations = new LocationCollection();
              List<GeoCoordinate> help=drawCircle(new GeoCoordinate(mid.latitude , mid.longitude),(int)minDistance);
              GeoCoordinate lefttop = help[45]; 
              GeoCoordinate rightbot = help[225];
              newPolygon.Locations.Add(lefttop);
              newPolygon.Locations.Add(help[135]);
              newPolygon.Locations.Add(rightbot);
              newPolygon.Locations.Add(help[315]);
              return newPolygon;
              

          }
          public static MyLocation middlePoint(List<MyLocation> list)
          {
              var mid = new MyLocation();
              double lat = 0;
              double lon = 0;
              foreach (MyLocation g in list)
              {
                  lat += g.latitude;
                  lon += g.longitude;
              }
              mid.latitude = lat / list.Count;
              mid.longitude = lon / list.Count;
              return mid;
          }


        public static void userJoin(string p)
        {
            PhoneApplicationService service = PhoneApplicationService.Current;
            User user = (User)service.State["user"];
            user.activeGame = p;
            user.status = 1;
            user.updateCurrentUser();
            CoreTask.start();
        }

        internal static void randomWalk(List<MyLocation> gameArea,MyLocation myLocation)
        {
            var list=drawCircle(myLocation.toGeoCoordinate(), Constants.BOT_MOVEMENT);
            var val=Constants.random.Next(0,360);
            myLocation.latitude = list[val].Latitude;
            myLocation.longitude = list[val].Longitude;
            while(!pointInPolygon(gameArea,myLocation))
            {

                Debug.WriteLine("randomwalk");
                val=Constants.random.Next(0,360);
                myLocation.latitude = list[val].Latitude;
                myLocation.longitude = list[val].Longitude;
            }
        }

        internal static void zombieWalk(List<MyLocation> gameArea, MyLocation myLocation,MyLocation nextSurvivor)
        {
            var list = drawCircle(myLocation.toGeoCoordinate(), Constants.BOT_MOVEMENT);
            var distance = myLocation.toGeoCoordinate().GetDistanceTo(nextSurvivor.toGeoCoordinate());
            var val = Constants.random.Next(0, 360);
            for (int k = 0; k < 20;k++ )
                if (pointInPolygon(gameArea, myLocation) &&
                    (new GeoCoordinate(list[val].Latitude, list[val].Longitude).GetDistanceTo(nextSurvivor.toGeoCoordinate()) <= distance))
                {                    
                    myLocation.latitude = list[val].Latitude;
                    myLocation.longitude = list[val].Longitude;
                    break;
                }
        }
    }
}
