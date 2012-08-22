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
    public class PendingGames : MyParseObject
    {
        public string gameId { get; set; }
        public string userId { get; set; }
        public int startTime { get; set; }

        static public void findUsersByGameId(string gameId, MyListener listener)
        {
            var parser = new Driver();
            parser.Objects.Query<Roles>().Where(c => c.gameId == gameId).Execute(r =>
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


        [Obsolete("Different way of finding pending games implemented")]
        public static void isPendingGamesStarted(User user, DateTime date, MyListener listener)
        {
            var parser = new Driver();
            string userId = user.Id;
            string dateString = "" + date.Year + date.DayOfYear + date.Hour + date.Minute;
            int dateInt = int.Parse(dateString);
            parser.Objects.Query<PendingGames>().Where(c => c.Id == user.Id && dateInt >= c.startTime).Execute(r =>
            {
                if (r.Success)
                {
                    var pendingGames = (List<MyParseObject>)r.Data.Results;
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            listener.onDataChange(pendingGames);
                        });
                }
            });
        }
        
    }
}
