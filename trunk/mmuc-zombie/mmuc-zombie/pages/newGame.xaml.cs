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
using mmuc_zombie.app.helper;

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
        public List<Invite> invites=new List<Invite>();
        PhoneApplicationService service;
        private bool _editable;

        public NewGame()
        {
            InitializeComponent();

            PhoneApplicationService service = PhoneApplicationService.Current;
            User user = (User)service.State["user"];
            string userId = user.Id;
            var parser = new Driver();
           
                parser.Objects.Get<MyLocation>(user.locationId, r =>
                {
                    if (r.Success)
                    {
                        loc = r.Data;
                        parser.Objects.Query<Friend>().Where(c => c.user == userId).Execute(r2 =>
                        {
                            if (r2.Success)
                            {
                                List<Friend> friends = (List<Friend>)r2.Data.Results;
                                Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    MapWithPolygon.SetView(new LocationRect(new System.Device.Location.GeoCoordinate(loc.latitude, loc.longitude), 0.5, 0.5));
                                    MapWithPolygon.ZoomLevel = 13;
                                    if (!loadUsers(friends))
                                    {            
                                        userListBox.Visibility = System.Windows.Visibility.Collapsed;
                                    }
                                    else
                                    {
                                        userListBox.Visibility = System.Windows.Visibility.Visible;
                                    }
                                });
                            }
                        });
                       
                }
            });
                       
            
        }
        private bool loadUsers( List<Friend> friends)
        {
            /* TEST DATA */

            mmuc_zombie.components.inviteFriends tmpUI;
       
              
                
                foreach (Friend friend in friends)
                {
                     tmpUI = new mmuc_zombie.components.inviteFriends();
                     tmpUI.nameTextBlock.Text = friend.friend;
                     tmpUI.createInvite(friend.friend);
                     tmpUI.invites = invites;
                     userStackPanel.Children.Add(tmpUI);
            }

            return userStackPanel.Children.Count > 0;
        }
     
        

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Double value =Math.Round(playerSlider.Value*10.0);
            playerValueTextbox.Text = value.ToString() ;
        }


        private void saveButtonClick(object sender, EventArgs e)
        {
            if (newPolygon != null)
            {   int i=0;
                List<MyLocation> locs = new List<MyLocation>();
                foreach (GeoCoordinate p in newPolygon.Locations)
                {
                    var loc = new MyLocation(p.Latitude, p.Longitude);
                    loc.number=i++;
                    locs.Add(loc);
                }

                var game = new Games();
                PhoneApplicationService service = PhoneApplicationService.Current;
                var user = (User)service.State["user"];
                game.ownerId = user.Id;
                game.name = nameTextfield.Text;
                game.state = 0;
                game.description = descriptionBox.Text;
                game.players = (int)Math.Round(playerSlider.Value * 10.0);
                game.privateGame = privateCheckbox.IsChecked.Value;
                game.create(locs,invites);
                
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

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            PositionRetriever.startPositionRetrieving(100);
            playerSlider.Value = 3;

            editable();
        }

        private void editable()
        {
            String gameId = NavigationContext.QueryString["gameId"];
            if (!String.IsNullOrWhiteSpace(gameId))
            {
                _editable = true;
                service = PhoneApplicationService.Current;
                User user = User.get();
                //getGame(gameId, getGameCallback);
                //populate fields
                //save button -> update button (actions)
            }
        }

        


       

        

    }

 
}
