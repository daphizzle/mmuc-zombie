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
using mmuc_zombie.app.helper;


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

    public Games() { }
    public void update(Action<Response<DateTime>> callback)
    {
        var parse = new Driver();
        parse.Objects.Update<Games>(this.Id).
               Set(u => u.state, state).
               Set(u => u.players, players).
               Set(u => u.privateGame, privateGame).
               Set(u => u.name, name).
               Set(u => u.radius, radius).
               Set(u => u.zombiesCount, zombiesCount).
               Set(u => u.startTime, startTime).
               Set(u => u.endTime, endTime).
               Set(u => u.description, description).
               Set(u => u.ownerId, ownerId).
               Execute(callback);
    }

    public new void update()
    {
        var parse = new Driver();
        parse.Objects.Update<Games>(this.Id).
            Set(u => u.state, state).
            Set(u => u.players, players).
            Set(u => u.privateGame, privateGame).
            Set(u => u.name, name).
            Set(u => u.radius, radius).
            Set(u => u.zombiesCount, zombiesCount).
            Set(u => u.startTime, startTime).
            Set(u => u.endTime, endTime).
            Set(u => u.locationId, locationId).
            Set(u => u.description, description).
            Set(u => u.ownerId,ownerId).
            Execute(r =>
            {
                if (r.Success)
                {
                    Debug.WriteLine("Game : " + Id + " successfull updated");
                }
                //else
                //{
                //    Debug.WriteLine("User : " + Id + " error while updating. " + r.Error.Message);
                //}

            });

    }

    public Games(string name, DateTime start, DateTime end, string ownerId, string description) 
    {
        this.name = name;
        this.startTime = startTime;
        this.endTime = endTime;
        this.ownerId = ownerId;
        this.description = description;
    }

    static public void findPendingGames(Action<Response<ResultsResponse<Games>>> callback)
    {
        var parse = new Driver();
        parse.Objects.Query<Games>().Where(c => c.state == 0).Execute(callback);
    }

    public void create(List<MyLocation> list, List<Invite> invites)
    {
        PhoneApplicationService service = PhoneApplicationService.Current;
        User user = (User)service.State["user"];

        var parse = new Driver();
        parse.Objects.Save(this, r =>
            {
                if (r.Success)
                {
                    Id = r.Data.Id;
                    foreach (MyLocation l in list)
                    {
                        l.gameId = Id;
                        l.create(r2 =>
                        {
                            if (r.Success)
                                Debug.WriteLine("(" + l.latitude + "," + l.longitude + ")");

                        });

                    }
                    foreach (Invite i in invites)
                    {
                        i.gameId = Id;
                        i.create(r2 =>
                            {
                                if (r.Success)
                                    Debug.WriteLine("Invite for user "+i.userId+" created");
                            });
                    }
                    user.status = 1;
                    user.activeGame = Id;
                    service.State["user"] = user; 
                    parse.Objects.Update<User>(user.Id).Set(u=>u.status,1).Set(u=>user.activeGame,Id).Execute(ro=>
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                
                                CoreTask.start();
                            });
                               });
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/pages/GameStart.xaml?gameId=" + Id, UriKind.Relative));
                        });
                }
            });
    }


    public static void findCustomGames(Action<Response<ResultsResponse<Invite>>> callback)
    {
        var parse = new Driver();
        PhoneApplicationService service = PhoneApplicationService.Current;
        User user = (User)service.State["user"];
        string userId=user.Id;
        List<MyParseObject> list = new List<MyParseObject>();
        parse.Objects.Query<Invite>().Where(c => c.userId == userId).Execute(callback);
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

    static public void findMyGames(Action<Response<ResultsResponse<Games>>> callback)
    {
        var parse = new Driver();
        PhoneApplicationService service = PhoneApplicationService.Current;
        User user = (User)service.State["user"];
        string userId = user.Id;
        List<MyParseObject> list = new List<MyParseObject>();
        parse.Objects.Query<Games>().Where(c => c.ownerId == userId).Execute(callback);
    }


    /*
     * Obsolete methods -- Be warned -- Do not use these
     */

    [Obsolete("Instead of this method user findMyGames")]
    static public void findByOwner(string ownerId, MyListener listener)
    {
        var parse = new Driver();
        PhoneApplicationService service = PhoneApplicationService.Current;
        User user = (User)service.State["user"];
        string userId = user.Id;
        List<MyParseObject> list = new List<MyParseObject>();
        parse.Objects.Query<Games>().Where(c => c.ownerId == ownerId).Execute(r =>
        {
            if (r.Success)
            {
                List<Games> games = (List<Games>)r.Data.Results;
                //int counter = 0;
                foreach (Games g in games)
                {
                    list.Add(g);
                    if (list.Count == games.Count)
                    {
                        listener.onDataChange(list);
                    }
                }
            }
        });
    }


    [Obsolete("Use other findCustomGames method")]
    internal static void findCustomGames(MyListener listener)
    {
        var parse = new Driver();
        PhoneApplicationService service = PhoneApplicationService.Current;
        User user = (User)service.State["user"];
        string userId = user.Id;
        List<MyParseObject> list = new List<MyParseObject>();
        parse.Objects.Query<Invite>().Where(c => c.userId == userId).Execute(r =>
        {
            if (r.Success)
            {
                List<Invite> invites = (List<Invite>)r.Data.Results;
                int counter = 0;
                foreach (Invite i in invites)
                {
                    parse.Objects.Get<Games>(i.gameId, r2 =>
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

    [Obsolete("Use other findPendingGames method now")]
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

}