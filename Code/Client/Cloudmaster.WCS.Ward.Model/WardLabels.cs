using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Cloudmaster.WCS.Classes;

namespace Cloudmaster.WCS.Ward.Model
{
    public class WardLabels : DependencyObject
    {
        public string WardDisplayName
        {
            get { return (string)GetValue(WardDisplayNameProperty); }
            set { SetValue(WardDisplayNameProperty, value); }
        }

        public static readonly DependencyProperty WardDisplayNameProperty =
            DependencyProperty.Register("WardDisplayName", typeof(string), typeof(WardLabels), new UIPropertyMetadata(string.Empty));

        public string OrdersStatus
        {
            get { return (string)GetValue(OrdersUpdateStatusProperty); }
            set { SetValue(OrdersUpdateStatusProperty, value); }
        }

        public static readonly DependencyProperty OrdersUpdateStatusProperty =
            DependencyProperty.Register("OrdersStatus", typeof(string), typeof(WardLabels), new UIPropertyMetadata(string.Empty));

        public void UpdateOrderStatus()
        {
            TimeSpan twoMinutes = TimeSpan.FromMinutes(2);
            TimeSpan oneHour = TimeSpan.FromHours(1);
            TimeSpan oneDay = TimeSpan.FromDays(1);

            DateTime now = DateTime.Now;

            DateTime? lastUpodated = WardModel.Instance.Feeds.Orders.LastSyncronized;

            if (!lastUpodated.HasValue)
            {
                OrdersStatus = "Loading";
            }
            else
            {
                if (now.Subtract(lastUpodated.Value) < twoMinutes)
                {
                    OrdersStatus = "Up-to-date";
                }
                else if (now.Subtract(lastUpodated.Value) < oneHour)
                {
                    int mintues = now.Subtract(lastUpodated.Value).Minutes;

                    OrdersStatus = string.Format("Last updated {0} minutes ago", mintues);
                }
                else if (now.Subtract(lastUpodated.Value) < oneDay)
                {
                    int hours = now.Subtract(lastUpodated.Value).Hours;

                    OrdersStatus = string.Format("Last updated {0} hours ago", hours);
                }
                else
                {
                    OrdersStatus = string.Format("Last updated over 1 day ago");
                }
            }
        }
    }
}
