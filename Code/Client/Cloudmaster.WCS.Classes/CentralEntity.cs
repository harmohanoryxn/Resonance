using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Cloudmaster.WCS.Classes.Helpers;

namespace Cloudmaster.WCS.Classes
{
    public abstract class CentralEntity : DependencyObject
    {
        internal CentralEntity () 
        {
            FormSummarys = new ObservableCollection<FormSummary>();
            Tasks = new ObservableCollection<Task>();
            Orders = new ObservableCollection<Order>();

            Orders.CollectionChanged += new NotifyCollectionChangedEventHandler(Orders_CollectionChanged);
        }

        public CentralEntity(Guid id)
        {
            Id = id;

            FormSummarys = new ObservableCollection<FormSummary>();
            Tasks = new ObservableCollection<Task>();
            Orders = new ObservableCollection<Order>();

            Orders.CollectionChanged += new NotifyCollectionChangedEventHandler(Orders_CollectionChanged);
        }

        void Orders_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NextOrder = null;

            DateTime now = DateTime.Now;

            var outstandingOrders = Orders.Where(o => o.RequestedDateTime > now);

            if (outstandingOrders.Count() > 0)
            {
                NextOrder = outstandingOrders.Min(o => o.RequestedDateTime);
            }
        }

        public Guid Id
        {
            get { return (Guid)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(Guid), typeof(CentralEntity), new UIPropertyMetadata(null));

        public string EntityType
        {
            get { return (string)GetValue(EntityTypeProperty); }
            set { SetValue(EntityTypeProperty, value); }
        }

        public static readonly DependencyProperty EntityTypeProperty =
            DependencyProperty.Register("EntityType", typeof(string), typeof(CentralEntity), new UIPropertyMetadata(string.Empty));

        public ObservableCollection<FormSummary> FormSummarys
        {
            get { return (ObservableCollection<FormSummary>)GetValue(FormSummarysProperty); }
            set { SetValue(FormSummarysProperty, value); }
        }

        public static readonly DependencyProperty FormSummarysProperty =
            DependencyProperty.Register("FormSummarys", typeof(ObservableCollection<FormSummary>), typeof(CentralEntity), new UIPropertyMetadata(null));

        public ObservableCollection<Task> Tasks
        {
            get { return (ObservableCollection<Task>)GetValue(TasksProperty); }
            set { SetValue(TasksProperty, value); }
        }

        public static readonly DependencyProperty TasksProperty =
            DependencyProperty.Register("Tasks", typeof(ObservableCollection<Task>), typeof(CentralEntity), new UIPropertyMetadata(null));

        public ObservableCollection<Order> Orders
        {
            get { return (ObservableCollection<Order>)GetValue(OrdersProperty); }
            set { SetValue(OrdersProperty, value); }
        }

        public static readonly DependencyProperty OrdersProperty =
            DependencyProperty.Register("Orders", typeof(ObservableCollection<Order>), typeof(CentralEntity), new UIPropertyMetadata(null));

        public DateTime? NextOrder
        {
            get { return (DateTime?)GetValue(NextOrderProperty); }
            set { SetValue(NextOrderProperty, value); }
        }

        public static readonly DependencyProperty NextOrderProperty =
            DependencyProperty.Register("NextOrder", typeof(DateTime?), typeof(CentralEntity), new UIPropertyMetadata(null));
    }
}
