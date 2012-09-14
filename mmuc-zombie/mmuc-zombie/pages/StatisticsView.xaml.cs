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
using mmuc_zombie.app.helper;
using Parse;
using System.Diagnostics;

namespace mmuc_zombie.pages
{
    public partial class StatisticsView : PhoneApplicationPage
    {        
        List<Statistics> statistics;
        Statistics history;
        Statistics achievements;

        public StatisticsView()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            loadTopKPlayers();
        }

        private void loadTopKPlayers()
        {
            statistics = new List<Statistics>(2);
            statistics.Add(new Statistics("Survivors", DateTime.Now));
            statistics.Add(new Statistics("Zombies", DateTime.Now));
            
            Game.findPlayedGames(loadPlayers);            
            Game.findPlayedGamesByMe(loadHistory);
        }

        
        public void loadPlayers(Response<ResultsResponse<Game>> r)
        {
            string gameID;            
            Roles role_tmp;
            User user_tmp;            

            var parse = new Driver();
            if (r.Success)
            {
                //retrieving all played games
                List<Game> playedGames = (List<Game>)r.Data.Results;
                //Debug.WriteLine("StatisticsView - Played Games: " + playedGames.Count);

                foreach (Game game in playedGames)
                {                        
                    gameID = game.Id;                    

                    //retrieving all data per played game
                    parse.Objects.Query<Roles>().Where(c => c.gameId == gameID).SortDescending(c => c.questCount).Execute(r2 =>
                    {
                        if (r2.Success)
                        {
                            List<Roles> rtmp = (List<Roles>)r2.Data.Results;
                            //Debug.WriteLine("StatisticsView - Roles("+gameID+"): " + rtmp.Count);

                            if (rtmp.Count > 0)
                            {
                                role_tmp = rtmp[0];

                                //retrieving the first ranked player
                                parse.Objects.Get<User>(role_tmp.userId, r3 =>
                                {
                                    if (r3.Success)
                                    {                                        
                                        user_tmp = r3.Data;
                                        
                                        if(role_tmp.roleType == Constants.ROLE_SURVIVOR)
                                            statistics[0].Players.Add(new Player(game, role_tmp, user_tmp));
                                        else if (role_tmp.roleType == Constants.ROLE_SURVIVOR)
                                            statistics[1].Players.Add(new Player(game, role_tmp, user_tmp));

                                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                                        {
                                            this.listBox.ItemsSource = statistics;
                                        });

                                    }
                                });
                            }
                        }
                    });
                }
                
            }

            //Debug.WriteLine("StatisticsView - Players: " + cont);
        }

        private void loadHistory(Response<Game> r)
        {
            string gameID;
            User user = User.getFromState();
            string userID = user.Id;
            history = new Statistics();
            achievements = new Statistics();

            if(r.Success)
            {
                Game g = r.Data;
                var parse = new Driver();

                gameID = g.Id;
                parse.Objects.Query<Roles>().Where(c => c.gameId == gameID && c.userId == userID).Execute(r2 =>
                {
                    if(r2.Data.Results.Count > 0)
                    {
                        Roles role = r2.Data.Results[0];
                        history.Players.Add(new Player(g,role,user));

                        if (role.roleType == Constants.ROLE_SURVIVOR)
                            achievements.gamesSurvivor++;
                        else if (role.roleType == Constants.ROLE_ZOMBIE)
                            achievements.gamesZombie++;

                        if (role.alive) achievements.winGames++;
                        else achievements.lostGames++;

                        
                    }
                });

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (!showHistory())
                    {
                        noResults.Visibility = System.Windows.Visibility.Visible;
                        historyList.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        noResults.Visibility = System.Windows.Visibility.Collapsed;
                        historyList.Visibility = System.Windows.Visibility.Visible;
                    }
                });
                
            }         
        }

        private bool showHistory()
        {            
            mmuc_zombie.components.gamePlayedStatistics tmpUI;
            showAchievements();

            foreach (Player player in history.Players)
            {
                tmpUI = new mmuc_zombie.components.gamePlayedStatistics();
                tmpUI.gameName.Text = player.game.name;
                tmpUI.gameId = player.game.Id;                
                tmpUI.owner.Text = player.game.ownerId;

                tmpUI.role.Text = player.role.roleType;
                tmpUI.countLabel.Text = player.role.roleType == Constants.ROLE_ZOMBIE ? Constants.ROLE_ZOMBIE : Constants.ROLE_SURVIVOR;
                tmpUI.count.Text = player.role.roleType == Constants.ROLE_ZOMBIE ? player.role.killCount.ToString() : player.role.questCount.ToString();

                tmpUI.Margin = new Thickness(0, 5, 0, 5);
                historyStack.Children.Add(tmpUI);
            }            
            return historyStack.Children.Count > 0;
        }

        private void showAchievements()
        {
            achUserName.Text = User.getFromState().UserName;
            playedGames.Text = achievements.playedGames.ToString();
            wonGames.Text = achievements.winGames.ToString();
            lostGames.Text = achievements.lostGames.ToString();
            nSurvivor.Text = achievements.gamesSurvivor.ToString();
            nZombie.Text = achievements.gamesZombie.ToString();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var user = User.getFromState();
            string userId = user.Id;
            string locId = user.locationId;
            var parse = new Driver();
            //delete all User except me
            parse.Objects.Query<User>().Where(c => c.Id != userId).Execute(r =>
            {
                if (r.Success)
                {
                    List<User> users = (List<User>)r.Data.Results;
                    foreach (User u in users)
                        parse.Objects.Delete<User>(u);
                }
            });

            //delete all Games
            parse.Objects.Query<Game>().Where(c => c.Id == c.Id).Execute(r =>
            {
                if (r.Success)
                {
                    List<Game> games = (List<Game>)r.Data.Results;
                    foreach (Game u in games)
                        parse.Objects.Delete<Game>(u);
                }
            });

            //delete all Locations except my Location
            parse.Objects.Query<MyLocation>().Where(c => c.Id != locId).Execute(r =>
            {
                if (r.Success)
                {
                    List<MyLocation> users = (List<MyLocation>)r.Data.Results;
                    foreach (MyLocation u in users)
                        parse.Objects.Delete<MyLocation>(u);
                }
            });


            //delete all Roles
            parse.Objects.Query<Roles>().Where(c => c.Id == c.Id).Execute(r =>
            {
                if (r.Success)
                {
                    List<Roles> users = (List<Roles>)r.Data.Results;
                    foreach (Roles u in users)
                        parse.Objects.Delete<Roles>(u);
                }
            });


            //delete all Invites
            parse.Objects.Query<Invite>().Where(c => c.Id == c.Id).Execute(r =>
            {
                if (r.Success)
                {
                    List<Invite> users = (List<Invite>)r.Data.Results;
                    foreach (Invite u in users)
                        parse.Objects.Delete<Invite>(u);
                }
            });


            //delete all Message
            parse.Objects.Query<Message>().Where(c => c.Id == c.Id).Execute(r =>
            {
                if (r.Success)
                {
                    List<Message> users = (List<Message>)r.Data.Results;
                    foreach (Message u in users)
                        parse.Objects.Delete<Message>(u);
                }
            });

            //delete all Friends
            parse.Objects.Query<Friend>().Where(c => c.Id == c.Id).Execute(r =>
            {
                if (r.Success)
                {
                    List<Friend> users = (List<Friend>)r.Data.Results;
                    foreach (Friend u in users)
                        parse.Objects.Delete<Friend>(u);
                }
            });

        }
    }
}
