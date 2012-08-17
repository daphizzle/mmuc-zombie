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


public class ZoneInfection : MyParseObject
    {
        public int radius { get; set; }
        public int maxLevel { get; set; }
        public int infectionDuration { get; set; }
        public int radiusIncFactor { get; set; }
        public int infectionDurationFactor { get; set; }
    }

