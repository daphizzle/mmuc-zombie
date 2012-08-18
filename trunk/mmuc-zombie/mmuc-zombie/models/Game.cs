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
     
    public int minPlayers { get; set; }
    public Boolean privateGame {get;set;}
    public int maxPlayers { get; set; }
    public String name { get; set; }
    public int radius { get; set; }
    public int zombiesCount { get; set; }
    public DateTime? startTime{get;set;}
    public string locationId { get; set; }
    public string ownerId { get; set; }


}