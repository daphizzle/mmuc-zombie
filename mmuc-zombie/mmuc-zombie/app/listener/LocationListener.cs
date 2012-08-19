using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


    class LocationListener : MyListener
    {
        User user;

        public LocationListener(User user)
        {
            this.user = user;
        }

        public void onDataChange(List<MyParseObject> list)
        {
            //there is only one MyParseObject
            user.locationId = ((Location)list[0]).Id;
        }

        public void onDataChange()
        {
        }

    }

