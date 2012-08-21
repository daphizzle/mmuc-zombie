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
using mmuc_zombie.app.model;
using Microsoft.Phone.Shell;

namespace mmuc_zombie.pages
{
    public partial class GameStart : PhoneApplicationPage,MyListener
    {
        List<User> users = new List<User>();
        String gameId;
        String creatorId;


        public GameStart()
        {
            InitializeComponent();            
        }

        public void onDataChange(List<MyParseObject> l)
        {
            if (l[0] is GameLobby)
            {
                //start drawing the page etc.
                mmuc_zombie.components.playerView tmpUI;
                foreach (GameLobby tmp in l)
                {
                    tmpUI = new mmuc_zombie.components.playerView();
                    tmpUI.nameTextBlock.Text = tmp.userName;
                    tmpUI.Margin = new Thickness(0, 5, 0, 5);
                    playerStack.Children.Add(tmpUI);
                    if (tmp.creator)
                    {
                        creatorId = tmp.userId;
                    }
                }
                PhoneApplicationService service = PhoneApplicationService.Current;
                User user = (User)service.State["user"];
                if (creatorId == user.Id)
                {
                    startButton.Visibility = Visibility.Visible;
                }
            }
            if (l[0] is Games)
            {
                //update the state of the game
                Games game = (Games)l[0];
                game.state = 2;
                game.update();
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
                gameId = NavigationContext.QueryString["gameId"];
                GameLobby.findPlayersByGameId(gameId, this);
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            //set the status on game to active
            Games.findById(gameId, this);

            //delete all gamelobby data with the gameId
            GameLobby.deleteGame(gameId);
        }

    }
}