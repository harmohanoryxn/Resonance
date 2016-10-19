using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using Cloudmaster.WCS.Ward.Model;

namespace Cloudmaster.WCS.Ward.Commands
{
    public class SecurityCommands
    {
        #region Lock Now Command

        public static RoutedUICommand LockNowCommand = new RoutedUICommand("Lock Now", "LockNowCommand", typeof(Window));

        public static void LockNowCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            LockNow();
        }

        public static void LockNowCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

        #region Lock Now

        public static void LockNow()
        {
            WardModel.Instance.SecurityViewModel.IsLocked = true;
        }

        #endregion
    }
}
