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
using System.Collections.Generic;
using Parse;
using mmuc_zombie.app.helper;
using System.ComponentModel;

namespace mmuc_zombie.app.model
{
    public class Statistics : INotifyPropertyChanged
    {
        public String Category { get; set; }
        public DateTime Range { get; set; }
        public IList<Player> Players { get; set; }

        public int playedGames { get; set; }
        public int winGames { get; set; }
        public int lostGames { get; set; }
        public int gamesSurvivor { get; set; }
        public int gamesZombie { get; set; }

        private bool hasNoOptions;
        private bool isExpanded;
        public event PropertyChangedEventHandler PropertyChanged;

        public Statistics()
        {
            this.Category = "";
            this.Range = DateTime.Now ;
            this.Players = new List<Player>();
            playedGames = winGames = lostGames = gamesSurvivor = gamesZombie = 0;
        } 

        public Statistics(String category, DateTime range)
        {
            this.Category = category;
            this.Range = range;
            this.Players = new List<Player>();
            playedGames = winGames = lostGames = gamesSurvivor = gamesZombie = 0;
        }

        public bool HasNoOptions
        {
            get
            {
                return this.hasNoOptions;
            }
            set
            {                
                this.hasNoOptions = (this.Players.Count == 0);
                
                if (!this.hasNoOptions) 
                isExpanded = false;                
            }
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
