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
using Microsoft.Phone.UserData;
using mmuc_zombie.app.model;
using Parse;
using mmuc_zombie.app.helper;

namespace mmuc_zombie.pages
{
    public partial class RunningGames : PhoneApplicationPage
    {

        List<Game> games = new List<Game>();

        public RunningGames()
        {
            InitializeComponent();
            Game.findRunningGames(displayGamesCallback);
        }

        private void displayGamesCallback(Response<ResultsResponse<Game>> r)
        {
            if (r.Success)
            {
                games = (List<Game>)r.Data.Results;
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (!loadRunningGames())
                    {
                        noResults.Visibility = System.Windows.Visibility.Visible;
                        gameList.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        noResults.Visibility = System.Windows.Visibility.Collapsed;
                        gameList.Visibility = System.Windows.Visibility.Visible;
                    }
                });
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //PositionRetriever.startPositionRetrieving(100);            
        }


        //Code cleanup: We now use direct callbacks instead of listeners for better readable and less widespread code
        //public void onDataChange(List<MyParseObject> list)
        //{
        //    var parse = new Driver();            
        //    foreach (MyParseObject o in list)
        //    {
        //        string id = o.Id;
        //        games.Add((Games)o);               
        //    }

        //    Deployment.Current.Dispatcher.BeginInvoke(() =>
        //    {
        //        if (!loadGames())
        //        {
        //            noResults.Visibility = System.Windows.Visibility.Visible;
        //            gameList.Visibility = System.Windows.Visibility.Collapsed;
        //        }
        //        else
        //        {
        //            noResults.Visibility = System.Windows.Visibility.Collapsed;
        //            gameList.Visibility = System.Windows.Visibility.Visible;
        //        }
        //    });
        //}

        private bool loadRunningGames()
        {
            mmuc_zombie.components.gameAvailable tmpUI;            
            foreach (Game tmp in games)
            {                                                
                tmpUI = new mmuc_zombie.components.gameAvailable();                
                tmpUI.gameID = tmp.Id;
                tmpUI.gameName.Text = tmp.name;
                tmpUI.startTime.Text = DateTime.Today.ToShortDateString(); // tmp.startTime.Value.ToShortDateString();
                tmpUI.endTime.Text = DateTime.Today.ToShortDateString(); // tmp.endTime.Value.ToShortDateString();
                tmpUI.Margin = new Thickness(0, 5, 0, 5);              
                gameStack.Children.Add(tmpUI);
                //tmpUI.edit.Visibility = tmp.ownerId.Equals(User.get().Id) ? Visibility.Visible : Visibility.Collapsed;                
            }
            return gameStack.Children.Count > 0;
        }

    }
}