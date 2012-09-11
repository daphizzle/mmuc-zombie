﻿using System;
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
using mmuc_zombie.app.facebook;
using mmuc_zombie.app.helper;
using System.Windows.Media.Imaging;
using Parse;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Info;
using mmuc_zombie.app.model;
using Microsoft.Phone.Tasks;
using Microsoft.Phone;

namespace mmuc_zombie.pages
{
    public partial class MyProfile : PhoneApplicationPage
    {
        
        User user;        
        private WebClient m_wcFacebookProfile;
        private WebClient m_wcFacebookFriends;
        private static FBUser m_CurFacebookUser;
        private CameraCaptureTask cameraTask;        

        public MyProfile()
        {
            InitializeComponent();
            user = User.get();
            InitializeOtherComponents();
            
            //PhoneApplicationService service = PhoneApplicationService.Current;
            //user = App.User;
            //user = (User)service.State["user"];
            
            if (user != null)
            {
                loadFriends();
            }
       }



        private void InitializeOtherComponents()
        {
            nickname.Text =  ( (user==null) || String.IsNullOrWhiteSpace(user.NickName) ) ? Constants.NONICKNAME : user.NickName;
            cameraTask = new CameraCaptureTask();
            cameraTask.Completed += cameraTask_Completed;            
        }
             
        private bool loadUsers(List<User> users,List<Friend> friends)
        {
            /* SOCIAL - ADD FRIENDS */

            mmuc_zombie.components.friendsView tmpUI;            
            
            foreach (User tmp in users)
            {                
                tmpUI = new mmuc_zombie.components.friendsView();
                foreach (Friend friend in friends)
                {
                    if (tmp.Id.Equals(friend.friend))
                    {
                        tmpUI.isFriend = true;
                        tmpUI.textBlock1.Text = "friend";
                    }
                }  
                tmpUI.nameTextBlock.Text = tmp.Id;
                tmpUI.userId = user.Id;
                tmpUI.friendId = tmp.Id;
                userStackPanel.Children.Add(tmpUI);
            }

            return userStackPanel.Children.Count > 0;
        }

        private void initializeFacebookProfile()
        {
            if (m_wcFacebookProfile == null)
            {
                m_wcFacebookProfile = new WebClient();
                m_wcFacebookProfile.DownloadStringCompleted += new DownloadStringCompletedEventHandler(m_wcFacebookProfile_DownloadStringCompleted);
	        }
	        try {
                //m_wcFacebookProfile.DownloadStringAsync(FacebookURIs.GetQueryUserUri(App.AccessToken));
                m_wcFacebookProfile.DownloadStringAsync(FacebookURIs.GetQueryUserUri(user.FacebookToken));
                Console.Out.WriteLine("Facebook - Loading user data");
	        }
	        catch(Exception eX) {
                Console.Out.WriteLine(eX.Message);
                Console.Out.WriteLine("Facebook - Could not load user data");                
	        }            
        }


