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
using mmuc_zombie.app.model;
using Parse;

namespace mmuc_zombie.app.listener
{
    public class LocationListener : MyListener
    {
        User user;

        public LocationListener(User user)
        {
            this.user = user;
        }

        public void onDataChange(List<MyParseObject> list)
        {
            //there is only one MyParseObject
            string locId = ((MyLocation)list[0]).Id;
            var parse = new Driver();
            user.locationId = locId;
            user.updateCurrentUser();
        }
    }
}
