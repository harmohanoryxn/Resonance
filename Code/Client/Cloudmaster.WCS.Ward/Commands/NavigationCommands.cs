using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS.Ward.Processing;
using Cloudmaster.WCS.Ward.Model;

namespace Cloudmaster.WCS.Ward.Commands
{
    public class NavigationButtonCommands
    {
        #region Back Button

        public static RoutedUICommand BackCommand = new RoutedUICommand("Back", "Back", typeof(Window));

        public static void BackCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            int navigationIndex = WardModel.Instance.NavigationViewModel.NavigationIndex;

            if (navigationIndex == (int) NavigationIndex.Schedule)
            {

            }
        }
        
        public static void BackCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        #endregion
    }
}
