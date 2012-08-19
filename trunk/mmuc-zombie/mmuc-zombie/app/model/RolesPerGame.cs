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

public class Roles : MyParseObject
    {
        string userId { get; set; } 
        string gameId { get; set; }
        public DateTime? endTime { get; set; }
        public DateTime? startTime { get; set; }
        public int rank { get; set; }
        string roleId { get; set; }
        string roleType { get; set; }
        
    }
