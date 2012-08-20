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
using Microsoft.Phone.Shell;
using System.Collections.Generic;


public class Games : MyParseObject
{
     
    public int players { get; set; }
    public Boolean privateGame {get;set;}
    public String name { get; set; }
    public int radius { get; set; }
    public int zombiesCount { get; set; }
    public string startTime{get;set;}
    public string endTime { get; set; }
    public GeoPoint location { get; set; }
    public string ownerId { get; set; }


    static public void findNearActiveGames(MyListener listener)
    {
        PhoneApplicationService service = PhoneApplicationService.Current;
        var parse = new Driver();
        User user = (User)service.State["user"];
        if (user.location != null)
        {
            parse.Objects.Query<Games>().Where(c => (c.location.Latitude - user.location.Latitude) < 10).Execute(r =>
            {
                if (r.Success)
                {
                    List<Games> found = (List<Games>)r.Data.Results;
                    List<MyParseObject> list = new List<MyParseObject>();
                    foreach (Games g in found)
                    {
                        list.Add(g);
                    }
                    listener.onDataChange(list);
                }
            });
        }
        else
        {
            //TODO: display error
        }
    
    }

}