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
using System.Diagnostics;
using System.Windows.Media.Imaging;
namespace mmuc_zombie.components

{
    public partial class friendsView : UserControl
    {
        public string friendId { get; set; }
        public string newfriend { get; set; }
        public string userId{get;set;}
        public Boolean isFriend=false;

        public friendsView()
        {
            InitializeComponent();
        }
        private void add_Click(object sender, RoutedEventArgs e)
        {
            if (!isFriend)
            {
                var friend = new Friend();
                friend.friend = newfriend;
                friend.user = userId;
                friend.create(r =>
                {
                    Debug.WriteLine(friendId + " is now a friend of " + userId);
                    isFriend = true;
                    friendId = r.Data.Id;
                });

                tmpTextBlock.Text = "friend";
                addButton.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("/mmuc-zombie;component/ext/img/del.jpg", UriKind.Relative)) };
            }
            else
            {
                isFriend = false;
                var parse = new Driver();
                var friend = new Friend();
                friend.Id = friendId;
                parse.Objects.Delete<Friend>(friend, r =>
                {
                    if (r.Success)
                    {
                        Debug.WriteLine(newfriend + " is now no friend of " + userId);
                    }
                });
                    tmpTextBlock.Text = "";
                    addButton.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("/mmuc-zombie;component/ext/img/_add.png", UriKind.Relative)) };
                



            }

        }
    }
}
