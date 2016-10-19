using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;
using Cloudmaster.WCS.Model;
using Cloudmaster.WCS.Classes;
using System.Windows;
using Cloudmaster.WCS.Forms.Processing;

namespace Cloudmaster.WCS.Controls.Forms.Commands
{
    public class OutboxCommands
    {
        #region Open

        public static RoutedUICommand OpenOutBoxCommand = new RoutedUICommand("Open Outbox", "OpenOutBoxCommand", typeof(UserControl));

        public static void OpenOutBoxCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            FormManager.Instance.NavigationIndex = (int) FormNavigationIndex.Outbox;
        }

        public static void OpenOutBoxCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

        #region Send Item In Queue

        public static RoutedUICommand SendItemInQueueCommand = new RoutedUICommand("Submit Form", "SubmitFormInQueueCommand", typeof(Window));

        public static void SendItemInQueueCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            FormInstance formToSubmit = FormManager.Instance.Outbox.SelectedItem;

            AttemptToSendForm(formToSubmit);
        }

        public static void SendItemInQueueCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            FormInstance form = FormManager.Instance.Outbox.SelectedItem;

            if (form != null)
            {
                e.CanExecute = !((form.OutboxStatus == OutboxStatus.Completed) | (form.Status == OutboxStatus.Sending));
            }
        }

        #endregion

        #region Methods

        public static void AttemptToSendForm(FormInstance formToSubmit)
        {
            SubmitFormProcessor uploader = new SubmitFormProcessor();

            uploader.Submit(formToSubmit);
        }

        public static void AttemptToSendOutstandingForms()
        {
            foreach (FormInstance formInstance in FormManager.Instance.Outbox.Queue)
            {
                if (!((formInstance.OutboxStatus == OutboxStatus.Completed) | (formInstance.Status == OutboxStatus.Sending)))
                {
                    AttemptToSendForm(formInstance);
                }
            }
        }

        #endregion
    }
}
