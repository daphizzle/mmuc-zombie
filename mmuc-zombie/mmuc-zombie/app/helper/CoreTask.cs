using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Phone.Shell;
using mmuc_zombie.app.model;
using System.Collections.Generic;
using Microsoft.Phone.Controls;

namespace mmuc_zombie.app.helper
{
    public class CoreTask : MyListener
    {

        DispatcherTimer timer = new DispatcherTimer();
        User user;
        Games game;
        PhoneApplicationService service = PhoneApplicationService.Current;

        public void start()
        {
            timer.Tick += new EventHandler(timerTask);
            timer.Interval = new TimeSpan(0, 1, 0);
            user = (User)service.State["user"];
            //to see the state of the user write it into the service
            service.State["state"] = "idle";
            timer.Start();
        }


        public void timerTask (object Sender,EventArgs e)
        {
            if (service.State["state"] == "idle")
            {
                //the user is currently not in a game, look if the a game has started
                DateTime date = DateTime.Now;
                PendingGames.isPendingGamesStarted(user, date, this);

            }
            if (service.State["state"] == "waiting")
            {
                //user is now in the gamelobby, waiting for the creator of the game to start the game
                Games.
            }
            if (service.State["state"] == "active")
            {
                //user is currently in a game
                //navigate to mapview

            }

//
            //
        }

        public void onDataChange(List<MyParseObject> l)
        {
            if (l[0] is PendingGames)
            {
                //game is about to be started
                PendingGames pGame = (PendingGames)l[0];
                //set status of game to waiting
                Games.findById(pGame.gameId, this);
                //navigate user to gameLobby
                (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/pages/GameStart.xaml?gameId="+pGame.gameId, UriKind.Relative));
                //set state to waiting
                service.State["state"] = "waiting";
            }
            if (l[0] is Games)
            {
                Games game = (Games)l[0];
                game.state = 1;
                game.update();
            }

        }

        public void test()
        {
            
        }
    }
}
