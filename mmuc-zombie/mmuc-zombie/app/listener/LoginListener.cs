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
using Microsoft.Phone.Shell;
using System.Collections.Generic;
using System.Diagnostics;
using mmuc_zombie.app.helper;
using mmuc_zombie.app.model;


namespace mmuc_zombie
{
    public class LoginListener : MyListener
    {
        private PhoneApplicationService service = PhoneApplicationService.Current;

        public void onDataChange(List<MyParseObject> list)
        {
            Debug.WriteLine("Logged in as" + ((User)list[0]).UserName);
            //there is only one MyParseObject
            //App.User = (User)list[0];
            //User.set((User)list[0]);
            service.State["user"] = (User)list[0];
            PositionRetriever.startPositionRetrieving(100);
            
        }
       

    }
}
