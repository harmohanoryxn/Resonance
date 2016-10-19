using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Cloudmaster.WCS.Classes;
using System.Collections.ObjectModel;
using Cloudmaster.WCS.Model.LocalStroage;
using Cloudmaster.WCS.Model.LocalStorage;
using Cloudmaster.WCS.Packaging;

namespace Cloudmaster.WCS.Model
{
    public class FormManager : DependencyObject
    {
        private static FormManager instance;

        public static FormManager Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new InvalidOperationException("Must initialize an instance of the Form Manager.");
                }
                else
                {
                    return instance;
                }
            }
        }

        public static void Initialize()
        {
            instance = new FormManager();

            instance.Strokes = new Strokes();

            Instance.Outbox = new Outbox<FormInstance, FormInstancePackager>(new FormInstancePackager());
            Instance.Outbox.Initialize();

            Instance.AutoSave = new AutoSave<FormInstance, FormInstancePackager>(new FormInstancePackager());

            Instance.AutoSave.Initialize();
        }

        public void StartForm(FormDefinition definition, Guid entityId, string entityType, string areaDisplayName, string roomDisplayName, string employeeNo)
        {
            FormInstance instance = new FormInstance(Guid.NewGuid(), definition, entityId, definition.Id, areaDisplayName, roomDisplayName, entityType, (FormCatagory)definition.FormCategory, employeeNo);

            FormManager.Instance.SelectedForm = instance;

            if (instance.Sections.Count > 0)
            {
                FormManager.Instance.SelectedSection = instance.Sections[0];
            }

            FormManager.Instance.NavigationIndex = (int) FormNavigationIndex.Form;

            FormManager.Instance.InProgress = true;
            FormManager.Instance.Status = string.Format("{0} In Progress", instance.Metadata.Name);
        }

        public bool InProgress
        {
            get { return (bool)GetValue(InProgressProperty); }
            set { SetValue(InProgressProperty, value); }
        }

        public static readonly DependencyProperty InProgressProperty =
            DependencyProperty.Register("InProgress", typeof(bool), typeof(FormManager), new UIPropertyMetadata(false));

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(FormManager), new UIPropertyMetadata(string.Empty));

        #region Private Methods

        public static bool HandleBackNavigation()
        {
            bool result = true;

            int currentIndex = (int) FormManager.Instance.NavigationIndex;

            if (currentIndex == (int) FormNavigationIndex.Check)
            {
                FormManager.Instance.AutoSave.Save(FormManager.Instance.SelectedForm);

                FormManager.Instance.NavigationIndex = (int )FormNavigationIndex.Form;
            }
            else if (currentIndex == (int)FormNavigationIndex.Image)
            {
                try
                {
                    ImageManager.SaveImage();
                }
                catch (Exception) { }

                FormManager.Instance.SelectedImage = null;

                FormManager.Instance.NavigationIndex = (int)FormNavigationIndex.Check;
            }
            else
            {
                result = false;
            }

            return result;
        }

        #endregion

        public int NavigationIndex
        {
            get { return (int)GetValue(NavigationIndexProperty); }
            set { SetValue(NavigationIndexProperty, value); }
        }

        public static readonly DependencyProperty NavigationIndexProperty =
            DependencyProperty.Register("NavigationIndex", typeof(int), typeof(FormManager), new UIPropertyMetadata(0));

        public RelatedFile SelectedImage
        {
            get { return (RelatedFile)GetValue(SelectedImageProperty); }
            set { SetValue(SelectedImageProperty, value); }
        }

        public static readonly DependencyProperty SelectedImageProperty =
            DependencyProperty.Register("SelectedImage", typeof(RelatedFile), typeof(FormManager), new UIPropertyMetadata(null));

        public FormInstance SelectedForm
        {
            get { return (FormInstance)GetValue(SelectedFormProperty); }
            set { SetValue(SelectedFormProperty, value); }
        }

        public static readonly DependencyProperty SelectedFormProperty =
            DependencyProperty.Register("SelectedForm", typeof(FormInstance), typeof(FormManager), new UIPropertyMetadata(null));

        public FormSummary SelectedFormSummary
        {
            get { return (FormSummary)GetValue(SelectedFormSummaryProperty); }
            set { SetValue(SelectedFormSummaryProperty, value); }
        }

        public static readonly DependencyProperty SelectedFormSummaryProperty =
            DependencyProperty.Register("SelectedFormSummary", typeof(FormSummary), typeof(FormManager), new UIPropertyMetadata(null));

        public Check SelectedCheck
        {
            get { return (Check)GetValue(SelectedCheckProperty); }
            set { SetValue(SelectedCheckProperty, value); }
        }

        public static readonly DependencyProperty SelectedCheckProperty =
            DependencyProperty.Register("SelectedCheck", typeof(Check), typeof(FormManager), new UIPropertyMetadata(null));

        public Section SelectedSection
        {
            get { return (Section)GetValue(SelectedSectionProperty); }
            set { SetValue(SelectedSectionProperty, value); }
        }

        public static readonly DependencyProperty SelectedSectionProperty =
            DependencyProperty.Register("SelectedSection", typeof(Section), typeof(FormManager), new UIPropertyMetadata(null));

        public Strokes Strokes
        {
            get { return (Strokes)GetValue(StrokesProperty); }
            set { SetValue(StrokesProperty, value); }
        }

        public static readonly DependencyProperty StrokesProperty =
            DependencyProperty.Register("Strokes", typeof(Strokes), typeof(FormManager), new UIPropertyMetadata(null));

        public ObservableCollection<FormDefinition> FormDefinitions
        {
            get { return (ObservableCollection<FormDefinition>)GetValue(FormDefinitionsProperty); }
            set { SetValue(FormDefinitionsProperty, value); }
        }

        public static readonly DependencyProperty FormDefinitionsProperty =
            DependencyProperty.Register("FormDefinitions", typeof(ObservableCollection<FormDefinition>), typeof(FormManager), new UIPropertyMetadata(null));

        public Outbox<FormInstance, FormInstancePackager> Outbox
        {
            get { return (Outbox<FormInstance, FormInstancePackager>)GetValue(OutboxProperty); }
            set { SetValue(OutboxProperty, value); }
        }

        public static readonly DependencyProperty OutboxProperty =
            DependencyProperty.Register("Outbox", typeof(Outbox<FormInstance, FormInstancePackager>), typeof(FormManager), new UIPropertyMetadata(null));

        public AutoSave<FormInstance, FormInstancePackager> AutoSave
        {
            get { return (AutoSave<FormInstance, FormInstancePackager>)GetValue(AutoSaveProperty); }
            set { SetValue(AutoSaveProperty, value); }
        }

        public static readonly DependencyProperty AutoSaveProperty =
            DependencyProperty.Register("AutoSave", typeof(AutoSave<FormInstance, FormInstancePackager>), typeof(FormManager), new UIPropertyMetadata(null));
    }
}
