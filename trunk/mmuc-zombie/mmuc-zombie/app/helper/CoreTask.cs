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
            Debug.WriteLine("TimerTask");
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
                    if (currentPage is GameStart)
                    {
                        var mypage = (GameStart)currentPage;
                        mypage.chatWindow.ItemsSource = messageList;
                    }
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
                        var help= new List<LobbyUsers>();
                        foreach (User u in lobbyUserList)
                        {
                            var lu=new LobbyUsers();
                            lu.userId=u.Id;
                            lu.picture = u.getPictureString();
                            lu.name=u.UserName;
                            help.Add(lu);
                        }
                            
                        var currentPage = ((PhoneApplicationFrame)Application.Current.RootVisual).Content;
                        if (currentPage is GameStart)
                        {
                            var mypage = (GameStart)currentPage;
                            if (mypage.playerList.ItemsSource == null)
                                Progressbar.HideProgressBar();
                            mypage.playerList.ItemsSource = help;
                        }
                    });
            }
        }
        static public void reload_LobbyUserList(Action<Response<ResultsResponse<User>>> callback)
        {
            string gameId = user.activeGame;
            var parse = new Driver();
            parse.Objects.Query<User>().Where(c => c.activeGame == gameId).Execute(callback);
        }
        static private void ingameMode()
        {
         

            Query.getRole(user.activeRole, check_IsAliveCallback);
            Debug.WriteLine("Ingame Mode");
            
               
                //{
                //    Debug.WriteLine("switching to Gamestart");
                //    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/pages/IngameView.xaml", UriKind.Relative));
                  
                //}
                //var myPage=(IngameView)currentPage;
                //myPage.getPinsData();
                      
        }

        static private void check_IsAliveCallback(Response<Roles> r)
        {
            if (r.Success)
            {
                Roles role = r.Data;
                if (role.alive)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                     {

                         var currentPage = ((PhoneApplicationFrame)Application.Current.RootVisual).Content;
                         if (!(currentPage is IngameView))
                         {   
                             (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/pages/IngameView.xaml", UriKind.Relative));
                         Debug.WriteLine("switch in ingamemode");
                         }
                         else
                         {
                             var myPage = (IngameView)currentPage;
                         myPage.loadData();
                         }
                     });
                }
                else
                {
                    Debug.WriteLine("U got Killed");    
                    
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            idleMode();
                            MessageBoxResult mb = MessageBox.Show("Sorry, you just died. But you can join another game!", "Alert", MessageBoxButton.OK);
                           (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/pages/Menu.xaml", UriKind.Relative));
                              
                            
                        });
                }
            }
        }
        static public void idleMode()
        {
            Debug.WriteLine("Idle Mode");
            //bei einem join wird ein neuer timer gestartetund userstate auf 1 gesetzt
           
                           timer.Stop();
                   
        }

         static private void check_GameStarted( Action<Response<Game>> callback)
        {   
               Query.getUser(user.Id, r =>
                {
                    if (r.Success)
                        user = r.Data;
                        user.saveToState();    
                        string gameId = user.activeGame;
                        var parse = new Driver();
                        parse.Objects.Get<Game>(gameId, callback);
                        

                });
            
        }
        static public void check_GameStartedCallback(Response<Game> r)
        {
            if(r.Success)
            {
               
                var parse = new Driver();
                var game = r.Data;
                Debug.WriteLine("got game " + game.Id);
                if (game.state == 1) //falls auf gamestart geklickt wird sollte game.state auf 1 gesetzt werden
                {
                    Debug.WriteLine("game "+game.Id+" started ");
                    user.status = 2;
                    service.State["user"] = user;
                    user.update();
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/pages/IngameView.xaml", UriKind.Relative));
                        });
                    //switching to gameview
                }
                else if (game.state == 3)
                {
                    user.status = 0;
                    user.activeGame = "";
                    service.State["user"] = user;
                    user.update();
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/pages/Menu.xaml", UriKind.Relative));

                        });
                }
            }
        }
       
    }
 }

        
               
