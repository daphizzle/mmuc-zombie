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
using System.Windows.Navigation;
using mmuc_zombie.app.helper;

namespace mmuc_zombie.components
{
    public partial class officialGame : UserControl
    {
        public String gameId;
        public officialGame()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void join_Click(object sender, RoutedEventArgs e)
        {
            var page = StaticHelper.GetParentOfType<PhoneApplicationPage>(this);
            page.NavigationService.Navigate(new Uri("/pages/GameView.xaml?gameId=" + gameId, UriKind.Relative));
        }



    }
}
