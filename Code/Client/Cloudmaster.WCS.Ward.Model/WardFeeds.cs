using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloudmaster.WCS.Model.Feeds;
using System.Windows;

namespace Cloudmaster.WCS.Ward.Model
{
    public class DepartmentFeeds : DependencyObject
    {
        public DepartmentFeeds()
        {
            Orders = new OrdersFeed();
            OrderRequests = new OrderRequestsFeed();
        }

        public OrderRequestsFeed OrderRequests
        {
            get { return (OrderRequestsFeed)GetValue(OrderRequestsProperty); }
            set { SetValue(OrderRequestsProperty, value); }
        }

        public static readonly DependencyProperty OrderRequestsProperty =
            DependencyProperty.Register("OrderRequests", typeof(OrderRequestsFeed), typeof(DepartmentFeeds), new UIPropertyMetadata(null));

        public OrdersFeed Orders
        {
            get { return (OrdersFeed)GetValue(OrdersProperty); }
            set { SetValue(OrdersProperty, value); }
        }

        public static readonly DependencyProperty OrdersProperty =
            DependencyProperty.Register("Orders", typeof(OrdersFeed), typeof(DepartmentFeeds), new UIPropertyMetadata(null));
    }
}
