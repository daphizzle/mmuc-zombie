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

namespace mmuc_zombie
{
    public class OnStartupListener : MyListener
    {
        public void onDataChange(List<MyParseObject> o)
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();

            using(var writer = new StreamWriter(store.OpenFile("user.txt",FileMode.Append)))
            {
                User user = (User)o[0];
                writer.Write(user.Id);
            }
        }
    }
}
