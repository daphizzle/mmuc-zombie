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
          var earthRadius = 6371;
      
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
    



        public static void userJoin(string p)
        {
            PhoneApplicationService service = PhoneApplicationService.Current;
            User user = (User)service.State["user"];
            user.activeGame = p;
            user.status = 1;
            user.updateCurrentUser();
            CoreTask.start();
        }
    }
}
