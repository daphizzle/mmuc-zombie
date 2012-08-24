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
using System.Diagnostics;

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
            string strStart = FacebookURIs.m_strUrl;
            string strLoweredAddress = e.Uri.OriginalString.ToLower();
            if (strLoweredAddress.StartsWith(strStart))
            {             
                String strTmp = e.Uri.OriginalString.Substring(strStart.Count());
                Uri tmp = FacebookURIs.GetTokenLoadUri(strTmp);
                wbLogin.Navigate(tmp);
                return;
            }

            string key = "access_token=";
            string strTest = wbLogin.SaveToString();
            if (strTest.Contains(key))
            {
                int nPos = strTest.IndexOf(key);
                string strPart = strTest.Substring(nPos + key.Count());
                nPos = strPart.IndexOf("&");
                strPart = strPart.Substring(0, nPos);
                App.AccessToken = strPart;                
                NavigationService.GoBack();
                return;
            }
	
        }

    }
}
