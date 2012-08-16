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
using Parse;


public class Game : MyParseObject
{
    public Boolean private_mode{get;set;}
    public int min_participants{get;set;}
    public int max_participants{ get; set; }
    public int radius { get; set; }
    public int zombie_quota { get; set; }
    public DateTime start { get; set; }
    public DateTime end { get; set; }
    public string game { get; set; }

    




}