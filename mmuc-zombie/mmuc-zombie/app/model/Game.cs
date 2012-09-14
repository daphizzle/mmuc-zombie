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


public class Game : MyParseObject
{
    //pending = 0, active = 1, finshed = 2
    public int state { get; set; }
    public int players { get; set; }
    public Boolean privateGame { get; set; }
    public String name { get; set; }
    public int radius { get; set; }
    public int zombiesCount { get; set; }
    // Format: DD.MM.YY
    public string startDate { get; set; }
    public string locationId { get; set; }
    public string description { get; set; }
    public string ownerId { get; set; }
    public string hostId { get; set; }
    public string events { get; set; }
    //small = 0, medium = 1, big = 2
    public int size { get; set; }

    public Game() {
        events = "";
    }
    public void update(Action<Response<DateTime>> callback)
    {
        var parse = new Driver();
        parse.Objects.Update<Game>(this.Id).
               Set(u => u.state, state).
               Set(u => u.players, players).
               Set(u => u.privateGame, privateGame).
               Set(u => u.name, name).
               Set(u => u.radius, radius).
               Set(u => u.zombiesCount, zombiesCount).
               Set(u => u.startDate, startDate).
               Set(u => u.description, description).
               Set(u => u.ownerId, ownerId).
               Set(u => u.hostId, hostId).
               Set(u => u.events, events).
               Execute(callback);
    }



    static public void findPendingGames(Action<Response<ResultsResponse<Game>>> callback)
    {
        var parse = new Driver();
        parse.Objects.Query<Game>().Where(c => c.state == 0).Execute(callback);
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

    public static void findPlayedGames(Action<Response<ResultsResponse<Game>>> callback)
    {
        var parse = new Driver();
        PhoneApplicationService service = PhoneApplicationService.Current;
        User user = (User)service.State["user"];
        string userId = user.Id;
        List<MyParseObject> list = new List<MyParseObject>();
        parse.Objects.Query<Game>().Where(c => c.state != (int)Constants.GAMEMODES.FINISHED).Execute(callback);
    }

    internal static void findPlayedGamesByMe(Action<Response<Game>> callback)
    {
        var parse = new Driver();
        PhoneApplicationService service = PhoneApplicationService.Current;
        User user = (User)service.State["user"];
        string userId = user.Id;
        List<MyParseObject> list = new List<MyParseObject>();
        string gameID;

        parse.Objects.Query<Game>().Where(c => c.state != (int)Constants.GAMEMODES.FINISHED).Execute(r =>
        {
            if (r.Success)
            {
                List<Game> playedGames = (List<Game>)r.Data.Results;
                foreach(Game g in playedGames)
                {
                    gameID = g.Id;
                    parse.Objects.Query<Roles>().Where(c => c.gameId == gameID && c.userId == userId).Execute(r2 =>
                    {
                        if (r2.Success)
                        {
                            parse.Objects.Get<Game>(gameID, callback);
                        }
                    });
                }
            }
        });
    }

    static public void findMyGames(Action<Response<ResultsResponse<Game>>> callback)
    {
        var parse = new Driver();
        PhoneApplicationService service = PhoneApplicationService.Current;
        User user = (User)service.State["user"];
        string userId = user.Id;
        List<MyParseObject> list = new List<MyParseObject>();
        parse.Objects.Query<Game>().Where(c => c.ownerId == userId).Execute(callback);
    }


    static public void findRunningGames(Action<Response<ResultsResponse<Game>>> callback)
    {
        var parse = new Driver();
        parse.Objects.Query<Game>().Where(c => c.state == (int)Constants.GAMEMODES.ACTIVE).Execute(callback);
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
        parse.Objects.Query<Game>().Where(c => c.ownerId == ownerId).Execute(r =>
        {
            if (r.Success)
            {
                List<Game> games = (List<Game>)r.Data.Results;
                //int counter = 0;
                foreach (Game g in games)
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
                    parse.Objects.Get<Game>(i.gameId, r2 =>
                    {
                        if (r2.Success)
                        {
                            counter++;
                            Game game = (Game)r2.Data;
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
        parse.Objects.Query<Game>().Where(c => c.state == (int)Constants.GAMEMODES.PENDING).Execute(r =>
            {
                if (r.Success)
                {
                    List<Game> found = (List<Game>)r.Data.Results;
                    List<MyParseObject> list = new List<MyParseObject>();
                    foreach (Game g in found)
                    {
                        list.Add(g);
                    }
                    listener.onDataChange(list);
                }
            });
    }
    
}