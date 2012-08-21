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
using Microsoft.Phone.Shell;
using mmuc_zombie.app.model;

namespace mmuc_zombie.pages
{
    public partial class GameView : PhoneApplicationPage, MyListener
    {
        PhoneApplicationService service = PhoneApplicationService.Current;
        Roles role = new Roles();

        public GameView()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            String gameId = NavigationContext.QueryString["gameId"];
            Games.findById(gameId,this);
            PendingGames.findUsersByGameId(gameId, this);
        }

        private void JoinButton_Click(object sender, RoutedEventArgs e)
        {
            User user = (User)service.State["user"];
            role.userId = user.Id;
            role.create();
                            
        }

        public void onDataChange(List<MyParseObject> l)
        {
            //Check the type of MyParseObject
            var o = l[0];
            if (o is Games)
            {
                //in this case there is only one MyParseObject
                //display data
                Games game = (Games)o;
                PageTitle.Text = game.name;
                MaxPlayers.Text = ""+game.players;
                StartTime.Text = ((DateTime)game.startTime).ToString();
                EndTime.Text = ((DateTime)game.endTime).ToString();
                Description.Text = game.description;

                //set the relevant data for joining the game
                role.gameId = game.Id;
                role.startTime = game.startTime;
                role.endTime = game.endTime;
            }
            if (o is PendingGames)
            {
                //there are many roles, but we only need the count
                RegPlayers.Text = "" + l.Count;
            }
        }
    }
}