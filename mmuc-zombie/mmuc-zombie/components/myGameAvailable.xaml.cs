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

namespace mmuc_zombie.components
{
    public partial class myGameAvailable : UserControl
    {
        public string gameID = "";

        public myGameAvailable()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void join_Click(object sender, RoutedEventArgs e)
        {
            var parse = new Driver();
            User user = User.get();
            user.status = 1;
            user.activeGame = gameID;
            User.set(user); 

            parse.Objects.Update<User>(user.Id).Set(u=>u.status,1).Set(u=>user.activeGame,gameID).Execute(ro=>
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {                                
                    CoreTask.start();
                });
            });

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {(
                    Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/pages/GameStart.xaml?gameId=" + gameID, UriKind.Relative));
                });
        } 
    }
}
