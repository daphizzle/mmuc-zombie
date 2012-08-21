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

        private void join_Click(object sender, RoutedEventArgs e)
        {
            if (!isInvited)
            {
                invites.Add(invite);
                textBlock1.Text = "invited";
            }
            else
            {
                invites.Remove(invite);
                textBlock1.Text = "";
            }
        }
    }
}
