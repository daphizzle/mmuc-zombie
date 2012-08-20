using System;
namespace Parse
{
   public class GeoPoint
   {
      public double Latitude { get; set; }
      public double Longitude { get; set; }

      public GeoPoint() { }
      public GeoPoint(double latitude, double longitude)
      {
         Latitude = latitude;
         Longitude = longitude;
      }

      /// <summary>
      /// Strictly used by the query engine
      /// </summary>
      public bool NearSphere(double latitude, double longitude)
      {
         return NearSphere(latitude, longitude, null);
      }

      /// <summary>
      /// Strictly used by the query engine
      /// </summary>
      public bool NearSphere(double latitude, double longitude, double? maxDistance)
      {
          if (maxDistance == null)
          {
              maxDistance = 5;
          }
         return distance(latitude,longitude,this.Latitude,this.Longitude)<maxDistance;
      }

      //Helper method to compute the distance of two Gps-Coordinates
      private double distance(double pos1Lat, double pos1Lon, double pos2Lat, double pos2Lon)
      {
          double R = 6371;
          double dLat = this.toRadian(pos2Lat - pos1Lat);
          double dLon = this.toRadian(pos2Lon - pos1Lon);

          double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(this.toRadian(pos1Lat)) * Math.Cos(this.toRadian(pos2Lat)) *
              Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
          double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
          double d = R * c;

          return d;
      }

      //Helper method to convert into radian
      private double toRadian(double val)
      {
          return (Math.PI / 180) * val;
      }

   }
}