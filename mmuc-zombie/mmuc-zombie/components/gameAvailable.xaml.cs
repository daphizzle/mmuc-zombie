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
using Parse;
using mmuc_zombie.app.helper;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;

namespace mmuc_zombie.components
{
    public partial class gameAvailable : UserControl
    {
        public string gameID = "";

        public gameAvailable()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void join_Click(object sender, RoutedEventArgs e)
        {
            var parse = new Driver();
            Roles role = new Roles();
            role.roleType = Constants.ROLE_OBSERVER;
            
            User user = User.get();
            user.status = 1;
            user.activeGame = gameID;

            role.create(r => {
                if (r.Success)
                {
                    user.activeRole = r.Data.Id;                    
                    user.updateCurrentUser();

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {(
                           Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/pages/IngameView.xaml", UriKind.Relative));
                    });
                }
            });
        }

    }
}
