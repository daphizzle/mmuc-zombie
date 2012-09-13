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
using Microsoft.Phone.Controls;

namespace mmuc_zombie.components

{
    public partial class facebookFriendView : UserControl        
    {
        public string fbUserId { get; set; }

        public facebookFriendView()
        {
            InitializeComponent();
        }

        private void comment_Click(object sender, RoutedEventArgs e)
        {
            postWall();
        }

        /**
         * Send a message to the current friend.
         * The message contains either an invitation to the current/next game, or an invitation to use the app
         */ 
        private void postWall()
        {
            string strUri = string.Format("/pages/facebookWallPostView.xaml?id={0}", this.fbUserId);            
            //NavigationService.Navigate(new Uri(strUri, UriKind.Relative));
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(strUri, UriKind.Relative));
            
        }
    }
}
