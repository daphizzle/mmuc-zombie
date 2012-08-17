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


public class QuarantineMode : MyParseObject
    {
        public DateTime? idleTime { get; set; }
        public int duration { get; set; }
        public Boolean disloyal { set; get; } //Zombies can be killed
    }

