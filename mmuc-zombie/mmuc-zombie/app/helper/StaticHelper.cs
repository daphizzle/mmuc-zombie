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

namespace mmuc_zombie.app.helper
{
    public class StaticHelper
    {


        public static T GetParentOfType<T>(DependencyObject item) where T : DependencyObject
        {
            if (item == null) throw new ArgumentNullException("item");
            T result = null;
            var parent = VisualTreeHelper.GetParent(item);
            if (parent == null) return result;
            else if (parent.GetType().IsSubclassOf(typeof(T)))
            {
                result = (T)parent;
            }
            else result = GetParentOfType<T>(parent);
            return result;
        }
    }
}
