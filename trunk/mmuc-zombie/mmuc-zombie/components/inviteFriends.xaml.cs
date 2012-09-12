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
using System.Windows.Media.Imaging;

namespace mmuc_zombie.components
{
    public partial class inviteFriends : UserControl
    {
      
        public string gameId;
        public string userId;
         public bool isInvited=false;
         private Invite invite;
         public List<Invite> invites;
          public inviteFriends()
        {
            InitializeComponent();
        }
          public void createInvite(string userId)
          {
              invite = new Invite();
              invite.userId = userId;
              invite.accepted = false;
          }

        private void add_Click(object sender, RoutedEventArgs e)
            {
            if (!isInvited)
            {
                invites.Add(invite);
                tmpTextBlock.Text = "invited";
                isInvited = true;
                addButton.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("/mmuc-zombie;component/ext/img/del.jpg", UriKind.Relative)) };

            }
            else
            {
                invites.Remove(invite);
                tmpTextBlock.Text = "";
                isInvited = false;
                addButton.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("/mmuc-zombie;component/ext/img/_add.png", UriKind.Relative)) };
            }
        }
    }
}
