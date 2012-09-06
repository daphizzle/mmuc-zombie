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
using mmuc_zombie.app.helper;

namespace mmuc_zombie
{
    public partial class MainPage : PhoneApplicationPage
    {

        PhoneApplicationService service = PhoneApplicationService.Current;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            // verifying whether the user is already logged on facebook or not.
            if (!String.IsNullOrWhiteSpace(App.AccessToken))
            {                                
                facebookLink.Visibility = System.Windows.Visibility.Collapsed;
            }
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
            NavigationService.Navigate(new Uri("/pages/FacebookLogin.xaml", UriKind.Relative));
        }
    }
}