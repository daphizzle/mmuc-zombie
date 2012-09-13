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
using Microsoft.Phone.Tasks;
using Microsoft.Phone;
using System.Diagnostics;

namespace mmuc_zombie.pages
{
    public partial class MyProfile : PhoneApplicationPage
    {
        
        User user;        
        private WebClient m_wcFacebookProfile;
        private WebClient m_wcFacebookFriends;
        private static FBUser m_CurFacebookUser;
        private CameraCaptureTask cameraTask;
        //private PhoneApplicationService service = PhoneApplicationService.Current;

        public MyProfile()
        {
            InitializeComponent();
            user = User.get();
            InitializeOtherComponents();
            
            //PhoneApplicationService service = PhoneApplicationService.Current;
            //user = App.User;
            //user = (User)service.State["user"];
        }



        private void InitializeOtherComponents()
        {
            nickname.Text =  ( (user==null) || String.IsNullOrWhiteSpace(user.UserName) ) ? Constants.NONICKNAME : user.UserName;
            cameraTask = new CameraCaptureTask();
            cameraTask.Completed += cameraTask_Completed;            
        }

        private bool loadUsers(List<User> users, List<Friend> friends)
        {
            /* SOCIAL - ADD FRIENDS */

            mmuc_zombie.components.friendsView tmpUI;
            userStackPanel.Children.Clear();

            foreach (User tmp in users)
            {
                if (!tmp.Id.Equals(User.getFromState().Id))
                {
                    tmpUI = new mmuc_zombie.components.friendsView();
                    foreach (Friend friend in friends)
                    {
                        if (tmp.Id.Equals(friend.friend))
                        {
                            tmpUI.isFriend = true;
                            tmpUI.addButton.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("/mmuc-zombie;component/ext/img/del.jpg", UriKind.Relative)) };
                            tmpUI.tmpTextBlock.Text = "friend";
                            tmpUI.friendId = friend.Id;
                        }
                    }
                    tmpUI.nameTextBlock.Text = tmp.UserName;
                    tmpUI.userImage.Source = new BitmapImage(new Uri(String.IsNullOrWhiteSpace(tmp.Facebook) ? tmp.getPicture() : tmp.Facebook, UriKind.Absolute));
                    tmpUI.userId = user.Id;
                    tmpUI.newfriend = tmp.Id;

                    userStackPanel.Children.Add(tmpUI);
                }
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
                Debug.WriteLine(eX.Message);
                Debug.WriteLine("Error parsing user data");
            }
            validateUI();
        }

        private void updateUser(Boolean feedback)
        {
            //User tmp = User.get();        
            user.Facebook = m_CurFacebookUser == null ? user.Facebook : m_CurFacebookUser.Picture.PictureUrl.Url;
            user.UserName = nickname.Text.Equals(Constants.NONICKNAME) ? user.UserName : nickname.Text;
            user.updateCurrentUser();
                if(feedback)
                MessageBox.Show("Your profile has been updated successfully");           
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            initializeFacebookProfile();
            initializeFacebookFriends();

            validateUI();
            loadUsers();
            //loadHistory();
        }

        private void validateUI() 
        {
            if (fbUserGrid.DataContext == null)
            {
                fbUserGrid.DataContext = new FBUser(Constants.AVATARPATH);
                nameText.Visibility = Visibility.Collapsed;
                genderText.Visibility = Visibility.Collapsed;
                hometownText.Visibility = Visibility.Collapsed;
                facebookLabel.Visibility = Visibility.Collapsed;
                offline.Visibility = Visibility.Visible;
                //if (appbar_facebook != null) appbar_facebook.IsEnabled = true;
            }
            else if (!((FBUser)fbUserGrid.DataContext).Picture.PictureUrl.Url.Equals(Constants.AVATARPATH))
            {                
                nameText.Visibility = Visibility.Visible;
                genderText.Visibility = Visibility.Visible;
                hometownText.Visibility = Visibility.Visible;
                facebookLabel.Visibility = Visibility.Visible;
                offline.Visibility = Visibility.Collapsed;
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
                Debug.WriteLine(eX.Message);
                Debug.WriteLine("Error start load fb friends");
            }
        }

        void m_wcFacebookFriends_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Debug.WriteLine(e.Error.Message);
                Debug.WriteLine("Error loading fb friends");
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
                    tmpUI.fbUserId = tmp.ID;
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

        private void loadUsers()
        {
            loadLocalUsers();            
        }
        
        private void loadLocalUsers()
        {
            if (user != null)
            {
                string userId = user.Id;
                var parse = new Driver();
                parse.Objects.Query<User>().Where(c => c.Id == c.Id &&c.bot==false).Execute(r =>
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

        

        private void appbar_save_Click(object sender, EventArgs e)
        {
            updateUser(true);
        }

        private void appbar_facebook_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/FacebookLogin.xaml", UriKind.Relative));
            updateUser(false);
        }

        private void appbar_facebook_logout_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/FacebookLogout.xaml", UriKind.Relative));
            updateUser(false);
        }       

        private void nickname_GotFocus(object sender, RoutedEventArgs e)
        {
            if (nickname.Text.Trim().Equals(Constants.NONICKNAME))
                nickname.Text = "";
            else
                nickname.SelectAll();
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
                Picture pic = new Picture(user);
                if (pr.ChosenPhoto != null)
                {
                    imgLocal = new byte[(int)pr.ChosenPhoto.Length];
                    pic.avatarBytes = imgLocal;
                    pr.ChosenPhoto.Read(imgLocal, 0, imgLocal.Length);
                    pr.ChosenPhoto.Seek(0, System.IO.SeekOrigin.Begin);
                    var bitmapImage = PictureDecoder.DecodeJpeg(pr.ChosenPhoto);
                    this.avatar.Source = bitmapImage;
                    pic.savePicture();
                }
               
            }
        }
         
        private void avatar_MouseEnter(object sender, MouseEventArgs e)
        {
            takePicture();
        }



        
    }
}