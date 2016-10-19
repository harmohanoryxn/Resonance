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
using Cloudmaster.WCS.Ward.Model;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS.Ward.Commands;
using System.Windows.Threading;

namespace Cloudmaster.WCS.Ward.Controls
{
    public partial class ViewDepartmentSchedule : UserControl
    {
        public ViewDepartmentSchedule()
        {
            InitializeComponent();

            CreateCommandBindings();

            CreateRefreshFilterTimer();
        }

        private void CreateRefreshFilterTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();

            timer.Tick += new EventHandler(RefreshFilterTimer_Tick);
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Start();
        }

        void RefreshFilterTimer_Tick(object sender, EventArgs e)
        {
            CollectionViewSource collectionViewSource = (CollectionViewSource) this.TryFindResource("filteredOrdersView");

            collectionViewSource.View.Refresh();
        }

        private void CreateCommandBindings()
        {
            // Appointment Commands

            CommandBindings.Add(new CommandBinding(OrderCommands.SendAcknowledgementCommand, OrderCommands.SendAcknowledgementCommandExecuted, OrderCommands.SendAcknowledgementCommandCanExecute));
        }
       

        private void lstAppointments_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            Order order = (Order) e.Item;

            bool isCompleted = (order.ObservationEndDateTime.HasValue);

            e.Accepted = !isCompleted;
        }
    }
}
