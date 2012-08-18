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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var game = new Game();
            PhoneApplicationService service = PhoneApplicationService.Current;
            var user=(User) service.State["user"];
            game.ownerId = user.Id;
            game.name = textBox1.Text;
            game.locationId = user.locationId;
            game.create();
        }

     
    }

 
}
