using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;

namespace mmuc_zombie.pages
{
    public partial class MapTest : PhoneApplicationPage
    {
        bool initializeZoom = true;
        double zoomFactor;
        double areaHeight;
        double areaWidth;
        GeoCoordinateWatcher watcher;
        public MapTest()
        {
            InitializeComponent();
        }



        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            // Reinitialize the GeoCoordinateWatcher
            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            watcher.MovementThreshold = 100;//distance in metres

            // Add event handlers for StatusChanged and PositionChanged events
            watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
            watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);

            // Start data acquisition
            watcher.Start();


            //hide button
            btnStart.Visibility = Visibility.Collapsed;
            button1.Visibility = Visibility.Visible;
            button2.Visibility = Visibility.Visible;
        }




        /// <summary>
        /// Custom method called from the PositionChanged event handler
        /// </summary>
        /// <param name="e"></param>
        void MyPositionChanged(GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            //set fake position for testing purpose
            //e.Position.Location = new GeoCoordinate(49.233, 7);


            // Update the map to show the current location
            mapMain.SetView(new LocationRect(e.Position.Location, 0.5, 0.5));
            mapMain.ZoomLevel = 13;

            //update pushpin and area location and show
            MapLayer.SetPosition(ppLocation, e.Position.Location);
            MapLayer.SetPositionOrigin(ppLocation, PositionOrigin.Center);
            MapLayer.SetPosition(area, e.Position.Location);
            MapLayer.SetPositionOrigin(area, PositionOrigin.Center);
            ppLocation.Visibility = System.Windows.Visibility.Visible;
            area.Visibility = System.Windows.Visibility.Visible;

        }

        /// <summary>
        /// Custom method called from the StatusChanged event handler
        /// </summary>
        /// <param name="e"></param>
        void MyStatusChanged(GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    // The location service is disabled or unsupported.
                    // Alert the user
                    tbStatus.Text = "sorry we can’t find you on this device";
                    break;
                case GeoPositionStatus.Initializing:
                    // The location service is initializing.
                    // Disable the Start Location button
                    tbStatus.Text = "looking for you…";
                    break;
                case GeoPositionStatus.NoData:
                    // The location service is working, but it cannot get location data
                    // Alert the user and enable the Stop Location button
                    tbStatus.Text = "can’t find you yet…";
                    ResetMap();

                    break;
                case GeoPositionStatus.Ready:
                    // The location service is working and is receiving location data
                    // Show the current position and enable the Stop Location button
                    tbStatus.Text = "found you!";
                    break;

            }
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //lazy initialization of initial zoomlevel and areasize
            if (initializeZoom)
            {
                zoomFactor = mapMain.ZoomLevel;
                initializeZoom = false;
                areaHeight = area.Height;
                areaWidth = area.Width;
            }
            // decrease zoom level
            double zoomlevel = ++mapMain.ZoomLevel;
            // adapt size of area
            if (zoomlevel >= zoomFactor)
            {
                area.Height = areaHeight * (1+zoomlevel - zoomFactor);
                area.Width = areaWidth * (1+zoomlevel - zoomFactor);
            }
            else
            {
                area.Height = areaHeight * (-1 / (zoomlevel - zoomFactor-1));
                area.Width = areaWidth * (-1 / (zoomlevel - zoomFactor-1));
            }
            tbStatus.Text = "multyplied by (H,W): (" + (area.Height / areaHeight) + "," + (area.Width / areaWidth) + ")";
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //lazy initialization of initial zoomlevel and areasize
            if (initializeZoom)
            {
                zoomFactor = mapMain.ZoomLevel;
                initializeZoom = false;
                areaHeight = area.Height;
                areaWidth = area.Width;
            }
            // decrease zoom level
            double zoomlevel = --mapMain.ZoomLevel;
            // adapt size of area            
            if (zoomlevel >= zoomFactor)
            {
                area.Height = areaHeight * (1+zoomlevel - zoomFactor);
                area.Width = areaWidth * (1+zoomlevel - zoomFactor);
            }else
            {
                area.Height = areaHeight * (-1 / (zoomlevel - zoomFactor-1));
                area.Width = areaWidth * (-1 / (zoomlevel - zoomFactor-1));
            }
            tbStatus.Text = "multyplied by (H,W): (" + (area.Height / areaHeight) + "," + (area.Width / areaWidth) + ")";
        }


        void ResetMap()
        {
            mapMain.SetView(new LocationRect(new GeoCoordinate(0,0,0), 10.0, 10.0));

            //update pushpin and area location and hide
            MapLayer.SetPosition(ppLocation, new GeoCoordinate(0, 0, 0));
            MapLayer.SetPosition(area, new GeoCoordinate(0, 0, 0));
            ppLocation.Visibility = System.Windows.Visibility.Collapsed;
            area.Visibility = System.Windows.Visibility.Collapsed;
        }



        #region Event Handlers

        /// <summary>
        /// Handler for the StatusChanged event. This invokes MyStatusChanged on the UI thread and
        /// passes the GeoPositionStatusChangedEventArgs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => MyStatusChanged(e));

        }

        /// <summary>
        /// Handler for the PositionChanged event. This invokes MyStatusChanged on the UI thread and
        /// passes the GeoPositionStatusChangedEventArgs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => MyPositionChanged(e));
        }

        #endregion




    }
}