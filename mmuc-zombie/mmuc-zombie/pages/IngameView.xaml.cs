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
            Query.getUsersByGame(user.activeGame, r =>
            {
                if (r.Success)
                {
                    userList = (List<User>)r.Data.Results;
                    String[] locationIds = new String[userList.Count];
                    String[] roleIds = new String[userList.Count];
                    for (int i = 0; i < userList.Count; i++)
                    {
                        locationIds[i] = userList[i].locationId;
                        roleIds[i] = userList[i].activeRole;
                    }
                    Query.getRoles(user.activeGame, r0 =>
                    {
                        if (r0.Success)
                        {
                            roleList = (List<Roles>)r0.Data.Results;
                            Query.getLocations(user.activeGame, r1 =>
                            {
                                if (r1.Success)
                                {
                                    locationList = (List<MyLocation>)r1.Data.Results;
                                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                                    {
                                        drawPins();
                                    });
                                }
                            });
                        }
                    });
                }
            });

        }

        private void drawPins()
        {
            for (int i = 0; i < userList.Count; i++)
            {
                var p = new Pushpin();
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
                var list = (List<MyLocation>)r.Data.Results;
                mapLayer.Children.Add(StaticHelper.drawPolygon(list, Colors.White));
            }

        }
    }
}