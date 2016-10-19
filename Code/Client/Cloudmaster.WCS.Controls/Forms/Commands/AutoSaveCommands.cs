using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS.Model;

namespace Cloudmaster.WCS.Controls.Forms.Commands
{
    public class AutoSaveCommands
    {
        #region Restore Last Auto Save

        public static RoutedUICommand RestoreLastAutoSaveCommand = new RoutedUICommand("RestoreLastAutoSaveCommand", "RestoreLastAutoSaveCommand", typeof(Window));

        public static void RestoreLastAutoSaveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            FormInstance formInstance = FormManager.Instance.AutoSave.GetMostRecentItem();

            FormManager.Instance.SelectedForm = formInstance;
        }

        public static void RestoreLastAutoSaveCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (FormManager.Instance.AutoSave.HasItems);
        }

        #endregion


    }
}
