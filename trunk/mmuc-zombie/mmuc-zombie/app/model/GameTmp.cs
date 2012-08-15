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
    public class GameTmp
    {
        public GameTmp()
        {
            Name = null;
            Start = new DateTime();
            End = new DateTime();
        }

        public GameTmp(String name, DateTime start, DateTime end)
        {
            Name = name;
            Start = start;
            End = end;
        }

        public String Name { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; } 

    }
}
