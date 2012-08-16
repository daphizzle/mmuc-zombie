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


namespace mmuc_zombie
{
    public partial class myGames : PhoneApplicationPage
    {
        public myGames()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
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

        private bool loadGames()
        {
            /* TEST DATA */
            List<GameTmp> games = new List<GameTmp>(3);
            games.Add(new GameTmp("Zombie Informatiks", DateTime.Today, DateTime.Today));
            games.Add(new GameTmp("Zombie DFKI", DateTime.Today, DateTime.Today));
            games.Add(new GameTmp("Zombie VC", DateTime.Today, DateTime.Today));
            /* TEST DATA */

            mmuc_zombie.components.myGameAvailable tmpUI;            

            foreach (GameTmp tmp in games)
            {
                tmpUI = new mmuc_zombie.components.myGameAvailable();
                tmpUI.gameName.Text = tmp.Name;
                tmpUI.startTime.Text = tmp.Start.ToShortDateString();
                tmpUI.endTime.Text = tmp.End.ToShortDateString();
                tmpUI.Margin = new Thickness(0, 5, 0, 5);              
                gameStack.Children.Add(tmpUI);
            }

            return gameStack.Children.Count > 0;
        }
    }
}