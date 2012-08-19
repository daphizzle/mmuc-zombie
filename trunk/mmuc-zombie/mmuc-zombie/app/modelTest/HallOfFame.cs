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

namespace mmuc_zombie.app.model
{
    public class HallOfFame
    {
        public String Category { get; set; }
        public DateTime Range { get; set; }
        public IList<PlayerTmp> Survivors { get; set; }
        public IList<PlayerTmp> Zombies { get; set; }

        public HallOfFame(String category, DateTime range)
        {
            this.Category = category;
            this.Range = range;
        }

        public bool loadSurvivors(int topk, DateTime start, DateTime end)
        {
            Survivors = new List<PlayerTmp>(6);
            Survivors.Add(new PlayerTmp("dpuschmann"));
            Survivors.Add(new PlayerTmp("lespin"));
            Survivors.Add(new PlayerTmp("jlahann"));
            Survivors.Add(new PlayerTmp("mvidriales"));
            Survivors.Add(new PlayerTmp("lcampos"));
            Survivors.Add(new PlayerTmp("mbader"));

            return true;
        }
    }
}
