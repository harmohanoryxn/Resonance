using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Cloudmaster.WCS.Ward.Commands;
using Cloudmaster.WCS.Ward.Model;
using Cloudmaster.WCS.Ward.Processing;

namespace Cloudmaster.WCS.Ward
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            InitializeLabelsUpdateTimer();

            InitializeRefreshTimer();

            InitializeSecurityTimer();

            CreateCommandBindings();
        }

        private void CreateCommandBindings()
        {
            // SecurityViewModel Commands

            CommandBindings.Add(new CommandBinding(SecurityCommands.LockNowCommand, SecurityCommands.LockNowCommandExecuted, SecurityCommands.LockNowCommandCanExecute));
        }

        private void InitializeLabelsUpdateTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();

            timer.Tick += new EventHandler(LabelsUpdateTimer_Tick);
            timer.Interval = TimeSpan.FromSeconds(60);
            timer.Start();
        }

        void LabelsUpdateTimer_Tick(object sender, EventArgs e)
        {
            WardModel.Instance.WardLabels.UpdateOrderStatus();
        }

        private void InitializeRefreshTimer()
        {
            int intervalInSeconds = int.Parse(Cloudmaster.WCS.Ward.Properties.Settings.Default.RefreshInterval);

            DispatcherTimer timer = new DispatcherTimer();

            timer.Tick += new EventHandler(RefreshTimer_Tick);
            timer.Interval = TimeSpan.FromSeconds(intervalInSeconds);

            timer.Start();
        }

        private void InitializeSecurityTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();

            timer.Tick += new EventHandler(SecurityTimer_Tick);
            timer.Interval = TimeSpan.FromSeconds(10);

            timer.Start();
        }

        void RefreshTimer_Tick(object sender, EventArgs e)
        {
            WardProcessorViewModel.Instance.RefreshOrders();
        }

        void SecurityTimer_Tick(object sender, EventArgs e)
        {
            if (WardModel.Instance.SecurityViewModel.DetermineIfLocked())
            {
                SecurityCommands.LockNow();
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            WardModel.Instance.SecurityViewModel.RecordLastUsageTime();
        }
    }
}
