using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace mmuc_zombie.app.helper
{
    public static class FacebookURIs {
        private static string m_strAppID = "172730609529224";
        private static string m_strAppSecret = "aa4e1d224d622f65311eeac3a6351e99";

        public static string m_strUrl = "https://www.facebook.com/connect/login_success.html?code=";
        private static string m_strLoginURL = "https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri=https://www.facebook.com/connect/login_success.html&display=touch&scope=publish_stream,user_hometown";
	    private static string m_strGetAccessTokenURL = "https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri=https://www.facebook.com/connect/login_success.html&client_secret={1}&code={2}";	
        private static string m_strQueryUserURL = "https://graph.facebook.com/me?fields=id,name,gender,link,hometown,picture&locale=en_US&access_token={0}";
        
        //private static string m_strAppLogoutURL = "https://www.facebook.com/logout.php?confirm=1&app_key={0}&session_key={1}&next=http://facebook.com/logout.php";
        private static string m_strAppLogoutURL = "https://www.facebook.com/logout.php?next=http://facebook.com/home.php";

        private static string m_strLoadFriendsURL = "https://graph.facebook.com/me/friends?access_token={0}";        
        private static string m_strPostMessageURL = "https://graph.facebook.com/{0}/feed";
        private static string m_strWall = "https://graph.facebook.com/{0}/feed?access_token={1}&fields=id,from,to,caption,description,attribution,message";

        public static Uri GetQueryUserUri(string strAccressToken)
        {
            return (new Uri(string.Format(m_strQueryUserURL, strAccressToken), UriKind.Absolute));
        }

        public static Uri GetLoginUri() {
		    return (new Uri(string.Format(m_strLoginURL, m_strAppID), UriKind.Absolute));
	    }

        public static Uri GetLogoutUri(string strAccessToken)
        {
            return (new Uri(string.Format(m_strAppLogoutURL, m_strAppID, SplitToken(strAccessToken)), UriKind.Absolute));
        }

        public static Uri GetTokenLoadUri(string strCode)
        {
            return (new Uri(string.Format(m_strGetAccessTokenURL, m_strAppID, m_strAppSecret, strCode), UriKind.Absolute));
        }

        public static Uri GetLoadFriendsUri(string strAccressToken)
        {
            return (new Uri(string.Format(m_strLoadFriendsURL, strAccressToken), UriKind.Absolute));
        }

        //public static Uri GetPostMessageUri()
        //{
        //    return (new Uri(m_strPostMessageURL, UriKind.Absolute));
        //}

        public static Uri GetPostMessageUri(string strUserID = "me")
        {
            return (new Uri(string.Format(m_strPostMessageURL, strUserID), UriKind.Absolute));
        }
        public static Uri GetWallUri(string strAccessToken, string strUserID = "me")
        {
            return (new Uri(string.Format(m_strWall, strUserID, strAccessToken), UriKind.Absolute));
        }

        //retrieves the session key from the accesstoken
        private static string SplitToken(string strToken)
        {
            if (!string.IsNullOrEmpty(strToken))
            {
                string[] aParts = strToken.Split('|');
                if (aParts.Length >= 3)
                {	//token format OK
                    return (aParts[1]);
                }
            }
            return ("");
        }
    }
}
