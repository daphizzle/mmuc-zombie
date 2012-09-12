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
using Microsoft.Phone.Controls;
using System.ComponentModel;
using Phone.Controls;


namespace mmuc_zombie.app.helper
{
    public class Progressbar
    {
        private static ProgressIndicator progress;
        private static BackgroundWorker backgroundWorker;

        public static void InitGLobalProgressBar()
        {
            progress = new ProgressIndicator();
            progress.ProgressType = ProgressTypes.WaitCursor;
            backgroundWorker = new BackgroundWorker();
        }

        public static void ShowProgressBar(string msg, DoWorkEventHandler doWork, RunWorkerCompletedEventHandler completed, ProgressChangedEventHandler changed)
        {
            backgroundWorker = new BackgroundWorker();


            if (changed != null)
            {
                backgroundWorker.DoWork -= doWork;
                backgroundWorker.DoWork += doWork;
            }

            if (completed != null)
            {
                backgroundWorker.RunWorkerCompleted -= completed;
                backgroundWorker.RunWorkerCompleted += completed;
            }

            if (changed != null)
            {
                backgroundWorker.ProgressChanged -= changed;
                backgroundWorker.ProgressChanged += changed;
            }

            backgroundWorker.WorkerReportsProgress = false;
            progress.SetText(msg);
            progress.Show();

            backgroundWorker.RunWorkerAsync();
        }

        public static void ShowProgressBar(string msg)
        {
            backgroundWorker.WorkerReportsProgress = false;
            progress.SetText(msg);
            progress.Show();
        }

        public static void ShowProgressBar()
        {
            ShowProgressBar(null);
        }
        public static void HideProgressBar()
        {
            if (progress.IsEnabled)
                progress.Hide();
        }
    }
}
