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
    public class SectionCommands
    {
        #region Section NavigationViewModel Buttons

        public static RoutedUICommand NextSectionCommand = new RoutedUICommand("Next", "NextSection", typeof(UserControl));

        public static void NextSectionCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            FormInstance selectedForm = FormManager.Instance.SelectedForm;

            Section selectedSection = FormManager.Instance.SelectedSection;

            int indexOfCurrentSection = selectedForm.Sections.IndexOf(selectedSection);

            FormManager.Instance.SelectedSection = selectedForm.Sections[indexOfCurrentSection + 1];
        }

        public static void NextSectionCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            FormInstance selectedForm = FormManager.Instance.SelectedForm;

            Section selectedSection = FormManager.Instance.SelectedSection;

            if ((selectedForm != null) && selectedSection != null)
            {
                int indexOfSelectedSection = selectedForm.Sections.IndexOf(selectedSection);

                e.CanExecute = (indexOfSelectedSection < (selectedForm.Sections.Count - 1));
            }
        }

        public static RoutedUICommand BackSectionCommand = new RoutedUICommand("Back", "BackSection", typeof(UserControl));

        public static void BackSectionCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            FormInstance selectedForm = FormManager.Instance.SelectedForm;

            Section selectedSection = FormManager.Instance.SelectedSection;

            int indexOfCurrentSection = selectedForm.Sections.IndexOf(selectedSection);

            FormManager.Instance.SelectedSection = selectedForm.Sections[indexOfCurrentSection - 1];
        }

        public static void BackSectionCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            FormInstance selectedForm = FormManager.Instance.SelectedForm;

            Section selectedSection = FormManager.Instance.SelectedSection;

            if ((selectedForm != null) && selectedSection != null)
            {
                int indexOfSelectedSection = selectedForm.Sections.IndexOf(selectedSection);

                e.CanExecute = (indexOfSelectedSection > 0);
            }
        }

        #endregion
    }
}
