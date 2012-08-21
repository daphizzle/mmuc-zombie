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
using System.Runtime.Serialization;

namespace mmuc_zombie.app.facebook
{

    [DataContract]
    public class FBUser
    {
        public FBUser(string URI)
        {
            ID = "";
            Name = "";
            Picture = new FBPicture(URI);
        }

        private string m_strID;
        [DataMember(Name = "id")]
        public string ID
        {
            get { return m_strID ?? ""; }
            set { m_strID = value; }
        }

        private string m_strName;
        [DataMember(Name = "name")]
        public string Name
        {
            get { return m_strName ?? ""; }
            set { m_strName = value; }
        }

        private FBPicture m_strPictureLink;
        [DataMember(Name = "picture")]
        public FBPicture Picture
        {
            get { return m_strPictureLink ?? new FBPicture { PictureUrl = new FBPictureUrl() }; }
            set { m_strPictureLink = value; }
        }

        private string m_strGender;
        [DataMember(Name = "gender")]
        public string Gender
        {
            get { return m_strGender ?? ""; }
            set { m_strGender = value; }
        }

        private string m_strLink;
        [DataMember(Name = "link")]
        public string Link
        {
            get { return m_strLink ?? ""; }
            set { m_strLink = value; }
        }

        private FBHomeTown m_fbjtHomeTown;
        [DataMember(Name = "hometown")]
        public FBHomeTown HomeTown
        {
            get { return m_fbjtHomeTown ?? new FBHomeTown { Name = "" }; }
            set { m_fbjtHomeTown = value; }
        }

        public string PicLink
        {
            //use a default image if user id is unknonw - should not occure
            get { return string.Format("http://graph.facebook.com/{0}/picture", m_strID ?? "100000886762882"); }
        }

        [DataContract]
        public class FBHomeTown
        {
            private string m_strName;
            [DataMember(Name = "name")]
            public string Name
            {
                get { return m_strName ?? ""; }
                set { m_strName = value; }
            }
        }        

        [DataContract]
        public class FBPicture
        {
            public FBPicture()
            {
            }
            
            public FBPicture(String URI)
            {
                PictureUrl = new FBPictureUrl(URI);
            }            

            private FBPictureUrl m_Url;
            [DataMember(Name = "data")]
            public FBPictureUrl PictureUrl
            {
                get { return m_Url ?? new FBPictureUrl { Url = "" }; }
                set { m_Url = value; }
            }
        }

        [DataContract]
        public class FBPictureUrl
        {
            public FBPictureUrl()
            {
            }

            public FBPictureUrl(String URI)
            {
                Url = URI;
            }            

            private string m_strUrl;
            [DataMember(Name = "url")]
            public string Url
            {
                get { return m_strUrl ?? ""; }
                set { m_strUrl = value; }
            }
        }

    }
}



