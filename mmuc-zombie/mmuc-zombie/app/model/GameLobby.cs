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
using Parse;
using System.Collections.Generic;

namespace mmuc_zombie.app.model
{
    public class GameLobby : MyParseObject
     {
        public string gameId;
        public string userId;
        public bool creator;
        public string userName;


        public static void findPlayersByGameId(string gameId, MyListener listener)
        {
            var parser = new Driver();
            parser.Objects.Query<GameLobby>().Where(c => c.gameId == gameId).Execute(r =>
                {
                    if (r.Success)
                    {
                        List<MyParseObject> l = (List<MyParseObject>)r.Data.Results;
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                listener.onDataChange(l);
                            });
                    }
                });
        }

        public static void deleteGame(string gameId)
        {
            var parser = new Driver();
            parser.Objects.Query<GameLobby>().Where(c => c.gameId == gameId).Execute(r =>
            {
                if (r.Success)
                {
                    List<MyParseObject> l = (List<MyParseObject>)r.Data.Results;
                    foreach (MyParseObject o in l)
                    {
                        o.destroy<GameLobby>(null);
                    }
                }
            });
        }
     }
}
