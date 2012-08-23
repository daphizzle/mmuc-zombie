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
using mmuc_zombie.app.listener;

namespace mmuc_zombie.app.helper
{
    public class PositionRetriever
    {
        static GeoCoordinateWatcher watcher;
        

        public static void startPositionRetrieving(int threshold)
        {
            // Reinitialize the GeoCoordinateWatcher
            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
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
            Debug.WriteLine("should update Location now!");
            PhoneApplicationService service = PhoneApplicationService.Current;
            User user = (User)service.State["user"];
            if (user.locationId != null)
            {
                double lat = e.Position.Location.Latitude;
                double lon = e.Position.Location.Longitude;
                var parse = new Driver();
                parse.Objects.Update<MyLocation>(user.locationId).Set(u => u.latitude, lat).Set(u => u.longitude, lon).Execute();
            }
            else
            {
                MyLocation loc = new MyLocation(e.Position.Location.Latitude, e.Position.Location.Longitude);
                loc.create(new LocationListener(user));
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
                    //TODO: display error ? 
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