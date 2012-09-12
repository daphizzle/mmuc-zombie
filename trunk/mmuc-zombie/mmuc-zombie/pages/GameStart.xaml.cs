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
using Parse;
using mmuc_zombie.app.helper;
using System.Device.Location;
using System.Diagnostics;

namespace mmuc_zombie.pages
{
    public partial class GameStart : PhoneApplicationPage
    {
        private Game game;
        private User user;
        PhoneApplicationService service;
        private int botCounter;
        private int i;
        private  List<User> userList;

        public GameStart()
        {
            InitializeComponent();            
        }

     
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
                Progressbar.ShowProgressBar();
                String gameId = NavigationContext.QueryString["gameId"];
                service = PhoneApplicationService.Current;
                user = (User)service.State["user"];
                getGame(gameId, getGameCallback);
        }

        private void getGame(String gameId,Action<Response<Game>> callback)
        {
            var parse = new Driver();
            parse.Objects.Get<Game>(gameId, callback);
        }
        public void getGameCallback(Response<Game> r)
        {
            if (r.Success)
            {
                game = (Game)r.Data;
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    gameName.Text = game.name;

                    if (game.ownerId.Equals(user.Id))
                    {
                        startButton.Visibility = Visibility.Visible;
                        bots.Visibility = Visibility.Visible;
                    }
                });
            }
        }
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            Query.getUsersByGame(user.activeGame, getUsersPerGame);
       
            
        }
        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult mb = MessageBox.Show("Do you want to leave the game?", "Alert", MessageBoxButton.OKCancel);
            if (mb == MessageBoxResult.OK)
            {
                PhoneApplicationService service = PhoneApplicationService.Current;
                user = (User)service.State["user"];
                var parse = new Driver();
                if (game.ownerId.Equals(user.Id))
                {

                    parse.Objects.Update<Game>(game.Id).Set(u => u.state, 3).Execute(ro => { });
                }
                else
                {
                    user.status = 0;
                    user.activeGame = "";
                    service.State["user"] = user;
                    parse.Objects.Update<User>(user.Id).Set(u => u.status, 0).Set(u => user.activeGame, "").Execute(ro =>
                    {
                    });
                }
                CoreTask.idleMode();
                NavigationService.Navigate(new Uri("/pages/Menu.xaml", UriKind.Relative));

            }
            else
                e.Cancel = true;
         
        }


        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            PhoneApplicationService service = PhoneApplicationService.Current;
             user = (User)service.State["user"];
             var parse = new Driver();
            if (game.ownerId.Equals(user.Id))
            {
                
                parse.Objects.Update<Game>(game.Id).Set(u=>u.state,3).Execute(ro=>{});
            }
            else
            {
                user.status = 0;
                user.activeGame = "";
                service.State["user"] = user;
                parse.Objects.Update<User>(user.Id).Set(u => u.status, 0).Set(u => user.activeGame, "").Execute(ro =>
                {
                });
            }
            CoreTask.idleMode();
            NavigationService.Navigate(new Uri("/pages/Menu.xaml", UriKind.Relative));

        }
        private void createRoles()
        {
            var parse = new Driver();
            if (i < userList.Count)
            {
                Roles role = new Roles();
                role.gameId = userList[i].activeGame;
                role.userId = userList[i].Id;
                //role.startTime = DateTime.Now;
                role.alive = true;
                if (userList[i].bot && ifZombie(i))
                {
                    role.roleType = "Zombie";

                }
                else
                {
                    role.roleType = "Survivor";
                    role.maxLife = 5;
                }
               
                parse.Objects.Save(role, r1 =>
                {
                    if (r1.Success)
                    {
                        Debug.WriteLine("create role: " + r1.Data.Id);
                        userList[i].activeRole = r1.Data.Id;        
                           userList[i].update(r2 =>
                           {
                              
                                i++;
                                createRoles();
                                Debug.WriteLine("Assigned role " + r1.Data.Id + " to " + userList[i].Id);
                       });
                    }
                });
            }
            else
            {
                i = 0;
                Quest quest = new Quest();
                quest.gameId = game.Id;
                quest.create(r =>
                {
                    if (r.Success)
                    {
                        startGame();
                    }
                });
                
            }


        }
        private void startGame()
        {   
            var parse = new Driver();
            game.state = 1;
            game.update(r=>
                {
                    if(r.Success)
                        Debug.WriteLine("Game started");
                });
        }
        public void getUsersPerGame (Response<ResultsResponse<User>> r)
        {
            if (r.Success)
            {
                userList = (List<User>)r.Data.Results;
                Debug.WriteLine("Get Users per Game");
                createRoles();                              
            }

        }

        private bool ifZombie(int i)
        {
            return i > (userList.Count / 3);
        }
  
        //Classes which are needed to select between Datatemplates
        public abstract class DataTemplateSelector : ContentControl
        {
            public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                return null;
            }

            protected override void OnContentChanged(object oldContent, object newContent)
            {
                base.OnContentChanged(oldContent, newContent);

                ContentTemplate = SelectTemplate(newContent, this);
            }
        }

        public class UserMessageTemplateSelector : DataTemplateSelector
        {
            public DataTemplate messageTemplate { get; set; }
            public DataTemplate playerTemplate { get; set; }

            public override DataTemplate SelectTemplate(object item,
              DependencyObject container)
            {
                if (item is User)
                    return playerTemplate;
                return messageTemplate;
            }
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            string message = user.UserName + ": " + messageInput.Text;
            messageInput.Text = "";
            Message msg = new Message();
            msg.gameId = game.Id;
            msg.msg = message;
            msg.create(r =>
            {
                if (r.Success)
                    Debug.WriteLine("Message created");
            });
        }

        private void bots_Click(object sender, RoutedEventArgs e)
        {
            Query.getGameArea(game.Id,paintBots);
       
        }
        private void paintBots(Response<ResultsResponse<MyLocation>> r){
            if (r.Success)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    var list = (List<MyLocation>)r.Data.Results;
                    var polygon = StaticHelper.rectangleInsidePolygon(list);
                    MyLocation l = StaticHelper.randomPointInRectangle(list,polygon.Locations[0], polygon.Locations[2]);
                    var parse = new Driver();
               
                parse.Objects.Save(l,r2=>{
                    if(r2.Success)
                    {
                           User b=new User();
                           b.UserName="Bot "+botCounter++;
                           b.bot = true;
                           b.activeGame=user.activeGame;
                           b.locationId=r2.Data.Id;
                           b.create(r3=>
                           {
                               if (r.Success)
                                   Debug.WriteLine("Bot with Id " + r3.Data.Id + " created. lat: "+l.latitude+" ,long: "+l.longitude);
                           });
                    }
                });
                });
            }
        }
  
      
      
          
       
    }
}