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
using mmuc_zombie.components;

namespace mmuc_zombie.pages
{
    public partial class StatisticsView : PhoneApplicationPage
    {        
        List<Statistics> statistics;
        Statistics history;
        Statistics achievements;
        private List<Game> games = new List<Game>();
        private List<Roles> zombies = new List<Roles>();
        private List<Roles> survivors = new List<Roles>();
        private List<User> users = new List<User>();

        public StatisticsView()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //loadSurvivors();
            Query.getAllGames(r =>
            {
                if (r.Success)
                {
                    games = (List<Game>)r.Data.Results;
                    Query.getAllZombies(r2 =>
                    {
                        if (r2.Success)
                        {
                            zombies = (List<Roles>)r2.Data.Results;
                            Query.getAllSurvivors(r3 =>
                            {
                                if (r3.Success)
                                {
                                    survivors = (List<Roles>)r3.Data.Results;
                                    Query.getAllUser(r4 =>
                                    {
                                        if (r4.Success)
                                        {
                                            users = (List<User>)r4.Data.Results;
                                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                            {
                                                displayBestSurvivors();
                                                displayBestZombies();
                                            });
                                        }
                                    });
                                }
                            });
                        }
                    });
                }
            });
            loadTopKPlayers();
        }

        private void displayBestZombies()
        {
            hallOfFamePlayer tmpUI;
            List<hallOfFamePlayer> hofP = new List<hallOfFamePlayer>();
            for (int i = 0; i < 10 && i < survivors.Count; i++)
            {
                tmpUI = new hallOfFamePlayer();
                User zomb = new User();
                foreach (User u in users)
                {
                    if (zombies[i].userId == u.Id)
                    {
                        zomb = u;
                    }
                }
                tmpUI.Rank.Text = "" + (i + 1);
                tmpUI.Username.Text = zomb.UserName;
                tmpUI.userImage.Source = zomb.getPicture();
                tmpUI.Achivement.Text = "Survs killed: " + zombies[i].killCount;
                hofP.Add(tmpUI);
            }
            zombieListBox.ItemsSource = hofP;

        }

        private void displayBestSurvivors()
        {
            hallOfFamePlayer tmpUI;
            List<hallOfFamePlayer> hofP = new List<hallOfFamePlayer>();
            for (int i = 0; i < 10 && i < survivors.Count; i++)
            {
                tmpUI = new hallOfFamePlayer();
                User surv = new User();
                foreach (User u in users)
                {
                    if (survivors[i].userId == u.Id)
                    {
                        surv = u;
                    }
                }
                tmpUI.Rank.Text = "" + (i + 1);
                tmpUI.Username.Text = surv.UserName;
                tmpUI.userImage.Source = surv.getPicture();
                tmpUI.Achivement.Text = "Quests done: " + survivors[i].questCount;
                hofP.Add(tmpUI);
            }
            survivorListBox.ItemsSource = hofP;
        }

        private void loadTopKPlayers()
        {
            Game.findPlayedGamesByMe(loadHistory);
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

        
    }
}
