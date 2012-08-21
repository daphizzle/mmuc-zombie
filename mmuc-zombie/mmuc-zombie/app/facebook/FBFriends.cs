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
    public class FBFriends
    {
        #region Friends
        private FBUser[] m_aFriends;
        [DataMember(Name = "data")]
        public FBUser[] Friends
        {
            get { return m_aFriends; }
            set { m_aFriends = value; }
        }
        #endregion
    }
}
