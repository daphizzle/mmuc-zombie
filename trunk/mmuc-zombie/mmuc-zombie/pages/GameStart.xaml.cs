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

namespace mmuc_zombie.pages
{
    public partial class GameStart : PhoneApplicationPage
    {
        private Games game;
        private User user;
        PhoneApplicationService service;
       

        public GameStart()
        {
            InitializeComponent();            
        }

     
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
                String gameId = NavigationContext.QueryString["gameId"];
                service = PhoneApplicationService.Current;
                user = (User)service.State["user"];
                getGame(gameId,getGameCallback);
        }

        private void getGame(String gameId,Action<Response<Games>> callback)
        {
            var parse = new Driver();
            parse.Objects.Get<Games>(gameId, callback);
        }
        public void getGameCallback(Response<Games> r)
        {
            if (r.Success)
            {
                game = (Games)r.Data;
                gameName.Text = game.name;
                if (game.ownerId == user.Id)
                {
                    startButton.Visibility = Visibility.Visible;
                }
            }
        }
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            game.state = 1;
            var parse = new Driver();
            parse.Objects.Update<Games>(game.Id).Set(u => u.state, 1).Execute(ro => { });
            fillRolesPerGameTable();
            
        }

        private void fillRolesPerGameTable()
        {
            Query.getUsersByGame(user.activeGame, addRolesPerGame);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            PhoneApplicationService service = PhoneApplicationService.Current;
             user = (User)service.State["user"];
             var parse = new Driver();
            if (game.ownerId.Equals(user.Id))
            {
                
                parse.Objects.Update<Games>(game.Id).Set(u=>u.state,3).Execute(ro=>{});
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
        public void addRolesPerGame (Response<ResultsResponse<User>> r)
        {
            if (r.Success)
            {
                List<User> list = (List<User>)r.Data.Results;
                int i = 0;
                foreach (User u in list)
                {
                    Roles role=new Roles();
                    role.gameId=user.activeGame;
                    role.userId=user.Id;
                    role.startTime = DateTime.Now;
                    if (ifZombie(i++,list.Count))
                        role.roleType="Zombie";
                    role.create();


                }
            }

        }

        private bool ifZombie(int i, int p)
        {
            return i > p / 4 * 3;
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
            msg.create();
        }

      
      
          
       
    }
}