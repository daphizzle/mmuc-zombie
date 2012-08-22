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
using Microsoft.Phone.Shell;
using System.Collections.Generic;
using mmuc_zombie.app.model;
using System.Diagnostics;
using Microsoft.Phone.Controls;


public class Games : MyParseObject
{
    //pending = 0, waiting = 1, active = 2, finshed = 3
    public int state { get; set; }
    public int players { get; set; }
    public Boolean privateGame { get; set; }
    public String name { get; set; }
    public int radius { get; set; }
    public int zombiesCount { get; set; }
    public DateTime? startTime { get; set; }
    public DateTime? endTime { get; set; }
    public string locationId { get; set; }
    public string description { get; set; }
    public string ownerId { get; set; }


    //static public void findNearActiveGames(MyListener listener)
    //{
    //    PhoneApplicationService service = PhoneApplicationService.Current;
    //    var parse = new Driver();
    //    User user = (User)service.State["user"];
    //    var latitudeOffsetUp = user.latitude + 10;
    //    var latitudeOffsetDown = user.latitude - 10;
    //    parse.Objects.Query<Games>().Where(c => c.latitude < latitudeOffsetUp && c.latitude > latitudeOffsetDown).Execute(r =>
    //    {
    //        if (r.Success)
    //        {
    //            List<Games> found = (List<Games>)r.Data.Results;
    //            List<MyParseObject> list = new List<MyParseObject>();
    //            foreach (Games g in found)
    //            {
    //                list.Add(g);
    //            }
    //            listener.onDataChange(list);
    //        }
    //    });


    //}

    static public void findPendingGames(MyListener listener)
    {
        var parse = new Driver();
        parse.Objects.Query<Games>().Where(c => c.state == 0).Execute(r =>
            {
                if (r.Success)
                {
                    List<Games> found = (List<Games>)r.Data.Results;
                    List<MyParseObject> list = new List<MyParseObject>();
                    foreach (Games g in found)
                    {
                        list.Add(g);
                    }
                    listener.onDataChange(list);
                }
            });
    }

    public void create(List<MyLocation> list, List<Invite> invites)
    {
        var parse = new Driver();
        parse.Objects.Save(this, r =>
            {
                if (r.Success)
                {
                    Id = r.Data.Id;
                    foreach (MyLocation l in list)
                    {
                        l.gameId = Id;
                        Debug.WriteLine("(" + l.latitude + "," + l.longitude + ")");
                        l.create();

                    }
                    foreach (Invite i in invites)
                    {
                        i.gameId = Id;  
                        i.create();
                    }
                    var pendingGame = new PendingGames();
                    pendingGame.gameId = Id;
                    pendingGame.userId = this.ownerId;
                    pendingGame.create();
                    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/pages/GameStart.xaml?gameId=" + Id, UriKind.Relative));
                }
            });
    }


    internal static void findCustomGames(MyListener listener)
    {
        var parse = new Driver();
        PhoneApplicationService service = PhoneApplicationService.Current;
        User user = (User)service.State["user"];
        string userId=user.Id;
        List<MyParseObject> list = new List<MyParseObject>();
        parse.Objects.Query<Invite>().Where(c => c.userId == userId).Execute(r =>
            {
                if (r.Success)
                {
                    List<Invite> invites=(List<Invite>)r.Data.Results;
                    int counter = 0;
                    foreach (Invite i in invites)
                    {  
                      parse.Objects.Get<Games>(i.gameId,r2 =>
                      {
                        if (r2.Success)
                         {
                             counter++;
                             Games game = (Games)r2.Data;
                             list.Add(game);
                             if (counter == invites.Count)
                             {
                                 listener.onDataChange(list);
                             }
                        }
                     });
                  }
                }
            });
    }

    static public void findById(string gameId, MyListener listener)
    {
        var parse = new Driver();
        parse.Objects.Get<Games>(gameId,(r=>
            {
                if(r.Success)
                {
                    var game = r.Data;
                    List<MyParseObject> l = new List<MyParseObject>();
                    l.Add(game);
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            listener.onDataChange(l);
                        });
                }
            }));
    }




}