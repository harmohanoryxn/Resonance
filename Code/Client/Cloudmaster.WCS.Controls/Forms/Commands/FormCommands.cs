using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using Cloudmaster.WCS.Classes;
using System.Windows.Ink;
using Cloudmaster.WCS;
using Cloudmaster.WCS.Model;
using Cloudmaster.WCS.Controls.Windows;
//using Cloudmaster.WCS.Controls.Windows;

namespace Cloudmaster.WCS.Controls.Forms.Commands
{
    public class FormCommands
    {
        #region Create Form

        public static RoutedUICommand CreateFormCommand = new RoutedUICommand("Create Form", "CreateForm", typeof(Window));

        public static void CreateFormCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            /*
            FormDefinition defintion = FormCommands.GetFormDefinitionForSelectedFormSummary();

            FormInstance instance = CreateForm(defintion);

            FormManager.Instance.SelectedForm = instance;

            if (instance.Sections.Count > 0)
            {
                FormManager.Instance.SelectedSection = instance.Sections[0];
            }

            FormManager.Instance.NavigationIndex = (int) FormNavigationIndex.Form;
             * */
        }

        public static void CreateFormCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (FormManager.Instance.SelectedFormSummary != null);
        }

        #endregion

        #region Submit Form

        public static RoutedUICommand SubmitFormCommand = new RoutedUICommand("Submit Form", "SubmitFormCommand", typeof(Window));

        public static void SubmitFormCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            FormManager.Instance.Strokes.SignatureStrokes = new StrokeCollection();

            bool? dialogResult = new SignOffWindow().ShowDialog();

            if (dialogResult == true)
            {
                FormInstance formToSubmit = FormManager.Instance.SelectedForm;

                formToSubmit.Metadata.DateCompleted = DateTime.Now;

                FormManager.Instance.Outbox.Queue.Add(formToSubmit);

                FormManager.Instance.Outbox.Save();

                FormManager.Instance.Outbox.RefreshOutboxLabel();

                OutboxCommands.AttemptToSendForm(formToSubmit);
            }
        }

        public static void SubmitFormCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (FormManager.Instance.SelectedForm != null);
        }

        #endregion

        #region Static Methods

        private static FormDefinition GetFormDefinitionForSelectedFormSummary()
        {
            FormSummary selectedFormSummary = FormManager.Instance.SelectedFormSummary;

            FormDefinition defintion = FormManager.Instance.FormDefinitions.Single(def => def.Id == selectedFormSummary.FormDefinitionId);

            return defintion;
        }

        /*
        public static FormInstance CreateForm(FormDefinition defintion, Floor selectedFloor, Room selectedRoom)
        {

            FormInstance instance = new FormInstance(Guid.NewGuid(), defintion, selectedRoom.Id, defintion.Id, selectedFloor.Name, selectedRoom.Name, selectedRoom.EntityType, (FormCatagory)defintion.FormCategory);

            return instance;
        }*/

        #endregion
    }
}
