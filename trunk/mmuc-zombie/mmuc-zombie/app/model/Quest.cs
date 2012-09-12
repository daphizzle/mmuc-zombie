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

namespace mmuc_zombie.app.model
{
    public class Quest : MyParseObject
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int healthPlus { get; set; }
        public bool active { get; set; }
        public string gameId { get; set; }

        public Quest()
        {
            active = false;
        }

        public void update(Action<Response<DateTime>> callback)
        {
            var parse = new Driver();
            parse.Objects.Update<Quest>(this.Id).
                Set(u => u.latitude, latitude).
                Set(u => u.longitude, longitude).
                Set(u => u.healthPlus, healthPlus).
                Set(u => u.active, active).
                Set(u => u.gameId, gameId).
                Execute(callback);
        }
    }
}
