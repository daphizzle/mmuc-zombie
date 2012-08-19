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

namespace mmuc_zombie.pages
{
    public partial class OfficialGames : PhoneApplicationPage
    {
        public OfficialGames()
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
            games.Add(new GameTmp("Zombie Informatiks", DateTime.Today, DateTime.Today, "Join us!", null));
            games.Add(new GameTmp("Zombie DFKI", DateTime.Today, DateTime.Today, "Join us!", null));
            games.Add(new GameTmp("Zombie VC", DateTime.Today, DateTime.Today, "Join us!", null));
            games.Add(new GameTmp("WP7 :)", new DateTime(2012, 09, 17), new DateTime(2012, 09, 17), "Join us!", null));
            /* TEST DATA */

            mmuc_zombie.components.officialGame tmpUI;
            //var _parse = new Driver();

            foreach (GameTmp tmp in games)
            {                
                //_parse.Objects.Save(tmp);
                
                tmpUI = new mmuc_zombie.components.officialGame();
                tmpUI.gameName.Text = tmp.Name;
                tmpUI.startTime.Text = tmp.Start.ToShortDateString();
                tmpUI.endTime.Text = tmp.End.ToShortDateString();
                tmpUI.description.Text = tmp.Description.Equals("") ? "No description." : tmp.Description;
                tmpUI.Margin = new Thickness(0, 5, 0, 5);              
                gameStack.Children.Add(tmpUI);
            }

            return gameStack.Children.Count > 0;
        }

    }
}