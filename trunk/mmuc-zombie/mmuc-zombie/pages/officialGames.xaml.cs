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
using Microsoft.Phone.Shell;

namespace mmuc_zombie.pages
{
    public partial class OfficialGames : PhoneApplicationPage
    {

        List<MyLocation> middlePoints = new List<MyLocation>();
        List<Game> games = new List<Game>();
        List<Game> gamesInRange = new List<Game>();
        public OfficialGames()
        {
            InitializeComponent();
            //Games.findPendingGames(this);
            Game.findPendingGames(displayPendingGamesCallback);
            
        }

        public void displayPendingGamesCallback(Response<ResultsResponse<Game>> r)
        {
            if (r.Success)
            {
                games = (List<Game>)r.Data.Results;
                int gameCounter = 0;
                var parse = new Driver();
                foreach (Game g in games)
                {
                    string id = g.Id;
                    parse.Objects.Query<MyLocation>().Where(c => c.gameId == id).Execute(r2 =>
                    {
                        if (r2.Success)
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {   
                                var list=(List<MyLocation>)r2.Data.Results;
                                drawPolygons(list);
                                if (inZoneRange(list))
                                    gamesInRange.Add(g);
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



        private bool inZoneRange(List<MyLocation> list)
        {
            PhoneApplicationService service = PhoneApplicationService.Current;
            MyLocation loc=(MyLocation)service.State["location"];
            return StaticHelper.pointInPolygon(list, loc);
        }

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
            var newlist=list.OrderBy(x=>x.number).ToList();
            foreach (MyLocation l in newlist)
            {
                newPolygon.Locations.Add(new System.Device.Location.GeoCoordinate(l.latitude,l.longitude));
            }
            NewPolygonLayer.Children.Add(newPolygon);
            MyPolygon rectangle = StaticHelper.rectangleInsidePolygon(list);
            NewPolygonLayer.Children.Add(rectangle);
            
            var tempLoc = new MyLocation(newPolygon.middlePoint().Latitude,newPolygon.middlePoint().Longitude);
            tempLoc.gameId = list[0].gameId;
         
            if (inZoneRange(list))
                middlePoints.Add(tempLoc);
        
        }

        private void drawPushPins()
        {
            foreach (MyLocation loc in middlePoints)
            {
                var p = new Pushpin();
                p.Location = new GeoCoordinate(loc.latitude, loc.longitude);
                p.MouseEnter += new System.Windows.Input.MouseEventHandler(polygonClick);
                p.Name =loc.gameId;
                p.Content = loc.gameId;
                mapLayer.Children.Add(p);
            }
         
            String locationId = User.getFromState().locationId;
            Query.getLocation(locationId, r =>
            {
                if (r.Success)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        Pushpin p = new Pushpin();
                        p.Location = r.Data.toGeoCoordinate();
                        p.Template = this.Resources["playerpin"] as ControlTemplate;
                        MapWithPolygon.Center = p.Location;
                        mapLayer.Children.Add(p);
                        MapWithPolygon.ZoomLevel = 16;
                    });
                }
            });
        }

        private void polygonClick(Object sender,MouseEventArgs e)
        {
            Pushpin p = (Pushpin)sender;
            StaticHelper.userJoin(p.Name);
            NavigationService.Navigate(new Uri("/pages/GameStart.xaml?gameId="+p.Name, UriKind.Relative));


        }

 	
        
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            PositionRetriever.startPositionRetrieving(100);
        }

        private bool loadGames()
        {
            mmuc_zombie.components.officialGame tmpUI;
         
            foreach (Game tmp in gamesInRange)
            {                
                tmpUI = new mmuc_zombie.components.officialGame();
                tmpUI.gameId = tmp.Id;
                tmpUI.gameName.Text = tmp.name;
                tmpUI.description.Text = tmp.description;
                tmpUI.Margin = new Thickness(0, 5, 0, 5);              
                gameStack.Children.Add(tmpUI);
            }

            return gameStack.Children.Count > 0;
        }

    }
}