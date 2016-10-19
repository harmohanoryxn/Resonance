using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;
using Cloudmaster.WCS.Model.Tasks;

namespace Cloudmaster.WCS.Controls.Tasks.Commands
{
    public class WizardCommands
    {
        #region Start Command

        public static RoutedUICommand StartCommand = new RoutedUICommand("StartCommand", "StartCommand", typeof(UserControl));

        public static void StartCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TaskManager.Instance.NavigationIndex = 0;
            TaskManager.Instance.IsWizardActive = true;
        }

        public static void StartCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (!TaskManager.Instance.IsWizardActive);
        }

        #endregion

        #region Finish Command

        public static RoutedUICommand FinishCommand = new RoutedUICommand("FinishCommand", "FinishCommand", typeof(UserControl));

        public static void FinishCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TaskManager.Instance.IsWizardActive = false;

            TaskWizardCompletedEventArgs args = new TaskWizardCompletedEventArgs () { Cancelled = false, TaskId = "123" };

            TaskManager.Instance.OnExitTaskEvent(args);
        }

        public static void FinishCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (TaskManager.Instance.IsWizardActive);
        }

        #endregion


        #region Next Command

        public static RoutedUICommand NextCommand = new RoutedUICommand("NextCommand", "NextCommand", typeof(UserControl));

        public static void NextCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TaskManager.Instance.NavigationIndex += 1;
        }

        public static void NextCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (TaskManager.Instance.NavigationIndex != (int)TaskNavigationIndex.ViewResults);
        }

        #endregion

        #region Back Command

        public static RoutedUICommand BackCommand = new RoutedUICommand("BackCommand", "BackCommand", typeof(UserControl));

        public static void BackCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TaskManager.Instance.NavigationIndex -= 1;
        }

        public static void BackCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (TaskManager.Instance.NavigationIndex != (int)TaskNavigationIndex.SelectTarget);
        }

        #endregion

        #region Cancel Command

        public static RoutedUICommand CancelCommand = new RoutedUICommand("CancelCommand", "CancelCommand", typeof(UserControl));

        public static void CancelCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TaskManager.Instance.IsWizardActive = false;

            TaskWizardCompletedEventArgs args = new TaskWizardCompletedEventArgs() { Cancelled = true };

            TaskManager.Instance.OnExitTaskEvent(args);
        }

        public static void CancelCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (TaskManager.Instance.NavigationIndex != (int)TaskNavigationIndex.SelectTarget);
        }

        #endregion
    }
}
