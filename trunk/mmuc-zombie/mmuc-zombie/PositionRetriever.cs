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

namespace mmuc_zombie
{
    public class PositionRetriever
    {
        GeoCoordinateWatcher watcher;
        PhoneApplicationService service = PhoneApplicationService.Current;


        public PositionRetriever()
        {
            // Reinitialize the GeoCoordinateWatcher
            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            watcher.MovementThreshold = 100;//distance in metres

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
        void onPositionChanged(GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            User user = (User)service.State["user"];
            if (user.locationId != null)
            {
                Location loc = new Location(e.Position.Location.Latitude,e.Position.Location.Longitude);
                loc.Id = user.locationId;
                loc.update();
            }
            else
            {
                Location loc = new Location(e.Position.Location.Latitude, e.Position.Location.Longitude);
                loc.create(new LocationListener(user));
            }
        }

        /// <summary>
        /// Custom method called from the StatusChanged event handler
        /// </summary>
        /// <param name="e"></param>
        void onStatusChanged(GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    //TODO: display error
                    break;
                case GeoPositionStatus.Initializing:
                    //TODO: display error
                    break;
                case GeoPositionStatus.NoData:
                    //TODO: display error
                    break;
                case GeoPositionStatus.Ready:
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
        void watcherStatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => onStatusChanged(e));

        }

        /// <summary>
        /// Handler for the PositionChanged event. This invokes MyStatusChanged on the UI thread and
        /// passes the GeoPositionStatusChangedEventArgs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void watcherPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => onPositionChanged(e));
        }

        #endregion
    }
}
