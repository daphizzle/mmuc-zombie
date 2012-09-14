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
using System.ComponentModel;

namespace mmuc_zombie.app.helper
{
    public class Player : User
    {
        //private bool isExpanded;
        //public event PropertyChangedEventHandler PropertyChanged;

        public Game game { get; set; }        
        public Roles role { get; set; }

        public Player(Game game, Roles role, User user)
        {            
            this.game = game;
            this.role = role;

            this.activeGame = user.activeGame;
            this.activeRole = user.activeRole;
            this.bot = user.bot;
            this.DeviceID = user.DeviceID;
            this.email = user.email;
            this.Facebook = user.Facebook;
            this.FacebookToken = user.FacebookToken;
            this.Id = user.Id;
            this.locationId = user.locationId;
            this.Password = user.Password;
            this.picture = user.picture;
            this.status = user.status;
            this.UserName = user.UserName;            
        }

        //public bool IsExpanded
        //{
        //    get
        //    {
        //        return this.isExpanded;
        //    }
        //    set
        //    {
        //        if (this.isExpanded != value)
        //        {
        //            this.isExpanded = value;
        //            this.OnPropertyChanged("IsExpanded");
        //        }
        //    }
        //}        

        //protected void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChangedEventHandler handler = this.PropertyChanged;
        //    if (handler != null)
        //    {
        //        handler(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

    }
}
