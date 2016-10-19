using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS.Model;

namespace Cloudmaster.WCS.Controls.Forms.Commands
{
    public class CheckCommands
    {
        #region Open Checks

        public static RoutedUICommand OpenCheckCommand = new RoutedUICommand("Open", "OpenCheck", typeof(UserControl));

        public static void OpenCheckCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            FormManager.Instance.NavigationIndex = (int) FormNavigationIndex.Check;
        }

        public static void OpenCheckCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (FormManager.Instance.SelectedCheck != null);
        }

        #endregion

        #region Check NavigationViewModel Buttons

        public static RoutedUICommand NextCheckCommand = new RoutedUICommand("Next", "NextCheck", typeof(UserControl));

        public static void NextCheckCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Section selectedSection = FormManager.Instance.SelectedSection;

            Check selectedCheck = FormManager.Instance.SelectedCheck;

            int indexOfCurrentCheck = selectedSection.Checks.IndexOf(selectedCheck);

            FormManager.Instance.SelectedCheck = selectedSection.Checks[indexOfCurrentCheck + 1];
        }

        public static void NextCheckCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Section selectedSection = FormManager.Instance.SelectedSection;

            Check selectedCheck = FormManager.Instance.SelectedCheck;

            if ((selectedSection != null) && selectedCheck != null)
            {
                int indexOfSelectedCheck = selectedSection.Checks.IndexOf(selectedCheck);

                e.CanExecute = (indexOfSelectedCheck < (selectedSection.Checks.Count - 1));
            }
        }

        public static RoutedUICommand BackCheckCommand = new RoutedUICommand("Back", "BackCheck", typeof(UserControl));

        public static void BackCheckCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Section selectedSection = FormManager.Instance.SelectedSection;

            Check selectedCheck = FormManager.Instance.SelectedCheck;

            int indexOfCurrentCheck = selectedSection.Checks.IndexOf(selectedCheck);

            FormManager.Instance.SelectedCheck = selectedSection.Checks[indexOfCurrentCheck - 1];
        }

        public static void BackCheckCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Section selectedSection = FormManager.Instance.SelectedSection;

            Check selectedCheck = FormManager.Instance.SelectedCheck;

            if ((selectedSection != null) && selectedCheck != null)
            {
                int indexOfSelectedCheck = selectedSection.Checks.IndexOf(selectedCheck);

                e.CanExecute = (indexOfSelectedCheck > 0);
            }
        }

        #endregion
    }
}
