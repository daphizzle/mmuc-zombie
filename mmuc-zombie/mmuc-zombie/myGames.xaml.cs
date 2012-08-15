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
    public partial class Page1 : PhoneApplicationPage
    {
        public Page1()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //SearchContacts();
        }

        //private void SearchContacts()
        //{
        //    List<GameTmp> games = new List<GameTmp>(3);
        //    games.Add(new GameTmp("Zombie 1", new DateTime(2012, 08, 15), new DateTime(2012, 08, 15)));
        //    ContactResultsData.DataContext = games;
        //}

        //private void ContactResultsData_Tap(object sender, GestureEventArgs e)
        //{
        //    App.con = ((sender as ListBox).SelectedValue as Contact);

        //    NavigationService.Navigate(new Uri("/ContactDetails.xaml", UriKind.Relative));
        //}
    }
}