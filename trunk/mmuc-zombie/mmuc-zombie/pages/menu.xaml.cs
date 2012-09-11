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

namespace mmuc_zombie.pages
{
    public partial class Menu : PhoneApplicationPage
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void profile_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/MyProfile.xaml", UriKind.Relative));
        }

        private void fame_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/HallFame.xaml", UriKind.Relative));
        }

        private void runningGame_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/RunningGames.xaml", UriKind.Relative));
        }

        private void newGame_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/NewGame.xaml", UriKind.Relative));
        }

        private void officialGames_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/OfficialGames.xaml", UriKind.Relative));
        }

        private void customGames_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/CustomGames.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Games.findPendingGames(this);
        }




    }
}