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
using mmuc_zombie;

    public class Listener
    {
        public static StartupListener onStartupListener = new StartupListener();
        public static LoginListener loginListener = new LoginListener();
    }
