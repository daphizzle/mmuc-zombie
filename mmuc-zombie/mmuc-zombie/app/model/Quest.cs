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

namespace mmuc_zombie.app.model
{
    public class Quest : MyParseObject
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int healthPlus { get; set; }
    }
}
