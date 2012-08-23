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
using System.Collections.Generic;
using Microsoft.Phone.Shell;

public class Roles : MyParseObject
    {
        public string userId { get; set; } 
        public string gameId { get; set; }
        public DateTime? endTime { get; set; }
        public DateTime? startTime { get; set; }
        public int rank { get; set; }
        //gets id from MyParsobject so not needed imho
        //public string roleId { get; set; }
        public string roleType { get; set; }
        public new void create()
        {
            PhoneApplicationService service = PhoneApplicationService.Current;
            var user = (User)service.State["user"];
            var parse = new Driver();
            parse.Objects.Save(this, r =>
            {
                if (r.Success)
                {
                    user.activeRole = r.Data.Id;
                    user.update();
                }
            });
        }

    }
