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

namespace mmuc_zombie
{
    public partial class PanoramaPage1 : PhoneApplicationPage
    {
        public PanoramaPage1()
        {
            InitializeComponent();
        }

        private void profile_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/myProfile.xaml", UriKind.Relative));
        }

        private void fame_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/test.xaml", UriKind.Relative));
        }

        private void myGames_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/myGames.xaml", UriKind.Relative));
        }

        private void newGame_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/newGame.xaml", UriKind.Relative));
        }

        private void officialGames_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/officialGames.xaml", UriKind.Relative));
        }

        private void customGames_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/test.xaml", UriKind.Relative));
        }

    }
}