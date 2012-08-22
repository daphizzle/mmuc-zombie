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
using Parse;
using mmuc_zombie.app.helper;

namespace mmuc_zombie.pages
{
    public partial class GameView : PhoneApplicationPage, MyListener
    {
        PhoneApplicationService service = PhoneApplicationService.Current;
        String gameId;

        public GameView()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            gameId = NavigationContext.QueryString["gameId"];
            Games.findById(gameId,this);

        }

        private void JoinButton_Click(object sender, RoutedEventArgs e)
        {
            User user = (User)service.State["user"];
            var parse = new Driver();
            user.status = 1;
            user.activeGame = gameId;
            service.State["user"] = user;
            parse.Objects.Update<User>(user.Id).Set(u => u.status, 1).Set(u => user.activeGame, gameId).Execute(ro =>
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {

                        CoreTask.start();
                    });
                });
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/pages/GameStart.xaml?gameId=" + gameId, UriKind.Relative));
                });
            
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


            }
        }
    }


}