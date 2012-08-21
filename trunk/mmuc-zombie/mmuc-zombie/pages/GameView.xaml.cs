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

namespace mmuc_zombie.pages
{
    public partial class GameView : PhoneApplicationPage, MyListener
    {
        public GameView()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            Games.findById(NavigationContext.QueryString["gameId"],this);
        }

        private void JoinButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public void onDataChange(List<MyParseObject> l)
        {
            //Check the type of MyParseObject
            var o = l[0];
            if (o is Games)
            {
                //in this case there is only one MyParseObject
                Games game = (Games)o;
                PageTitle.Text = game.name;
                MaxPlayers.Text = ""+game.players;
                StartTime.Text = ((DateTime)game.startTime).ToString();
                EndTime.Text = ((DateTime)game.endTime).ToString();
                Description.Text = game.description;
            }
            if (o is User)
            {
                //there are many users, but we only need the count
                RegPlayers.Text = "" + l.Count;
            }
        }
    }
}