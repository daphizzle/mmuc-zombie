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
using mmuc_zombie.app.facebook;
using mmuc_zombie.app.helper;
using System.Windows.Media.Imaging;
using Parse;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Info;
using mmuc_zombie.app.model;

namespace mmuc_zombie.pages
{
    public partial class MyProfile : PhoneApplicationPage
    {
        
        User user;
        private WebClient m_wcFacebookProfile;
        private WebClient m_wcFacebookFriends;
        private static FBUser m_CurFacebookUser;

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



        private void InitializeOtherComponents()
        {
            nickname.Text = String.IsNullOrWhiteSpace(user.NickName) ? Constants.NONICKNAME : user.NickName;
        }
             
        private bool loadUsers(List<User> users,List<Friend> friends)
        {
            /* TEST DATA */

            mmuc_zombie.components.friendsView tmpUI;
            //var _parse = new Driver();
            
            foreach (User tmp in users)
            {
                //_parse.Objects.Save(tmp);
               
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
                m_wcFacebookProfile.DownloadStringAsync(FacebookURIs.GetQueryUserUri(App.AccessToken));
		        Console.Out.WriteLine("Loading user data");
	        }
	        catch(Exception eX) {
                Console.Out.WriteLine(eX.Message);
                Console.Out.WriteLine("Could not load user data");                
	        }            
        }


        void m_wcFacebookProfile_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                m_CurFacebookUser = null;
                Console.Out.WriteLine(e.Error.Message);
                Console.Out.WriteLine("Error loading user data");
                return;
            }
            try
            {                
                m_CurFacebookUser = JsonStringSerializer.Deserialize<FBUser>(e.Result);
                fbUserGrid.DataContext = m_CurFacebookUser;
                updateUser();
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
            User tmp = User.get();
            tmp.Facebook = m_CurFacebookUser == null ? tmp.Facebook : m_CurFacebookUser.Picture.PictureUrl.Url;
            tmp.NickName = nickname.Text.Equals(Constants.NONICKNAME) ? tmp.NickName : nickname.Text;
            tmp.updateCurrentUser();
            //App.User.updateCurrentUser();
            //appbar_facebook.IsEnabled = false;
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
                fbUserGrid.DataContext = new FBUser("/mmuc-zombie;component/ext/img/avatar.png");
                name.Visibility = Visibility.Collapsed;
                facebook.Visibility = Visibility.Collapsed;
                //if (appbar_facebook != null) appbar_facebook.IsEnabled = true;
            }
            else
            {                
                name.Visibility = Visibility.Visible;
                facebook.Visibility = Visibility.Visible;
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
                m_wcFacebookFriends.DownloadStringAsync(FacebookURIs.GetLoadFriendsUri(App.AccessToken));
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
            //throw new NotImplementedException();
        }

        private void loadHistory()
        {
            List<Games> games = new List<Games>(4);

            games.Add(new Games("Game 1",DateTime.Now,DateTime.Now,"1","..."));
            games.Add(new Games("Game 2", DateTime.Now, DateTime.Now, "1", "..."));
            games.Add(new Games("Game 3", DateTime.Now, DateTime.Now, "2", "..."));
            games.Add(new Games("Game 4", DateTime.Now, DateTime.Now, "3", "..."));

            mmuc_zombie.components.gamePlayed tmpUI;
            foreach (Games game in games)
            {
                tmpUI = new mmuc_zombie.components.gamePlayed();
                tmpUI.gameName.Text = game.name;
                tmpUI.gameId = game.Id;
                tmpUI.startTime.Text = game.startTime.ToString();
                tmpUI.endTime.Text = game.endTime.ToString();
                //tmpUI.owner.Text = User.find(game.ownerId, null);
                tmpUI.owner.Text = game.ownerId;
                tmpUI.Margin = new Thickness(0, 5, 0, 5);
                historyStack.Children.Add(tmpUI);
            }
        }

        private void appbar_facebook_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/FacebookLogin.xaml", UriKind.Relative));
        }

        private void nickname_GotFocus(object sender, RoutedEventArgs e)
        {
            if (nickname.Text.Trim().Equals(Constants.NONICKNAME))
                nickname.Text = "";
        }

        private void nickname_LostFocus(object sender, RoutedEventArgs e)
        {
            updateUser();
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
            parse.Objects.Query<Games>().Where(c => c.Id == c.Id).Execute(r =>
            {
                if (r.Success)
                {
                    List<Games> games = (List<Games>)r.Data.Results;
                    foreach (Games u in games)
                        parse.Objects.Delete<Games>(u);
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