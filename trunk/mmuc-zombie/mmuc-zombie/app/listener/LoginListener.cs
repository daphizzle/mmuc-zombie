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
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                Progressbar.HideProgressBar();
            });
            //there is only one MyParseObject            
            //User.set((User)list[0]);
            service.State["user"] = (User)list[0];
            App.AccessToken = ((User)list[0]).FacebookToken;
            PositionRetriever.startPositionRetrieving(100);
            
        }
       

    }
}
