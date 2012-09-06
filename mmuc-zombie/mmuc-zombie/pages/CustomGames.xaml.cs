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
using Microsoft.Phone.UserData;
using mmuc_zombie.app.model;
using Parse;
using System.Diagnostics;
using System.Device.Location;
using mmuc_zombie.app.helper;

namespace mmuc_zombie.pages
{
    public partial class CustomGames : PhoneApplicationPage
    {

        List<MyLocation> middlePoints = new List<MyLocation>();
        List<Games> games = new List<Games>();
        public CustomGames()
        {
            InitializeComponent();
            //Games.findCustomGames(this);
            Games.findCustomGames(displayCustomGames);

        }

        private void displayCustomGames(Response<ResultsResponse<Invite>> r)
        {
            var parse = new Driver();
            if (r.Success)
            {
                List<Invite> invites = (List<Invite>)r.Data.Results;
                foreach (Invite i in invites)
                {
                    parse.Objects.Get<Games>(i.gameId, r2 =>
                    {
                        if (r2.Success)
                        {
                            Games game = (Games)r2.Data;
                            games.Add(game);


                            if (games.Count == invites.Count)
                            {
                                int gameCounter = 0;
                                foreach(Games g in games)
                                {
                                    string id = g.Id;
                                    parse.Objects.Query<MyLocation>().Where(c => c.gameId == id).Execute(r3 =>
                                    {
                                        if (r3.Success)
                                        {
                                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                            {

                                                drawPolygons((List<MyLocation>)r3.Data.Results);
                                                gameCounter++;
                                                if (gameCounter == games.Count)
                                                {
                                                    drawPushPins();
                                                    if (!loadGames())
                                                    {
                                                        noResults.Visibility = System.Windows.Visibility.Visible;
                                                        gameList.Visibility = System.Windows.Visibility.Collapsed;
                                                    }
                                                    else
                                                    {
                                                        noResults.Visibility = System.Windows.Visibility.Collapsed;
                                                        gameList.Visibility = System.Windows.Visibility.Visible;
                                                    }
                                                }
                                            });
                                        }
                                    });
                                }
                            }
                        }
                    });
                }
            }
        }

        //Code cleanup: We now use direct callbacks instead of listeners for better readable and less widespread code
        //public void onDataChange(List<MyParseObject> list)
        //{
        //    var parse = new Driver();
        //    int gameCounter = 0;
        //    foreach (MyParseObject o in list)
        //    {
        //        string id = o.Id;
        //        games.Add((Games)o);

        //        parse.Objects.Query<MyLocation>().Where(c => c.gameId == id).Execute(r =>
        //        {
        //            if (r.Success)
        //            {
        //                Deployment.Current.Dispatcher.BeginInvoke(() =>
        //                {

        //                    drawPolygons((List<MyLocation>)r.Data.Results);
        //                    gameCounter++;
        //                    if (gameCounter == list.Count)
        //                    {
        //                        drawPushPins();
        //                        if (!loadGames())
        //                        {
        //                            noResults.Visibility = System.Windows.Visibility.Visible;
        //                            gameList.Visibility = System.Windows.Visibility.Collapsed;
        //                        }
        //                        else
        //                        {
        //                            noResults.Visibility = System.Windows.Visibility.Collapsed;
        //                            gameList.Visibility = System.Windows.Visibility.Visible;
        //                        }
        //                    }
        //                });
        //            }
        //        });
        //    }


        //}



        private void drawPolygons(List<MyLocation> list)
        {
            MyPolygon newPolygon = new MyPolygon();
            // Defines the polygon fill details
            newPolygon.Locations = new LocationCollection();
            Random rand = new Random();
            Color clr = Colors.White;
            switch (rand.Next(5))
            {
                case 0: clr = Colors.Blue; break;
                case 1: clr = Colors.Green; break;
                case 2: clr = Colors.Orange; break;
                case 3: clr = Colors.Yellow; break;
                case 4: clr = Colors.Red; break;
            }
            newPolygon.Fill = new SolidColorBrush(clr);
            newPolygon.Stroke = new SolidColorBrush(clr);
            newPolygon.StrokeThickness = 3;
            newPolygon.Opacity = 0.3;
            var newlist = list.OrderBy(x => x.number).ToList();
            foreach (MyLocation l in newlist)
            {
                newPolygon.Locations.Add(new System.Device.Location.GeoCoordinate(l.latitude, l.longitude));
            }
            NewPolygonLayer.Children.Add(newPolygon);

            var tempLoc = new MyLocation(newPolygon.middlePoint().Latitude, newPolygon.middlePoint().Longitude);
            tempLoc.gameId = list[0].gameId;
            middlePoints.Add(tempLoc);

        }

        private void drawPushPins()
        {
            foreach (MyLocation loc in middlePoints)
            {
                var p = new Pushpin();
                p.Location = new GeoCoordinate(loc.latitude, loc.longitude);
                p.MouseEnter += new System.Windows.Input.MouseEventHandler(polygonClick);
                p.Name = loc.gameId;
                p.Content = loc.gameId;
                NewPolygonLayer.Children.Add(p);
            }
            MapWithPolygon.SetView(new LocationRect(new System.Device.Location.GeoCoordinate(middlePoints[0].latitude, middlePoints[0].longitude), 0.5, 0.5));
            MapWithPolygon.ZoomLevel = 13;

        }

        private void polygonClick(Object sender, MouseEventArgs e)
        {
            Pushpin p = (Pushpin)sender;
            StaticHelper.userJoin(p.Name);
            NavigationService.Navigate(new Uri("/pages/GameStart.xaml?gameId=" + p.Name, UriKind.Relative));

        }



        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            PositionRetriever.startPositionRetrieving(100);
        }

        private bool loadGames()
        {
            

            mmuc_zombie.components.officialGame tmpUI;
           
            foreach (Games tmp in games)
            {
           
                tmpUI = new mmuc_zombie.components.officialGame();
                tmpUI.gameName.Text = tmp.name;
                tmpUI.description.Text = tmp.description;
                tmpUI.Margin = new Thickness(0, 5, 0, 5);
                gameStack.Children.Add(tmpUI);
            }

            return gameStack.Children.Count > 0;
        }

    }
}