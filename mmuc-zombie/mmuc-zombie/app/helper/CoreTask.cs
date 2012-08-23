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
using Parse;
using System.Diagnostics;
using mmuc_zombie.pages;

namespace mmuc_zombie.app.helper
{
    public class CoreTask 
    {

        static DispatcherTimer timer = new DispatcherTimer();
        static User user;
        static PhoneApplicationService service;
        static private List<User>lobbyUserList;
        static private List<Message> messageList;
 
        static public void start()
        {
            timer.Tick += new EventHandler(timerTask);
            timer.Interval = new TimeSpan(0, 0, 10);
            service = PhoneApplicationService.Current;
            user = (User)service.State["user"];
            timer.Start();
        }


        static public void timerTask (object Sender,EventArgs e){
            if (user.status==0)
                idleMode();
            else if (user.status==1)
                lobbyMode();
            else if (user.status==2)
                ingameMode();
        }

        static private void lobbyMode()
        {
            Debug.WriteLine("LobbyMode");
            check_GameStarted(check_GameStartedCallback);
            reload_LobbyUserList(reload_LobbyUserListCallback);
            reload_ChatWindow(reload_ChatWindowCallback);
        }

        static public void reload_ChatWindowCallback(Response<ResultsResponse<Message>> r)
        {
            if (r.Success)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    messageList = (List<Message>)r.Data.Results;
                    var currentPage = ((PhoneApplicationFrame)Application.Current.RootVisual).Content;
                    var mypage = (GameStart)currentPage;
                    mypage.chatWindow.ItemsSource = messageList;
                });
            }
        }

        private static void reload_ChatWindow(Action<Response<ResultsResponse<Message>>> callback)
        {
            string gameId = user.activeGame;
            var parse = new Driver();
            parse.Objects.Query<Message>().Where(c => c.gameId == gameId).Execute(callback);
        }

        static public void reload_LobbyUserListCallback(Response<ResultsResponse<User>> r)
        {
            if (r.Success)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        lobbyUserList = (List<User>)r.Data.Results;
                        var currentPage = ((PhoneApplicationFrame)Application.Current.RootVisual).Content;
                        var mypage = (GameStart)currentPage;
                        mypage.playerList.ItemsSource = lobbyUserList;
                    });
            }
        }
        static private void reload_LobbyUserList(Action<Response<ResultsResponse<User>>> callback)
        {
            string gameId = user.activeGame;
            var parse = new Driver();
            parse.Objects.Query<User>().Where(c => c.activeGame == gameId).Execute(callback);
        }
        static private void ingameMode()
        {
            Debug.WriteLine("Ingame Mode");
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                var currentPage = ((PhoneApplicationFrame)Application.Current.RootVisual).Content;
                if(currentPage is GameStart)
                {
                    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/pages/Menu.xaml", UriKind.Relative));
                    currentPage= ((PhoneApplicationFrame)Application.Current.RootVisual).Content;
                }
                var myPage=(IngameView)currentPage;
                myPage.getPinsData();
            });          
        }
    

        static public void idleMode()
        {
            Debug.WriteLine("Idle Mode");
            //bei einem join wird ein neuer timer gestartetund userstate auf 1 gesetzt
            timer.Stop();
        }

         static private void check_GameStarted( Action<Response<Games>> callback)
        {   
            string gameId=user.activeGame;
            var parse = new Driver();
            parse.Objects.Get<Games>(gameId,callback);

        }
        static public void check_GameStartedCallback(Response<Games> r)
        {
            if(r.Success)
            {
                var parse = new Driver();
                var game = r.Data;
                if (game.state == 1) //falls auf gamestart geklickt wird sollte game.state auf 1 gesetzt werden
                {

                    user.status = 2;
                    service.State["user"] = user;
                    parse.Objects.Update<User>(user.Id).Set(u => u.status, 2).Execute(ro =>
                    {
                    });
                    //switching to gameview
                }
                else if (game.state == 3)
                {
                    user.status = 0;
                    user.activeGame = "";
                    service.State["user"] = user;
                    parse.Objects.Update<User>(user.Id).Set(u => u.status, 0).Set(u => user.activeGame, "").Execute(ro =>
                    {
                    });
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/pages/Menu.xaml", UriKind.Relative));

                        });
                }
            }
        }

        static private  T ListFindHelper<T>(DependencyObject parentElement) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parentElement);
            if (count == 0)
                return null;

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parentElement, i);

                var listboxchild = child;

                if (child != null && child is StackPanel && (child as StackPanel).Name.Equals("panelwithtwo"))
                {
                    if (VisualTreeHelper.GetChildrenCount(child) > 1)
                    {
                        listboxchild = VisualTreeHelper.GetChild(child, 1);
                        var nm = (listboxchild as ListBox).Name;
                        if (listboxchild is ListBox && (listboxchild as ListBox).Name.Equals("recom_list"))
                        {
                            (listboxchild as ListBox).ItemsSource = lobbyUserList;
                            return (T)listboxchild;
                        }
                    }
                }
                else
                {

                    var result = ListFindHelper<T>(child);
                }
            }
            return null;
        }











        [Obsolete("new way implemented")]
        public void check_pendingGameStartedCallback(Response<ResultsResponse<PendingGames>> r)
        {
             if (r.Success)
             {   
                var pendingGames = (List<PendingGames>)r.Data.Results;
                if (pendingGames.Count > 0)
                {
                    user.status = 2;
                    user.activeGame=pendingGames[0].gameId;
                    user.update();
                    Debug.WriteLine("Status 2-------" + pendingGames[0].gameId);
                    //open Lobbyview , sollte nur ein game sein.
                }
                else check_noPendingGames(check_noPendingGamesCallBack);
             }
        }

        [Obsolete("new way implemented")]
        private void  check_noPendingGamesCallBack(Response<ResultsResponse<PendingGames>> r)
        {
             if (r.Success)
             {
                 if (r.Data.Results.Count == 0)
                 {
                     user.status = 0;
                     user.update();
                     //switching to idle mode
                 }
             }
        }

        [Obsolete("new way implemented")]
        private void check_noPendingGames(Action<Response<ResultsResponse<PendingGames>>> callback)
        {
            string userId = user.Id;
            var parse = new Driver();
            parse.Objects.Query<PendingGames>().Where(c => c.Id == user.Id).Execute(callback);
        }
        [Obsolete("new way implemented")]
        private void pendingMode()
        {
            //default status leaved der user aus einer lobby/game, oder ein game endet oder wird abgebrochen, switchen alle user back in den mode
            //ein user wird in die pending liste geadded in dem  moment wo er ein game joined. Falls ein game für einen user beendet ist wird er wieder gelöscht. falls er a
            check_pendingGameStarted(check_pendingGameStartedCallback);
        }
       [Obsolete("new way implemented")]
        private void check_pendingGameStarted(Action<Response<ResultsResponse<PendingGames>>> callback)
        {
            string userId = user.Id;
            var parse = new Driver();
            DateTime date = DateTime.Now;
            string dateString = "" + date.Year + date.DayOfYear + date.Hour + date.Minute;
            int dateInt = int.Parse(dateString);
            parse.Objects.Query<PendingGames>().Where(c => c.Id == user.Id && dateInt >= c.startTime).Execute(callback);

        }

       
    }
 }

        
               