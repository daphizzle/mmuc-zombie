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
using System.Collections.Generic;
using System.Diagnostics;

public class User :  MyParseObject
{



    public string UserName{get;set;}
    public string Password{get;set;}
    public string facebook { get; set; }
    public string email { get; set; }
    public string locationId { get; set; }

   
      
   

    public static void find(string userId, MyListener listener)
    {
        var driver = new Driver();
        driver.Objects.Get<User>(userId, r =>
        {
            if (r.Success)
            {
                var user = r.Data;
                Debug.WriteLine("Logged in as user" + user.UserName);
                List<MyParseObject> list = new List<MyParseObject>();
                list.Add(user);
                listener.onDataChange(list);

            }
        });

    }

    public static void find(List<string> userIds, MyListener listener)
    {
        foreach (string userId in userIds)
 
        {
        }
    }
        
 
    
}