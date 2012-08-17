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
using mmuc_zombie.app.model;

namespace mmuc_zombie
{
    public partial class HallFame : PhoneApplicationPage
    {
        public HallFame()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            loadSurvivors();
        }

        private void loadSurvivors()
        {
            List<HallOfFame> ranges = new List<HallOfFame>(3);

            ranges.Add(new HallOfFame("Today",DateTime.Now));
            ranges.Add(new HallOfFame("Last week",DateTime.Now));
            ranges.Add(new HallOfFame("Last month", DateTime.Now));

            foreach (HallOfFame hof in ranges)
            {
                hof.loadSurvivors(6, DateTime.Today, DateTime.Today);                    
            }

            this.listBox.ItemsSource = ranges;
        }
    }
}
