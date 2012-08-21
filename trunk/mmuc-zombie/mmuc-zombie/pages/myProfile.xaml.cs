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
using Microsoft.Phone.Shell;

namespace mmuc_zombie.pages
{
    public partial class MyProfile : PhoneApplicationPage
    {
        
        User user;
        public MyProfile()
        {
            InitializeComponent();
            PhoneApplicationService service = PhoneApplicationService.Current;
            user = (User)service.State["user"];
            string userId=user.Id;
            var parse = new Driver();
            parse.Objects.Query<User>().Where(c => c.Id == c.Id).Execute(r =>
            {
                if (r.Success)
                {
                    List<User> users=(List<User>)r.Data.Results;
                    parse.Objects.Query<Friend>().Where(c => c.user ==userId ).Execute(r2 =>
                       {
                           if (r2.Success)
                           {
                               List<Friend> friends = (List<Friend>)r2.Data.Results;
                              Deployment.Current.Dispatcher.BeginInvoke(() =>
                              {
                                   if (!loadUsers(users,friends))
                                    {            
                                        userListBox.Visibility = System.Windows.Visibility.Collapsed;
                                    }
                                    else
                                    {
                                        userListBox.Visibility = System.Windows.Visibility.Visible;
                                    }
                               });
                           }
                       });
                  }
            });
       }
            
     

        private void appbar_save_Click(object sender, EventArgs e)
        {
            MessageBox.Show("save");
        }

        private void appbar_cancel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("cancel");
        }
        private bool loadUsers(List<User> users,List<Friend> friends)
        {
            /* TEST DATA */

            mmuc_zombie.components.friendsView tmpUI;
            //var _parse = new Driver();
            
            foreach (User tmp in users)
            {
                //_parse.Objects.Save(tmp);
               
                tmpUI = new mmuc_zombie.components.friendsView();
                foreach (Friend friend in friends)
                {
                    if (tmp.Id.Equals(friend.friend))
                    {
                        tmpUI.isFriend = true;
                        tmpUI.textBlock1.Text = "friend";
                    }
                }  
                tmpUI.nameTextBlock.Text = tmp.Id;
                tmpUI.userId = user.Id;
                tmpUI.friendId = tmp.Id;
                userStackPanel.Children.Add(tmpUI);
            }

            return userStackPanel.Children.Count > 0;
        }

    }
}