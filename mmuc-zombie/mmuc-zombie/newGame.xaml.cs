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
using Microsoft.Phone.Shell;

namespace mmuc_zombie
{
    public partial class NewGame : PhoneApplicationPage
    {
        public NewGame()
        {
            InitializeComponent();
        }

   

     
        

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Double value =Math.Round(playerSlider.Value*10.0);
            playerValueTextbox.Text = value.ToString() ;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void saveButtonClick(object sender, EventArgs e)
        {
            Location location = new Location();
            var game = new Games();
            PhoneApplicationService service = PhoneApplicationService.Current;
            var user = (User)service.State["user"];
            game.ownerId = user.Id;
            game.name = nameTextfield.Text;
            game.locationId = user.locationId;
            game.players = (int)Math.Round(playerSlider.Value*10.0);
          //  game.startTime = new DateTime(startDatePicker.Value.Value.Year, startDatePicker.Value.Value.Month,startDatePicker.Value.Value.Day,startTimePicker.Value.Value.Hour,startTimePicker.Value.Value.Minute,0).ToString();

          //  game.startTime = "blubb";
          //  game.endTime = new DateTime(endDatePicker.Value.Value.Year, endDatePicker.Value.Value.Month, endDatePicker.Value.Value.Day, endTimePicker.Value.Value.Hour, endTimePicker.Value.Value.Minute, 0).ToString();
            game.create();
         
      

        }

        private void cancleButtonClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/menu.xaml", UriKind.Relative));
        }

     
       

        

    }

 
}
