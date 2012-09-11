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
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace mmuc_zombie.pages
{
    public partial class IngameView : PhoneApplicationPage
    {
        List<User> userList;
        List<MyLocation> locationList  = new List<MyLocation>(); 
        List<Roles>roleList = new List<Roles>();
        User user;
        Game game; 
        Roles role;
        MyLocation myLocation;
        Boolean painting=false;
        private int i;
   
        public IngameView()
        {
            InitializeComponent();
            PhoneApplicationService service = PhoneApplicationService.Current;
            user = (User)service.State["user"];
            drawGameBorder();
        }

        //constructor used for testing only
        public IngameView(List<User> uList, List<MyLocation> locList, List<Roles> rList, User user, Game game, Roles role, MyLocation myLoc)
        {
            userList = uList;
            locationList = locList;
            roleList = rList;
            this.user = user;
            this.game = game;
            this.role = role;
            myLocation = myLoc;
        }

        public void drawGameBorder()
        {
            Query.getGame(user.activeGame,getGameCallback);
            Query.getGameArea(user.activeGame, getGameAreaCallback); 
                
        }


        public void loadData()
        {
             if (!painting){
                 painting = true;
                 //Debug.WriteLine("loadData");
                 loadGame();
             }
        }

        private void loadGame()
        {
            Query.getGame(user.activeGame, r =>
                {
                    if (r.Success)
                    {
                        game = r.Data;
                        if (game.state == 3)
                        {
                            //our game is now finished
                            gameFinished();
                        }
                        else
                        {
                            loadUsers();
                        }
                    }
                });
        }

        private void gameFinished()
        {
            //game has finished, set data for user
            user.activeGame = "";
            user.activeRole = "";
            user.status = 0;
            user.update(r =>
                {
                    //display a message, that the game has now ended
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            MessageBoxResult mb = MessageBox.Show("Congratulations, there are no Survivors left. Zombies win!", "Alert", MessageBoxButton.OK);
                            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/pages/Menu.xaml", UriKind.Relative));
                        });
                });
        }

        private void loadUsers()
        {
             Query.getUsersByGame(user.activeGame, r =>
            {
                if (r.Success)
                {
                     Deployment.Current.Dispatcher.BeginInvoke(() =>
                     {
                        userList = (List<User>)r.Data.Results;
                        //Debug.WriteLine("got Users");
                        locationList=new List<MyLocation>();
                        loadLocations();    
                     });
                }
            });

        }
        private void loadLocations()
        {   
            if (i<userList.Count)
                Query.getLocation(userList[i].locationId, r =>
                {
                    if (r.Success)
                    {
                        //Debug.WriteLine("got Location "+ r.Data.Id+" is equal userlocation "+userList[i].locationId+ "  "+r.Data.latitude+","+r.Data.longitude);
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                           locationList.Add(r.Data);
                           i++;
                           loadLocations(); 
                        });
                    }
                });
             else
             {
                i=0;
                roleList = new List<Roles>();
                loadRoles();
             }
        }
        private void loadRoles()
        {
            if (i<userList.Count)
                Query.getRole(userList[i].activeRole, r =>
                {   
                    if (r.Success)
                    {
                        //Debug.WriteLine("got Role " + r.Data.Id + " is equal userRole " + userList[i].activeRole);
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            roleList.Add(r.Data);
                            i++;
                            loadRoles();
                        });
                    }
                });
            else
            {
                i=0;
                doIngameStuff();
            }
        }

        private void updateRoles()
        {
            if (i < roleList.Count)
            {
                roleList[i].update(r =>
                    {
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    Debug.WriteLine("Updated role " + roleList[i].Id);
                                    i++;
                                    updateRoles();
                                });
                      
                    });
            }
            else
            {
                i = 0;
                updateUser();
            }
        }

        private void updateUser()
        {
            if (i < userList.Count)
            {
                userList[i].update(r =>
                {
             
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            Debug.WriteLine("Updated user " + userList[i].Id);
                            i++;
                            updateUser();
                        });
                    
                });
            }
            else
            {
                i = 0;
                drawPins();
            }
        }

        private void doIngameStuff()
        {
            if (user==null){
                PhoneApplicationService service = PhoneApplicationService.Current;
                user = (User)service.State["user"];
             }
          
            if (user.Id == game.hostId)
            {
                if (noSurvivorsLeft())
                {
                    game.state = 3;
                    game.update(r =>
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    Debug.WriteLine("updated game " + game.Id);
                                    drawPins();
                                });
                        });

                }
                else
                {
                    Debug.WriteLine("Starting infection");
                    infectSurvivors();
                }
            }
            else
                drawPins();
        }

        
        private void infectSurvivors()
        {
            bool hostDied = false;
            for (int i = 0; i < locationList.Count; i++)
            {
                if (roleList[i].roleType.Equals("Survivor"))
                {
                    //check how many zombies are near this survivor
                    int infectionPlus = amountOfZombiesNearSurvivor(i);
                    if (infectionPlus > 0)
                    {
                        //there are zombies near survivor
                        Debug.WriteLine(String.Format("There are {0} zombies near user {1}", infectionPlus,userList[i].UserName));
                        if (roleList[i].infectionCount + infectionPlus > 5)
                        {

                            Debug.WriteLine(String.Format("User {0} is about to die", userList[i].UserName));
                            //survivor got infected and died
                            roleList[i].alive = false;
                            userList[i].activeGame = "";
                            userList[i].status = 0;
                            userList[i].activeRole = "";
                            if (userList[i].Id == game.hostId)
                            {
                                //current host died, we need a new one
                                hostDied = true;
                                foreach (User u in userList)
                                {
                                    if (u.bot || u.Id == game.hostId)
                                    {
                                        continue;
                                    }
                                    game.hostId = u.Id;
                                }
                            }
                        }
                        else
                        {

                            //survivor is getting infected but still alive
                            roleList[i].infectionCount += infectionPlus;
                            Debug.WriteLine(String.Format("User {0} is currently getting infected, progress = {1}", userList[i].UserName, roleList[i].infectionCount));

                        }
                    }
                    else
                    {
                        //there are no zombies near the survivor
                        Debug.WriteLine(String.Format("There are {0} zombies near user {1}", infectionPlus, userList[i].UserName));
                        if (roleList[i].infectionCount > 0)
                        {
                            //this means that the user managed to escape the zombies while they were infecting him
                            Debug.WriteLine(String.Format("User {0} got savely away",  userList[i].UserName));
                            roleList[i].infectionCount = 0;
                        }
                    }
                }
            }
            this.i = 0;
            if (hostDied)
            {
                updateGame();
            }
            else
            {
                updateRoles();
            }
        }

        private bool noSurvivorsLeft()
        {
            bool noSurvivors = true;
            foreach(Roles r in roleList)
            {
                if (r.roleType == "Survivor")
                {
                    noSurvivors = false;
                }
            }
            return noSurvivors;
        }

        private void updateGame()
        {
            game.update(r =>
                {
                    Debug.WriteLine("Updated game: " + game.Id);
                    updateRoles();
                });
        }

        private int amountOfZombiesNearSurvivor(int i)
        {
            int zombies = 0;
            for (int j = 0; j < userList.Count; j++)
            {
                double distance = locationList[j].toGeoCoordinate().GetDistanceTo(locationList[i].toGeoCoordinate());
                bool near = (distance)<2500;
                if(roleList[j].roleType.Equals("Zombie")&&near)
                {
                    ++zombies;
                }
            }
            return zombies;    
        } 
        
        private void drawPins()
        {
            

            //Debug.WriteLine("Start Drawing Users");
            //debug.Text = "";
            mapLayer.Children.Clear();
            int playerPosition = 0;
            for (int i = 0; i < userList.Count; i++)
            {
                var p = new Pushpin();
                //Debug.WriteLine("-----------------------------------");
                //Debug.WriteLine("User" + userList[i].Id);
                p.Location = new GeoCoordinate(locationList[i].latitude, locationList[i].longitude);
                p.Name = userList[i].Id;
                //debug.Text += "User: " + userList[i].Id + "\n Location (" + locationList[i].latitude + "," + locationList[i].longitude + ")\n";
                if (locationList[i].Id.Equals(user.locationId))
                        myLocation=locationList[i];
                //Debug.WriteLine("location "+locationList[i].Id); 
                if (roleList[i].Id.Equals(user.activeRole))
                        role=roleList[i];
           
                if (roleList[i].roleType.Equals("Zombie"))
                {
                  //  p.Style = (Style)(Application.Current.Resources["PushpinStyle2"]);
                 //   Debug.WriteLine("Zombiestyle");
                    if (user.Id.Equals(userList[i].Id))
                    {
                        playerPosition = i;
                        p.Template = this.Resources["playerzombiepin"] as ControlTemplate;
                    }
                    else
                        p.Template = this.Resources["zombiepin"] as ControlTemplate;
                   
                 
                }
                else
                {
                 //   p.Style = (Style)(Application.Current.Resources["PushpinStyle"]);
                    Debug.WriteLine("Survivorstyle");
                    if (user.Id.Equals(userList[i].Id))
                    {
                        playerPosition = i;
                        p.Template = this.Resources["playersurvivorpin"] as ControlTemplate;
                    }
                    else
                        p.Template = this.Resources["survivorpin"] as ControlTemplate;
                    
                  
                }
            
                mapLayer.Children.Add(p);
            }
            //if (role.roleType == "Survivor")
            //{
            //    Bar1.Visibility = System.Windows.Visibility.Visible;
            //    Bar2.Visibility = System.Windows.Visibility.Visible;
            //    Bar3.Visibility = System.Windows.Visibility.Visible;
            //    Bar4.Visibility = System.Windows.Visibility.Visible;
            //    Bar5.Visibility = System.Windows.Visibility.Visible;
            //    switch (role.infectionCount)
            //    {
            //        case 1:
            //            Bar1.Fill = new SolidColorBrush(Colors.Red);    
            //            Bar2.Fill = new SolidColorBrush(Colors.Green);
            //            Bar3.Fill = new SolidColorBrush(Colors.Green);
            //            Bar4.Fill = new SolidColorBrush(Colors.Green);
            //            Bar5.Fill = new SolidColorBrush(Colors.Green);
            //            break;
            //        case 2:
            //            Bar1.Fill = new SolidColorBrush(Colors.Red);
            //            Bar2.Fill = new SolidColorBrush(Colors.Red);
            //            Bar3.Fill = new SolidColorBrush(Colors.Green);
            //            Bar4.Fill = new SolidColorBrush(Colors.Green);
            //            Bar5.Fill = new SolidColorBrush(Colors.Green);
            //            break;
            //        case 3:
            //            Bar1.Fill = new SolidColorBrush(Colors.Red);
            //            Bar2.Fill = new SolidColorBrush(Colors.Red);
            //            Bar3.Fill = new SolidColorBrush(Colors.Red);
            //            Bar4.Fill = new SolidColorBrush(Colors.Green);
            //            Bar5.Fill = new SolidColorBrush(Colors.Green);
            //            break;
            //        case 4:
            //            Bar1.Fill = new SolidColorBrush(Colors.Red);
            //            Bar2.Fill = new SolidColorBrush(Colors.Red);
            //            Bar3.Fill = new SolidColorBrush(Colors.Red);
            //            Bar4.Fill = new SolidColorBrush(Colors.Red);
            //            Bar5.Fill = new SolidColorBrush(Colors.Green);
            //            break;
            //        case 5:
            //            Bar1.Fill = new SolidColorBrush(Colors.Red);
            //            Bar2.Fill = new SolidColorBrush(Colors.Red);
            //            Bar3.Fill = new SolidColorBrush(Colors.Red);
            //            Bar4.Fill = new SolidColorBrush(Colors.Red);
            //            Bar5.Fill = new SolidColorBrush(Colors.Red);
            //            break;
            //        default:
            //            Bar1.Fill = new SolidColorBrush(Colors.Green);
            //            Bar2.Fill = new SolidColorBrush(Colors.Green);
            //            Bar3.Fill = new SolidColorBrush(Colors.Green);
            //            Bar4.Fill = new SolidColorBrush(Colors.Green);
            //            Bar5.Fill = new SolidColorBrush(Colors.Green);
            //            break;
            //    }
            //}
            map.Center=locationList[playerPosition].toGeoCoordinate();
            map.ZoomLevel = 14;
            painting = false;
        }
        


        public void getGameCallback(Response<Game> r)
        {
            if (r.Success)
            {
                game = r.Data;
            }
        }



        public void getGameAreaCallback(Response<ResultsResponse<MyLocation>> r)
        {
            if (r.Success)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        var list = (List<MyLocation>)r.Data.Results;
                        gameAreaLayer.Children.Add(StaticHelper.inGameArea(list));

                    });
            }

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            PositionRetriever.startPositionRetrieving(1);
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult mb = MessageBox.Show("Do you want to leave the game?", "Alert", MessageBoxButton.OKCancel);
            if (mb != MessageBoxResult.OK)
            {
                e.Cancel = true;
            }
            else
            {
                user.activeGame = "";
                user.status = 0;
                user.update();
                (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/pages/Menu.xaml", UriKind.Relative));
            }
        }



        
        //private void map_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    e.Handled = true;
        
        //}



        //private void map_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_MapZoom(object sender, MapZoomEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_KeyUp(object sender, KeyEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_KeyDown(object sender, KeyEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_MapPan(object sender, MapDragEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        //{
        //    e.Handled = true;
        //}


        //private void map_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_ViewChangeEnd(object sender, MapEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_ViewChangeStart(object sender, MapEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    e.Handled = true;
        //}

        
        //private void map_ViewChangeOnFrame(object sender, MapEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void map_TargetViewChanged(object sender, MapEventArgs e)
        //{
        //    e.Handled = true;
        //}



        //private void map_MouseEnter(object sender, MouseEventArgs e)
        //{
            
        //}

        //private void map_MouseLeave(object sender, MouseEventArgs e)
        //{

        //}

        //private void map_MouseMove(object sender, MouseEventArgs e)
        //{
            
        //}

    }
}