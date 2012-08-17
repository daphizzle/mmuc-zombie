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

public class Zombie : MyParseObject
    {
        public int level { get; set; }
        public int infectionCount{get;set;}
        public int quarantineMode { get; set; }
    }

