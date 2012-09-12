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
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using mmuc_zombie.components;

namespace mmuc_zombie.pages
{
    public partial class HallFame : PhoneApplicationPage
    {
        private List<Game> games = new List<Game>();
        private List<Roles> zombies = new List<Roles>();
        private List<Roles> survivors = new List<Roles>();
        private List<User> users = new List<User>();
        bool step1 = false, step2 = false, step3 = false, step4 = false;

        public HallFame()
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
        }

        private void displayBestZombies()
        {
            hallOfFamePlayer tmpUI;
            List<hallOfFamePlayer> hofP = new List<hallOfFamePlayer>();
            CollectionViewSource orderedZombies = new CollectionViewSource();
            orderedZombies.Source = zombies;
            orderedZombies.SortDescriptions.Clear();
            orderedZombies.SortDescriptions.Add(new SortDescription("killCount",ListSortDirection.Descending));
            zombies = (List<Roles>)orderedZombies.View.SourceCollection;
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
                tmpUI.userImage.Source = new BitmapImage(new Uri(String.IsNullOrWhiteSpace(zomb.Facebook) ? zomb.getPicture() : zomb.Facebook, UriKind.Absolute));
                tmpUI.Achivement.Text = "Survivors killed: " + zombies[i].killCount;
                hofP.Add(tmpUI);
            }
            zombieListBox.ItemsSource = hofP;
            
        }

        private void displayBestSurvivors()
        {
            hallOfFamePlayer tmpUI;
            List<hallOfFamePlayer> hofP = new List<hallOfFamePlayer>();
            CollectionViewSource orderedSurvivors = new CollectionViewSource();
            orderedSurvivors.Source = survivors;
            orderedSurvivors.SortDescriptions.Clear();
            orderedSurvivors.SortDescriptions.Add(new SortDescription("questCount", ListSortDirection.Descending));
            survivors = (List<Roles>)orderedSurvivors.View.SourceCollection;
            for (int i = 0; i < 10 && i<survivors.Count; i++)
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
                tmpUI.Rank.Text = "" + (i+1);
                tmpUI.Username.Text = surv.UserName;
                tmpUI.userImage.Source = new BitmapImage(new Uri(String.IsNullOrWhiteSpace(surv.Facebook) ? surv.getPicture() : surv.Facebook, UriKind.Absolute));
                tmpUI.Achivement.Text = "Quests done: " + survivors[i].questCount;
                hofP.Add(tmpUI);
            }
            survivorListBox.ItemsSource = hofP;
        }



    }
}
