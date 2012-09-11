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
namespace mmuc_zombie.components

{
    public partial class friendsView : UserControl
    {
        public string friendId { get; set; }
        public string userId{get;set;}
        public Boolean isFriend=false;

        public friendsView()
        {
            InitializeComponent();
        }       
        private void add_Click(object sender, RoutedEventArgs e)
        {   
            if (!isFriend){
                var friend = new Friend();
                friend.friend=friendId;
                friend.user=userId;
                friend.create(r=>
                {
                    Debug.WriteLine(friendId+" is now a friend of "+userId);
                });

                tmpTextBlock.Text="friend";
            }
        }

       
    }
}
