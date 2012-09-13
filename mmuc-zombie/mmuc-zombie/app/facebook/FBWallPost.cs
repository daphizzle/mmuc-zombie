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

namespace mmuc_zombie.app.facebook
{
    public class FBWallPost
    {
        public string TheCaption { get; set; }
        public string TheDescription { get; set; }
        public string TheLink { get; set; }
        public string TheMessage { get; set; }
        public string TheName { get; set; }
        public string ThePictureLink { get; set; }

        public FBWallPost(bool bFillDefaults)
        {
            if (bFillDefaults)
            {
                TheCaption = "Multiplayer Game";
                TheDescription = "Massive Urban Mobile Computing";
                TheLink = "http://www.uni-saarland.de";
                TheMessage = "Message sent from my windows phone 7";
                TheName = "Zombie Outbreak";
                ThePictureLink = "http://icons.iconarchive.com/icons/deleket/halloween-avatars/128/Zombie-icon.png";
            }
        }

        public string GetPostParameters(string strAccessToken)
        {
            try
            {
                string strRet = "access_token=" + strAccessToken;
                if (!string.IsNullOrEmpty(TheCaption))
                {
                    strRet += "&caption=" + HttpUtility.UrlEncode(TheCaption);
                }
                if (!string.IsNullOrEmpty(TheDescription))
                {
                    strRet += "&description=" + HttpUtility.UrlEncode(TheDescription);
                }
                if (!string.IsNullOrEmpty(TheLink))
                {
                    strRet += "&link=" + HttpUtility.UrlEncode(TheLink);
                }
                if (!string.IsNullOrEmpty(TheMessage))
                {
                    strRet += "&message=" + HttpUtility.UrlEncode(TheMessage);
                }
                if (!string.IsNullOrEmpty(TheName))
                {
                    strRet += "&name=" + HttpUtility.UrlEncode(TheName);
                }
                if (!string.IsNullOrEmpty(ThePictureLink))
                {
                    strRet += "&picture=" + HttpUtility.UrlEncode(ThePictureLink);
                }
                return (strRet);
            }
            catch { return (""); }
        }

    }
}