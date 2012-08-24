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
using Microsoft.Phone.Shell;
using mmuc_zombie.app.facebook;
using mmuc_zombie.app.helper;
using mmuc_zombie;

public class User :  MyParseObject
{
    public string activeGame { get; set; }


    public int status { get; set; }
    //int=0: "idle mode" he is doing nothing, he ha no pending games; no timertask is running, if he joins a game he switch to status 1
    //int=1: "lobby-mode" user has joined a game , timertask checks if gameowner creates a game. if he does user switch to status 3. user can leave:if he has pending games left he switch to status 1, if not he switch to status 0.
    //int=2: "ingame" mode  many things are checked ...(infection, userlocation ...)if he leaves a game: it is checked if he has pending events if yes he switches to status 1 else 0
    public string activeRole { get; set; }
    public string UserName{get;set;}
    public string Password{get;set;}    
    public string email { get; set; }    
    public string locationId { get; set; }
    public string NickName { get; set; }
    public string Facebook { get; set; }
    public string DeviceID { get; set; }
    //public FBUser _facebook { get; set; }
    //public Device _deviceID { get; set; }
    

    public User()
    {
        status = (int)Constants.USERGAMEMODES.INIT;
        activeRole = "";
        UserName = "";
        Password = "";
        Facebook = "";
        email = "";
        locationId = "";
        DeviceID = "";
        NickName = "";      
    }

    public static void set(User user)
    {
        PhoneApplicationService service = PhoneApplicationService.Current;
        service.State["user"] = user;        
    }

    public static User get()
    {
        PhoneApplicationService service = PhoneApplicationService.Current;
        return (User)service.State["user"];
    }

    public void updateCurrentUser()
    {
        //PhoneApplicationService service = PhoneApplicationService.Current;
        //service.State["user"] = this;
        set(this);
        update();
    }

    public new void  update()
    {        
        var parse = new Driver();
        parse.Objects.Update<User>(this.Id).
            Set(u => u.status, status).
            Set(u => u.activeGame, activeGame).
            Set(u => u.activeRole, activeRole).
            Set(u => u.UserName, UserName).
            Set(u => u.Password, Password).
            Set(u => u.Facebook, Facebook).
            Set(u => u.email, email).
            Set(u => u.locationId, locationId).
            Set(u => u.DeviceID, DeviceID).
            Set(u => u.NickName, NickName).
            Execute(r =>
                {                    
                    if (r.Success)
                    {
                        Debug.WriteLine("User : " + Id + " successfull updated");
                    }
                    //else
                    //{
                    //    Debug.WriteLine("User : " + Id + " error while updating. " + r.Error.Message);
                    //}
                    
                });
        
    }


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
                User.set(user);
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