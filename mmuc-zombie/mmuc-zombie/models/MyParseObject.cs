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
using System.Collections.Generic;
using Parse;
using System.Diagnostics;



public class MyParseObject :ParseObject{

    public void create(){
        create(null);
    }
    public void create(MyListener listener){
        String s = this.GetType().Name;
        var parse = new Driver();
        parse.Objects.Save(this, r =>
        {
            if (r.Success)
            {
                this.Id = r.Data.Id;
                var createdAt = r.Data.CreatedAt;
                Debug.WriteLine(s+" " + this.Id + " stored");              
                List<MyParseObject> list =new List<MyParseObject>();
                list.Add(this);
                if (listener != null)
                {
                    listener.onDataChange(list);
                }
            }
            else
            {
                string msg = r.Error.Message;
                Debug.WriteLine(s+" " + this.Id + " Error :" + msg);
            }
        });
   
    }
    public void update()  {
        update(null);
    }
    public Boolean update(MyListener listener)
    {
        String s = this.GetType().Name;
        var parse= new Driver();
        parse.Objects.Update(this.Id, this, r =>
        {
            if (r.Success)
            {
                Debug.WriteLine(s+" " + this.Id + " saved");
                List<MyParseObject> list = new List<MyParseObject>();
                list.Add(this);
                if (listener != null)
                {
                    listener.onDataChange(list);
                }
            }
            else
            {
                string msg = r.Error.Message;
              
                Debug.WriteLine(s+" " + this.Id + " Error :" + msg);
            }
        });
        return true;

                

    }
    public Boolean destroy<T>(MyListener listener) where T:MyParseObject
    {
        ParseConfiguration.Configure("w8I4cwfDTXeMzvPPSzkAiinbnkMWijhZkZ7Jnxwd", "BbL0rdiCCzC2yE0fdtm7da6nKEXdBt2EXDTHEvVT", "j2si7yNE2Fg8ORdEp3lG3f7zXY2bv2Gb8zpDR57p");
        Boolean success = false;
        var parse = new Driver();

        parse.Objects.Delete<T>(this.Id, r =>
        {
            if (r.Success)
            {
                success = true;
                if (listener != null)
                {
                    listener.onDataChange(null);
                }
            }
            else
            {
                 var message=r.Error.Message;
                 int d=message.Length;
            }
        });
        return success;

    }

  }

