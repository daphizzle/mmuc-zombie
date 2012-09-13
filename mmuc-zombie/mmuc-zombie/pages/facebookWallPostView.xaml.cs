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
using System.Diagnostics;

namespace mmuc_zombie.components
{
    public partial class facebookWallPostView : PhoneApplicationPage
    {        
        private FBWallPost m_fbPost;
        private Uri m_uriFeedLoad;
        private Uri m_uriPost;
        private string fbUserID;

        public facebookWallPostView()
        {
            InitializeComponent();
            init();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string strID;                        
            if (!NavigationContext.QueryString.TryGetValue("id", out strID))
            {
                NavigationService.GoBack();
            }
            fbUserID = strID;
            m_uriFeedLoad = FacebookURIs.GetWallUri(App.AccessToken, fbUserID);
            m_uriPost = FacebookURIs.GetPostMessageUri(fbUserID);	//post to this users wall  
        }

        private void init()
        {
            commentText.Text = Constants.FBCOMMENT;                      
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            m_fbPost = new FBWallPost(true);            
        }

        private void commentText_GotFocus(object sender, RoutedEventArgs e)
        {
            if (commentText.Text.Equals(Constants.FBCOMMENT))
                commentText.Text = "";
            else
                commentText.SelectAll();
        }
        
        private void postButton_Click(object sender, RoutedEventArgs e) {
            FBWallPost fbwPost = new FBWallPost(true);
            WebClient m_wcPostMessage = new WebClient();
            
            m_wcPostMessage = new WebClient();
            m_wcPostMessage.UploadStringCompleted += new UploadStringCompletedEventHandler(m_wcPostMessage_UploadStringCompleted);
            
            fbwPost.TheMessage = this.commentText.Text + "\n" + fbwPost.TheMessage;
            string strParams = fbwPost.GetPostParameters(App.AccessToken);
            m_wcPostMessage.UploadStringAsync(m_uriPost, "POST", strParams);	        
        }
        void m_wcPostMessage_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {                
                Debug.WriteLine("Error posting message. " + e.Error.Message);
                MessageBox.Show("Error posting your message");
                return;
            }
            else
            {
                Debug.WriteLine("Post done. " + e.Result);
                MessageBox.Show("Your post have been sent succesfully");                
            }            
        }
        

    }
}