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

namespace mmuc_zombie
{
    public partial class MyProfile : PhoneApplicationPage
    {
        public MyProfile()
        {
            InitializeComponent();
        }

        private void appbar_save_Click(object sender, EventArgs e)
        {
            MessageBox.Show("save");
        }

        private void appbar_cancel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("cancel");
        }
    }
}