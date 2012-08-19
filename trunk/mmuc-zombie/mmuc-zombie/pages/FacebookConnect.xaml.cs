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

namespace mmuc_zombie.pages
{
    public partial class FacebookConnect : PhoneApplicationPage
    {
        public FacebookConnect()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            bool bWeAreLoggedIn = !string.IsNullOrEmpty(App.AccessToken);
            btnLogin.IsEnabled = !bWeAreLoggedIn;                       
            txtStatus.Text = bWeAreLoggedIn ? "User logged!" : "Login via facebook connect";
            txtError.Text = bWeAreLoggedIn ? App.AccessToken : "OK";
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/FacebookLogin.xaml", UriKind.Relative));
        }        
    }
}