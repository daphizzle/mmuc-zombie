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

public class User : IParseUser, IParseObject
{

    public string Id { get; set; }
    public string UserName{get;set;}
    public string Password{get;set;}
    
    public void addFriend (User u){
        Friend f=new Friend();
        f.user = Id;
        f.friend = u.Id;
        var parse = new Driver();
        parse.Objects.Save(f);
       
    }

    public void create(){
        var parse = new Driver();
        parse.Objects.Save(this,r =>
        {      
           if (r.Success)
        {
           Id = r.Data.Id;
        }
        else
        {
            //log r.Error.Message;
        }
        });

        
    }
}