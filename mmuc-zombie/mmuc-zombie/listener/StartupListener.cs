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
using System.IO.IsolatedStorage;
using System.IO;
using System.Collections.Generic;
using Microsoft.Phone.Shell;
using System.Diagnostics;

namespace mmuc_zombie
{
    public class StartupListener : MyListener
    {
        private PhoneApplicationService service = PhoneApplicationService.Current;

        public void onDataChange(List<MyParseObject> o)
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();

            using(var writer = new StreamWriter(store.OpenFile("userId.txt",FileMode.Open)))
            {
                //there is only one MyParseObject
                User user = (User)o[0];
                writer.Write(user.Id);
                Debug.WriteLine("Writing user id into Isolated Store"+user.Id);
                service.State["user"] = user;
                new PositionRetriever();
            }
        }
    }
}
