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

public class User :  ParseObject
{

    
    public string UserName{get;set;}
    public string Password{get;set;}
    
    public void addFriend (User u){
        Friend f=new Friend();
        f.user = Id;
        f.friend = u.Id;
        var parse = new Driver();
        parse.Objects.Save(f);
       
    }

    public Boolean create(){
        Boolean success=false;
        var parse = new Driver();
        parse.Objects.Save(this,r =>
        {      
           if (r.Success)
           {
                Id = r.Data.Id;
                success=true;
            }
            else
            {
                //log r.Error.Message;
               
             }
        });  
        return success;
    }
    public Boolean update(){
        Boolean success=false;
        var parse = new Driver();
        parse.Objects.Update(this, r => 
        {
              if (r.Success)
              {
                  success=true;
              }
              else 
              {
                //log r.Error.Message;
              }
        });
        return success;
    }
    public Boolean destroy(){
        ParseConfiguration.Configure("w8I4cwfDTXeMzvPPSzkAiinbnkMWijhZkZ7Jnxwd", "BbL0rdiCCzC2yE0fdtm7da6nKEXdBt2EXDTHEvVT", "j2si7yNE2Fg8ORdEp3lG3f7zXY2bv2Gb8zpDR57p");
        Boolean success = false;
        var parse = new Driver(); 
    //    parse.Objects.Delete<User>("123"):
        return success;
    
    }
    
}