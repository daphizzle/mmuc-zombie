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
using mmuc_zombie.app.helper;

namespace mmuc_zombie.pages
{
    public partial class FacebookLogin : PhoneApplicationPage
    {
        public FacebookLogin()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            wbLogin.Navigate(FacebookURIs.GetLoginUri());
        }

        private void wbLogin_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            string strLoweredAddress = e.Uri.OriginalString.ToLower();
            if (strLoweredAddress.Contains("access_token"))
            {
                int nPos = strLoweredAddress.IndexOf("access_token");
                string strPart = strLoweredAddress.Substring(nPos + 13); // 13 acces_token=
                nPos = strPart.IndexOf("&");
                strPart = strPart.Substring(0, nPos); 
                App.AccessToken = strPart;                
                NavigationService.GoBack();                
                txtError.Text = "OK";
                return;
            }
	
        }

    }
}
