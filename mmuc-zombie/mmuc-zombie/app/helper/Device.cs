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
using Microsoft.Phone.Info;

namespace mmuc_zombie.app.helper
{
    public class Device
    {
        byte[] id;

        public Device()
        {
            GetDeviceUniqueID();
        }

        public byte[] GetDeviceUniqueID()
        {
            id = null;
            object uniqueId;
            if (DeviceExtendedProperties.TryGetValue("DeviceUniqueId", out uniqueId))
                id = (byte[])uniqueId;
            return id;
        }

        public string toString()
        {
            String str = "";

            if (id != null)
            {
                foreach (byte b in id)
                {
                    str = str + b.ToString();
                }
            }
            return str;
        }

    }
}
