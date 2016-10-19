using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS.Controls.Scheduling;
using Cloudmaster.WCS.Ward.Model;
using Cloudmaster.WCS.Ward.Processing;
using System.Windows.Controls;


namespace Cloudmaster.WCS.Ward.Commands
{
    public class OrderCommands
    {
        #region View Orders

        public static RoutedUICommand ViewOrdersCommand = new RoutedUICommand("View Orders", "ViewOrdersCommand", typeof(Window));

        public static void ViewOrdersCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            WardModel.Instance.NavigationViewModel.NavigationIndex = (int) NavigationIndex.Schedule;
        }

        public static void ViewOrdersCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

        #region Send Acknowledgments

        public static RoutedUICommand SendAcknowledgementCommand = new RoutedUICommand("SendAcknowledgementCommand", "SendAcknowledgementsCommand", typeof(UserControl));

        public static void SendAcknowledgementCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Order selectedOrder = WardModel.Instance.SelectedOrder;

            SendAcknowledgement(selectedOrder);
        }

        private static void SendAcknowledgement(Order order)
        {
        	var appointment = new AppointmentViewModel(order);
            SendOrderAcknowledgementsProcessor processor = new SendOrderAcknowledgementsProcessor();

			bool requiresInjectionAcknowledgement = appointment.RequiresInjectionAcknowledgement() || order.Metadata.IsInjectionAcknowledged;
			bool requiresFastingAcknowledgement = appointment.RequiresFastingAcknowledgement() || order.Metadata.IsFastingAcknowledged;
			bool requiresPrepAcknowledgement = appointment.RequiresPrepAcknowledgement() || order.Metadata.IsPrepWorkAcknowledged;
			bool requiresExamAcknowledgement = appointment.RequiresExamAcknowledgement() || order.Metadata.IsFastingAcknowledged;

            processor.ExecuteInBackground(order.PlaceOrderId, requiresFastingAcknowledgement, requiresPrepAcknowledgement, requiresExamAcknowledgement, requiresInjectionAcknowledgement);
        }

        public static void SendAcknowledgementCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Order selectedOrder = WardModel.Instance.SelectedOrder;
			var appointment = new AppointmentViewModel(selectedOrder);

            if (selectedOrder != null)
            {
                if (AppointmentExtensions.IsActiveAndAssigned(selectedOrder))
                {
                    if (selectedOrder != null)
                    {
						bool requiresInjectionAcknowledgement = appointment.RequiresInjectionAcknowledgement();
						bool requiresFastingAcknowledgement = appointment.RequiresFastingAcknowledgement();
						bool requiresPrepAcknowledgement = appointment.RequiresPrepAcknowledgement();
						bool requiresExamAcknowledgement = appointment.RequiresExamAcknowledgement();

                        e.CanExecute = (requiresFastingAcknowledgement | requiresPrepAcknowledgement | requiresExamAcknowledgement | requiresInjectionAcknowledgement);
                    }
                }
            }
        }

        #endregion
    }
}
