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
using System.Device.Location;
using Microsoft.Phone.Shell;
using Parse;
using mmuc_zombie.app.model;
using System.Diagnostics;

namespace mmuc_zombie.app.helper
{
    public class PositionRetriever
    {
        static GeoCoordinateWatcher watcher;
        

        public static void startPositionRetrieving(int threshold)
        {
            // Reinitialize the GeoCoordinateWatcher
            //if (watcher != null)
            //    watcher.Stop();
            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
            watcher.MovementThreshold = threshold;//distance in metres

            // Add event handlers for StatusChanged and PositionChanged events
            watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcherStatusChanged);
            watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcherPositionChanged);

            // Start data acquisition

            watcher.Start();
        }


        /// <summary>
        /// Custom method called from the PositionChanged event handler
        /// </summary>
        /// <param name="e"></param>
        static void onPositionChanged(GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            PhoneApplicationService service = PhoneApplicationService.Current;
         
            User user = (User)service.State["user"];
            if (!user.locationId.Equals(""))
            {
                double lat = e.Position.Location.Latitude;
                double lon = e.Position.Location.Longitude;
                var parse = new Driver();
                parse.Objects.Update<MyLocation>(user.locationId).Set(u => u.latitude, lat).Set(u => u.longitude, lon).Execute(r =>
                    {
                        MyLocation loc = new MyLocation();
                            loc.latitude = lat;
                            loc.longitude = lon;
                            service.State["location"] =loc;
                    });

            }
            else
            {
                MyLocation loc = new MyLocation(e.Position.Location.Latitude, e.Position.Location.Longitude);
                loc.create(r =>
                {
                    if (r.Success)
                    {
                        service.State["location"] =loc;
                        user.locationId = r.Data.Id;
                        Debug.WriteLine("Location with Lat: " + e.Position.Location.Latitude + ", Long: " + e.Position.Location.Longitude + " created");
                        user.update(r2=>
                        {
                            Debug.WriteLine("Location "+r.Data.Id+" assigned to User "+user.Id);
                            user.saveToState();
                        });
                    }
                });

            }
        }

        /// <summary>
        /// Custom method called from the StatusChanged event handler
        /// </summary>
        /// <param name="e"></param>
        static void onStatusChanged(GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    Debug.WriteLine("Watcher disabled");
                    break;
                case GeoPositionStatus.Initializing:
                    //Don´t do anything data aquisition should start soon
                    break;
                case GeoPositionStatus.NoData:
                    Debug.WriteLine("Watcher got no Data");
                    
                    //TODO: display error ? or just wait for new Data
                    break;
                case GeoPositionStatus.Ready:
                    Debug.WriteLine("Watcher running!");
                    //This is the good case, don´t display errors
                    break;

            }
        }


        #region Event Handlers

        /// <summary>
        /// Handler for the StatusChanged event. This invokes MyStatusChanged on the UI thread and
        /// passes the GeoPositionStatusChangedEventArgs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void watcherStatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => onStatusChanged(e));

        }

        /// <summary>
        /// Handler for the PositionChanged event. This invokes MyStatusChanged on the UI thread and
        /// passes the GeoPositionStatusChangedEventArgs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void watcherPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => onPositionChanged(e));
        }

        #endregion
    }
}