        void m_wcFacebookProfile_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                m_CurFacebookUser = null;
                Console.Out.WriteLine(e.Error.Message);
                Console.Out.WriteLine("Facebook - Error loading user data");
                return;
            }
            try
            {                
                m_CurFacebookUser = JsonStringSerializer.Deserialize<FBUser>(e.Result);
                fbUserGrid.DataContext = m_CurFacebookUser;
                //updateUser();
                Console.Out.WriteLine("User data loaded");
            }
            catch (Exception eX)
            {
                m_CurFacebookUser = null;
                Console.Out.WriteLine(eX.Message);
                Console.Out.WriteLine("Error parsing user data");
            }
            validateUI();
        }

        private void updateUser()
        {
            //User tmp = User.get();            
            user.Facebook = m_CurFacebookUser == null ? user.Facebook : m_CurFacebookUser.Picture.PictureUrl.Url;
            user.NickName = nickname.Text.Equals(Constants.NONICKNAME) ? user.NickName : nickname.Text;            
            if (user.updateCurrentUser())
                MessageBox.Show("Your profile has been updated successfully");
            else
                MessageBox.Show("Your profile has not been updated");            
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            initializeFacebookProfile();
            initializeFacebookFriends();

            validateUI();
            loadFriends();
            loadHistory();
        }

        private void validateUI() 
        {
            if (fbUserGrid.DataContext == null)
            {
                fbUserGrid.DataContext = new FBUser(Constants.AVATARPATH);
                name.Visibility = Visibility.Collapsed;
                facebookLabel.Visibility = Visibility.Collapsed;
                //if (appbar_facebook != null) appbar_facebook.IsEnabled = true;
            }
            else if (!((FBUser)fbUserGrid.DataContext).Picture.PictureUrl.Url.Equals(Constants.AVATARPATH))
            {                
                name.Visibility = Visibility.Visible;
                facebookLabel.Visibility = Visibility.Visible;
                //if (appbar_facebook != null) appbar_facebook.IsEnabled = false;
            }
        }

        private void initializeFacebookFriends()
        {
            if (m_wcFacebookFriends == null)
            {
                m_wcFacebookFriends = new WebClient();
                m_wcFacebookFriends.DownloadStringCompleted += new DownloadStringCompletedEventHandler(m_wcFacebookFriends_DownloadStringCompleted);
            }
            try
            {
                m_wcFacebookFriends.DownloadStringAsync(FacebookURIs.GetLoadFriendsUri(user.FacebookToken));
            }
            catch (Exception eX)
            {
                Console.WriteLine(eX.Message);
                Console.WriteLine("Error start load friends");
            }
        }

        void m_wcFacebookFriends_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Console.WriteLine(e.Error.Message);
                Console.WriteLine("Error loading friends");
                return;
            }
            try
            {                
                //fbFriends.DataContext = JsonStringSerializer.Deserialize<FBFriends>(e.Result); ;

                FBFriends friends = JsonStringSerializer.Deserialize<FBFriends>(e.Result);

                mmuc_zombie.components.facebookFriendView tmpUI;

                foreach (FBUser tmp in friends.Friends)
                {
                    tmpUI = new mmuc_zombie.components.facebookFriendView();
                    tmpUI.name.Text = tmp.Name;
                    tmpUI.image.Source = new BitmapImage(new Uri(tmp.PicLink, UriKind.Absolute));
                    tmpUI.Margin = new Thickness(0, 5, 0, 5);
                    friendStack.Children.Add(tmpUI);
                }

            }
            catch (Exception eX)
            {
                Console.WriteLine(eX.Message);
                Console.WriteLine("Error parsing friends");
            }
        }

        private void loadFriends()
        {
            loadLocalFriends();            
        }
        
        private void loadLocalFriends()
        {
            if (user != null)
            {
                string userId = user.Id;
                var parse = new Driver();
                parse.Objects.Query<User>().Where(c => c.Id == c.Id).Execute(r =>
                {
                    if (r.Success)
                    {
                        List<User> users = (List<User>)r.Data.Results;
                        parse.Objects.Query<Friend>().Where(c => c.user == userId).Execute(r2 =>
                        {
                            if (r2.Success)
                            {
                                List<Friend> friends = (List<Friend>)r2.Data.Results;
                                Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    if (!loadUsers(users, friends))
                                    {
                                        userListBox.Visibility = System.Windows.Visibility.Collapsed;
                                    }
                                    else
                                    {
                                        userListBox.Visibility = System.Windows.Visibility.Visible;
                                    }
                                });
                            }
                        });
                    }
                });
            }
        }

        private void loadHistory()
        {
            List<Game> games = new List<Game>(4);

            games.Add(new Game("Game 1",DateTime.Now,DateTime.Now,"1","..."));
            games.Add(new Game("Game 2", DateTime.Now, DateTime.Now, "1", "..."));
            games.Add(new Game("Game 3", DateTime.Now, DateTime.Now, "2", "..."));
            games.Add(new Game("Game 4", DateTime.Now, DateTime.Now, "3", "..."));

            mmuc_zombie.components.gamePlayed tmpUI;
            foreach (Game game in games)
            {
                tmpUI = new mmuc_zombie.components.gamePlayed();
                tmpUI.gameName.Text = game.name;
                tmpUI.gameId = game.Id;
                //tmpUI.startTime.Text = game.startTime.ToString();
                //tmpUI.endTime.Text = game.endTime.ToString();
                //tmpUI.owner.Text = User.find(game.ownerId, null);
                tmpUI.owner.Text = game.ownerId;
                tmpUI.Margin = new Thickness(0, 5, 0, 5);
                historyStack.Children.Add(tmpUI);
            }
        }

        private void appbar_save_Click(object sender, EventArgs e)
        {
            updateUser();
        }

        private void appbar_facebook_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/FacebookLogin.xaml", UriKind.Relative));
        }

        private void appbar_facebook_logout_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/FacebookLogout.xaml", UriKind.Relative));
        }       

        private void nickname_GotFocus(object sender, RoutedEventArgs e)
        {
            if (nickname.Text.Trim().Equals(Constants.NONICKNAME))
                nickname.Text = "";
        }

        //private void nickname_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    updateUser();
        //}

        private void avatar_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            takePicture();
        }

        private void takePicture()
        {            
            cameraTask.Show();
        }

        void cameraTask_Completed(object sender, PhotoResult pr)            
        {
            if (pr.TaskResult == TaskResult.OK)
            {
                byte[] imgLocal;
                //avatar.DataContext = e.ChosenPhoto;
                //MessageBox.Show(e.ChosenPhoto);
                //MediaLibrary medialibrary = new MediaLibrary();
                //medialibrary.SavePicture("givenameofimage", e.ChosenPhoto);                
                if (pr.ChosenPhoto != null)
                {
                    imgLocal = new byte[(int)pr.ChosenPhoto.Length];
                    user._avatar = imgLocal;
                    pr.ChosenPhoto.Read(imgLocal, 0, imgLocal.Length);
                    pr.ChosenPhoto.Seek(0, System.IO.SeekOrigin.Begin);
                    var bitmapImage = PictureDecoder.DecodeJpeg(pr.ChosenPhoto);
                    this.avatar.Source = bitmapImage;
                }
               
            }
        }
 
        //private void btnShowCamera_Click(object sender, RoutedEventArgs e)
        //{
        //    cameraTask.Show();         
        //}

        private void avatar_MouseEnter(object sender, MouseEventArgs e)
        {
            takePicture();
        }



        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string userId = user.Id;
            string locId = user.locationId;
            var parse = new Driver();
            //delete all User except me
            parse.Objects.Query<User>().Where(c => c.Id != userId).Execute(r =>
                {
                    if (r.Success)
                    {
                        List<User> users = (List<User>)r.Data.Results;
                        foreach(User u in users)
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