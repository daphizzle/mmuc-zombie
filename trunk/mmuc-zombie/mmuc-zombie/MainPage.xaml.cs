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
using Parse;
using System.Diagnostics;
using Microsoft.Phone.Shell;
using System.Device.Location;

namespace mmuc_zombie
{
    public partial class MainPage : PhoneApplicationPage,MyListener
    {

        PhoneApplicationService service = PhoneApplicationService.Current;
        List<MyParseObject> gameList;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }


        public void onDataChange(List<MyParseObject> list)
        {
            gameList = list;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ////Create 2 Games
            //Games gameNear1 = new Games();
            //gameNear1.latitude = 45;
            //gameNear1.longitude = 4;
            //gameNear1.name = "finished game";
            //gameNear1.state = 2;
            //gameNear1.create();

            //Games gameNear2 = new Games();
            //gameNear2.latitude = 43;
            //gameNear2.longitude = 5;
            //gameNear2.name = "pending game";
            //gameNear2.create();
            //gameNear2.state = 0;
            //Games.findPendingGames(this);
            

        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
        }
            

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/MapTest.xaml", UriKind.Relative));
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/Menu.xaml", UriKind.Relative));
        }

        private void facebookLink_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/FacebookConnect.xaml", UriKind.Relative));
        }
    }
}