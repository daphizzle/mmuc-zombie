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
using mmuc_zombie.app.model;
using Microsoft.Phone.Shell;
using Parse;
using mmuc_zombie.app.helper;
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace mmuc_zombie.pages
{
    public partial class IngameView : PhoneApplicationPage
    {
        List<User> userList;
        List<MyLocation> locationList;
        List<Roles> roleList;
        User user;
        Games game;
        private int nextPlayerCounter;
        private Roles role;
        private MyLocation myLocation;
        public IngameView()
        {
            InitializeComponent();
            PhoneApplicationService service = PhoneApplicationService.Current;
            user = (User)service.State["user"];
            drawGameBorder();
        }

        public void drawGameBorder()
        {
            Query.getGame(user.activeGame,getGameCallback);
            Query.getGameArea(user.activeGame, getGameAreaCallback); 
                
        }
        public void getPinsData()
        {
            locationList = new List<MyLocation>();
            roleList = new List<Roles>();
            Debug.WriteLine("getPinsData");
            Query.getUsersByGame(user.activeGame, r =>
            {
                if (r.Success)
                {
                    userList = (List<User>)r.Data.Results;
                    Debug.WriteLine("got Users");

                    foreach (User u in userList)
                    {
                        Query.getRole(u.activeRole, r0 =>
                        {
                            if (r0.Success)
                            {
                                Debug.WriteLine("got Roles");
                               
                                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                                      {
                                          roleList.Add(r0.Data);
                                if (userList.Count == roleList.Count && userList.Count == locationList.Count)
                                    doIngameStuff();
                                      });
                            }
                        });
                        Query.getLocation(u.locationId, r0 =>
                        {
                            if (r0.Success)
                            {
                                Debug.WriteLine("got Locations");
                               
                                Deployment.Current.Dispatcher.BeginInvoke(() =>
                                      {
                                          locationList.Add(r0.Data);
                                          if (userList.Count == roleList.Count && userList.Count == locationList.Count)
                                          {
                                              // doIngameStuff(); 
                                          }
                                      });
                            }
                        });


                    }
                }
            });

        }

        private void doIngameStuff()
        {
            if (user==null){
                PhoneApplicationService service = PhoneApplicationService.Current;
                user = (User)service.State["user"];
             }
            drawPins();
            if (nextPlayerCounter++ == 5)
               if(role.roleType=="Zombie"){
                   infectNearSurvivors();
               }
        }

        private void infectNearSurvivors()
        {
            for (int i = 0; i < locationList.Count; i++)
            {
                if (userList[i].activeRole == "Survivor" &&
                    myLocation.toGeoCoordinate().GetDistanceTo(locationList[i].toGeoCoordinate()) < 25)
                {
                    roleList[i].alive = false;
                    userList[i].activeGame = "";

                }

                
                }
            }

               
        

        private void drawPins()
        {
            debug.Text = "";
            mapLayer.Children.Clear();
            for (int i = 0; i < userList.Count; i++)
            {
                var p = new Pushpin();
                Debug.WriteLine("-----------------------------------");
                Debug.WriteLine("User" + userList[i].Id);
                p.Location = new GeoCoordinate(locationList[i].latitude, locationList[i].longitude);
                p.Name = userList[i].Id;
                debug.Text += "User: " + userList[i].Id + "\n Location (" + locationList[i].latitude + "," + locationList[i].longitude + ")\n";
                if (locationList[i].Id.Equals(user.locationId))
                        myLocation=locationList[i];
                
                if (roleList[i].Id.Equals(user.activeRole))
                        role=roleList[i];
                
                if (roleList[i].roleType.Equals("Zombie"))
                {
                    p.Style = (Style)(Application.Current.Resources["PushpinStyle2"]);
                    
                }
                else
                    p.Style = (Style)(Application.Current.Resources["PushpinStyle"]);
                MyLocation fu=new MyLocation();
                fu.fromGeoCoordinate(p.Location);
                StaticHelper.drawEllipse(fu, Colors.Red);
                mapLayer.Children.Add(p);
            }

        }
        


        public void getGameCallback(Response<Games> r)
        {
            if (r.Success)
            {
                game = r.Data;
            }
        }

    




























        public void getGameAreaCallback(Response<ResultsResponse<MyLocation>> r)
        {
            if (r.Success)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        var list = (List<MyLocation>)r.Data.Results;
                        double west = int.MaxValue;
                        double east = int.MinValue;
                        double north = int.MinValue;
                        double south = int.MaxValue;
                        foreach (MyLocation l in list)
                        {
                            if (l.latitude < west)
                                west = l.latitude;
                            if (l.latitude > east)
                                east = l.latitude;
                            if (l.longitude < south)
                                south = l.longitude;
                            if (l.longitude > north)
                                north = l.longitude;
                        }
                        var mid = new GeoCoordinate();
                        double lat = 0;
                        double lon = 0;
                        foreach (MyLocation g in list)
                        {
                            lat += g.latitude;
                            lon += g.longitude;
                        }
                        mid.Latitude = lat / list.Count;
                        mid.Longitude = lon / list.Count;
                        double widthE = east - mid.Latitude;
                        double widthW = mid.Latitude - west;
                        double heightN = north - mid.Longitude;
                        double heightS = mid.Longitude - south;
                        double width = 0;
                        double height = 0;
                        if (widthE > widthW)
                        {
                            width = widthE;
                        }
                        else
                        {
                            width = widthW;
                        }
                        if (heightN > heightS)
                        {
                            height = heightN;
                        }
                        else
                        {
                            height = heightS;
                        }
                        if (height > width)
                        {
                            width = height;
                        }
                        else
                        {
                            height = width;
                        }

                        map.SetView(new LocationRect(mid,height,width));
                        gameAreaLayer.Children.Add(StaticHelper.drawPolygon(list, Colors.Yellow));

                    });
            }

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            PositionRetriever.startPositionRetrieving(1);
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult mb = MessageBox.Show("Do you want to leave the game?", "Alert", MessageBoxButton.OKCancel);
            if (mb != MessageBoxResult.OK)
            {
                e.Cancel = true;
            }
            else
            {
                user.activeGame = "";
                user.status = 0;
                user.update();
                (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/pages/Menu.xaml", UriKind.Relative));
            }
        }



        
        //private void map_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    e.Handled = true;
        
        //}



        //private void map_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_MapZoom(object sender, MapZoomEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_KeyUp(object sender, KeyEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_KeyDown(object sender, KeyEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_MapPan(object sender, MapDragEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        //{
        //    e.Handled = true;
        //}


        //private void map_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_ViewChangeEnd(object sender, MapEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_ViewChangeStart(object sender, MapEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    e.Handled = true;
        //}

        
        //private void map_ViewChangeOnFrame(object sender, MapEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_TargetViewChanged(object sender, MapEventArgs e)
        //{
        //    e.Handled = true;
        //}



        //private void map_MouseEnter(object sender, MouseEventArgs e)
        //{
            
        //}

        //private void map_MouseLeave(object sender, MouseEventArgs e)
        //{

        //}

        //private void map_MouseMove(object sender, MouseEventArgs e)
        //{
            
        //}

    }
}