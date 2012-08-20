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
using mmuc_zombie.app.model;
using System.Diagnostics;


public class Games : MyParseObject
{
    //pending = 0, active = 1, finshed = 2
    public int state { get; set; } 
    public int players { get; set; }
    public Boolean privateGame {get;set;}
    public String name { get; set; }
    public int radius { get; set; }
    public int zombiesCount { get; set; }
    public DateTime? startTime{get;set;}
    public DateTime? endTime { get; set; }
    public string locationId { get; set; }
    public string description { get; set; }
    public string ownerId { get; set; }


    //static public void findNearActiveGames(MyListener listener)
    //{
    //    PhoneApplicationService service = PhoneApplicationService.Current;
    //    var parse = new Driver();
    //    User user = (User)service.State["user"];
    //    var latitudeOffsetUp = user.latitude + 10;
    //    var latitudeOffsetDown = user.latitude - 10;
    //    parse.Objects.Query<Games>().Where(c => c.latitude < latitudeOffsetUp && c.latitude > latitudeOffsetDown).Execute(r =>
    //    {
    //        if (r.Success)
    //        {
    //            List<Games> found = (List<Games>)r.Data.Results;
    //            List<MyParseObject> list = new List<MyParseObject>();
    //            foreach (Games g in found)
    //            {
    //                list.Add(g);
    //            }
    //            listener.onDataChange(list);
    //        }
    //    });
        
    
    //}

    static public void findPendingGames(MyListener listener)
    {
        var parse = new Driver();
        parse.Objects.Query<Games>().Where(c => c.state ==0).Execute(r =>
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

    public void create(List<MyLocation> list)
    {
        var parse = new Driver();
        parse.Objects.Save(this, r =>
            {
                if (r.Success)
                {
                    Id = r.Data.Id;
                    foreach (MyLocation l in list)
                    {
                        l.gameId = Id;
                        Debug.WriteLine("(" + l.latitude + "," + l.longitude + ")");
                        l.create();
                        
                    }

                }
            });
    }

}