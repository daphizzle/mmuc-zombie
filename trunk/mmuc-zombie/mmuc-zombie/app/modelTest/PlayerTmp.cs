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
using System.Collections.Generic;

namespace mmuc_zombie.app.model
{
    public class PlayerTmp : UserTmp, INotifyPropertyChanged
    {
        private bool isExpanded;

        public PlayerTmp(String nickname)
        {
            this.NickName = nickname;
            //this.Avatar = "ext/img/avatar.png";
            this.Avatar = "/mmuc-zombie;component/ext/img/avatar.png";            
        }

        public IList<HistoryTmp> History
        {
            get;
            set;
        }

        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }
            set
            {
                if (this.isExpanded != value)
                {
                    this.isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
