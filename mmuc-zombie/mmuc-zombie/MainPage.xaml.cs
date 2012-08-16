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

namespace mmuc_zombie
{
    public partial class MainPage : PhoneApplicationPage
    {
      
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
            ParseConfiguration.Configure("w8I4cwfDTXeMzvPPSzkAiinbnkMWijhZkZ7Jnxwd", "BbL0rdiCCzC2yE0fdtm7da6nKEXdBt2EXDTHEvVT");
        }
            

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/mapTest.xaml", UriKind.Relative));
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/menu.xaml", UriKind.Relative));
        }
    }
}