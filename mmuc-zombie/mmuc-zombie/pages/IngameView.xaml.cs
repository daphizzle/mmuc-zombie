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
                                    drawPins();
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
                                              drawPins();
                                      });
                            }
                        });


                    }
                }
            });

        }

        private void drawPins()
        {
            mapLayer.Children.Clear();
            for (int i = 0; i < userList.Count; i++)
            {
                var p = new Pushpin();
                Debug.WriteLine("-----------------------------------");
                Debug.WriteLine("User" + userList[i].Id);
                Debug.WriteLine("Location " + locationList[i]);
                Debug.WriteLine("Role" + roleList[i]);
                p.Location = new GeoCoordinate(locationList[i].latitude, locationList[i].longitude);
                p.Name = userList[i].Id;
                
               
            //    System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            //    img.Source = new BitmapImage(new Uri("/Images/myLocation.png", UriKind.Relative));
                if (roleList[i].roleType.Equals("Zombie"))
                    p.Background=new SolidColorBrush(Colors.Red);
                else
                    p.Background = new SolidColorBrush(Colors.Green);
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
                        map.SetView(new LocationRect(new System.Device.Location.GeoCoordinate(list[0].latitude, list[0].longitude), 0.5, 0.5));
                        map.ZoomLevel = 13;
                        gameAreaLayer.Children.Add(StaticHelper.drawPolygon(list, Colors.Yellow));

                    });
            }

        }
    }
}