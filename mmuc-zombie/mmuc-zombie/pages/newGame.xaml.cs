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
using Microsoft.Phone.Shell;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;
using Parse;
using mmuc_zombie.app.model;
using System.Device.Location;

namespace mmuc_zombie.pages
{
    public partial class NewGame : PhoneApplicationPage
    {
        // The user defined polygon to add to the map.
        MapPolygon newPolygon = null;
        // The map layer containing the polygon points defined by the user.
        MapLayer polygonPointLayer = new MapLayer();
        // Determines whether the map is accepting user polygon points
        // through single mouse clicks.
        bool inCreatePolygonMode = false;
        MyLocation loc;
        public NewGame()
        {
            InitializeComponent();

            PhoneApplicationService service = PhoneApplicationService.Current;
            User user = (User)service.State["user"];
            var parser = new Driver();
           
                parser.Objects.Get<MyLocation>(user.locationId, r =>
                {
                    if (r.Success)
                    {
                        loc = r.Data;
                         Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {   
                        
                        MapWithPolygon.SetView(new LocationRect(new System.Device.Location.GeoCoordinate(loc.latitude, loc.longitude), 0.5, 0.5));
                        MapWithPolygon.ZoomLevel = 13;

                        });
                    }
                });

            
        }

   

     
        

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Double value =Math.Round(playerSlider.Value*10.0);
            playerValueTextbox.Text = value.ToString() ;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void saveButtonClick(object sender, EventArgs e)
        {
            if (newPolygon != null)
            {
                List<MyLocation> locs = new List<MyLocation>();
                foreach (GeoCoordinate p in newPolygon.Locations)
                {
                    locs.Add(new MyLocation(p.Latitude, p.Longitude));
                }

                var game = new Games();
                PhoneApplicationService service = PhoneApplicationService.Current;
                var user = (User)service.State["user"];
                game.ownerId = user.Id;
                game.name = nameTextfield.Text;
                game.locationId = user.locationId;
                game.state = 0;
                game.players = (int)Math.Round(playerSlider.Value * 10.0);
                game.startTime = new DateTime(startDatePicker.Value.Value.Year, startDatePicker.Value.Value.Month,
                    startDatePicker.Value.Value.Day, startTimePicker.Value.Value.Hour,
                    startTimePicker.Value.Value.Minute, 0);
                game.endTime = new DateTime(endDatePicker.Value.Value.Year, endDatePicker.Value.Value.Month,
                    endDatePicker.Value.Value.Day, endTimePicker.Value.Value.Hour, endTimePicker.Value.Value.Minute, 0);
                game.privateGame = privateCheckbox.IsChecked.Value;
                game.create(locs);
            }
      

        }

        private void cancleButtonClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/menu.xaml", UriKind.Relative));
        }

        private void MapWithPolygon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //If the map is accepting polygon points, create a point.
            if (inCreatePolygonMode)
            {
                // Creates a location for a single polygon point and adds it to
                // the polygon's point location list.
                Location polygonPointLocation = MapWithPolygon.ViewportPointToLocation(e.GetPosition(MapWithPolygon));
                newPolygon.Locations.Add(polygonPointLocation);

                // A visual representation of a polygon point.
                System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();
                r.Fill = new SolidColorBrush(Colors.Red);
                r.Stroke = new SolidColorBrush(Colors.Yellow);
                r.StrokeThickness = 1;
                r.Width = 8;
                r.Height = 8;

                // Adds a small square where the user clicked, to mark the polygon point.
                polygonPointLayer.AddChild(r, polygonPointLocation);
            }
        }

        private void btnCreatePolygon_Click(object sender, RoutedEventArgs e)
        {
                        // Toggles ON the CreatePolygonMode flag.
            if (((Button)sender).Tag.ToString() == "false")
            {
                ((Button)sender).Tag = "true";
                inCreatePolygonMode = true;

                txtButton.Text = "Save Area";
                // Clears any objects added to the polygon layer.
                if(NewPolygonLayer.Children.Count > 0)
                    NewPolygonLayer.Children.Clear();

                // Adds the layer that contains the polygon points
                NewPolygonLayer.Children.Add(polygonPointLayer);

                newPolygon = new MapPolygon();
                // Defines the polygon fill details
                newPolygon.Locations = new LocationCollection();
                newPolygon.Fill = new SolidColorBrush(Colors.Blue);
                newPolygon.Stroke = new SolidColorBrush(Colors.Green);
                newPolygon.StrokeThickness = 3;
                newPolygon.Opacity = 0.8;
   
            }
            //Toggle OFF the CreatePolygonMode flag.
            else
            {
                ((Button)sender).Tag = "false";
                inCreatePolygonMode = false;

                txtButton.Text = "Draw Area";
 

                
                //If there are two or more points, add the polygon layer to the map
                if (newPolygon.Locations.Count >= 2)
                {
                    // Removes the polygon points layer.
                    polygonPointLayer.Children.Clear();

                    // Adds the filled polygon layer to the map.
                    NewPolygonLayer.Children.Add(newPolygon);
                }
            }
        }

        public static bool pnpoly(int nvert, MyLocation[] locs, double testx, double testy)
        {
            int i, j;
            bool c = false;
            for (i = 0, j = nvert - 1; i < nvert; j = i++)
            {
                if (((locs[i].longitude > testy) != (locs[j].longitude > testy)) &&
                 (testx < (locs[j].latitude - locs[i].latitude) * (testy - locs[i].longitude) / (locs[j].longitude - locs[i].longitude) + locs[i].latitude))
                    c = !c;
            }
            return c;
        }
       

        

    }

 
}
