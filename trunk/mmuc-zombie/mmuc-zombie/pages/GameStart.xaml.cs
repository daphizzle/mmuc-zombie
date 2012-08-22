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

namespace mmuc_zombie.pages
{
    public partial class GameStart : PhoneApplicationPage
    {
        private Games game;
        private User user;
       

        public GameStart()
        {
            InitializeComponent();            
        }

     
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
                String gameId = NavigationContext.QueryString["gameId"];
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
                game = r.Data;
        }
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            game.state = 1;
            game.update();
            fillRolesPerGameTable();
            
        }

        private void fillRolesPerGameTable()
        {
            throw new NotImplementedException();
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

                NavigationService.Navigate(new Uri("/pages/Menu.xaml", UriKind.Relative));
            }

        }
      
          
       
    }
}