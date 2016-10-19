using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Cloudmaster.WCS.Classes
{
    public partial class FormInstance : DependencyObject, IOutboxable, IAutoSaveable
    {
        internal FormInstance()
        {
            Sections = new ObservableCollection<Section>();

            TasksCreated = new ObservableCollection<Task>();

            Signature = new RelatedFile();
        }

        public FormInstance(Guid id, FormDefinition definition, Guid entityId, Guid definitionId, string departmentName, string roomName, string entityType, FormCatagory category, string employeeNo)
        {
            Id = id;

            Metadata = new FormMetadata();

            Metadata.Name = definition.Name;
            Metadata.EmployeeNo = employeeNo;
            Metadata.Description = definition.Description;
            Metadata.EntityId = entityId;
            Metadata.FormDefinitionId = definitionId;
            Metadata.Room = roomName;
            Metadata.EntityType = entityType;
            Metadata.CWorksLocationId = departmentName;
            Metadata.FormCategory = (int) category;

            Sections = new ObservableCollection<Section>();

            foreach (Section section in definition.Sections)
            {
                Section instance = new Section(Guid.NewGuid()) { Name = section.Name };

                foreach (Check check in section.Checks)
                {
                    instance.Checks.Add((Check)check.Clone());
                }

                Sections.Add(instance);
            }

            TasksCreated = new ObservableCollection<Task>();

            Signature = new RelatedFile();
        }

        public Guid Id
        {
            get { return (Guid)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(Guid), typeof(FormInstance), new UIPropertyMetadata(null));

        

        public ObservableCollection<Task> TasksCreated
        {
            get { return (ObservableCollection<Task>)GetValue(TasksCreatedProperty); }
            set { SetValue(TasksCreatedProperty, value); }
        }

        public static readonly DependencyProperty TasksCreatedProperty =
            DependencyProperty.Register("TasksCreated", typeof(ObservableCollection<Task>), typeof(FormInstance), new UIPropertyMetadata(null));

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(FormInstance), new UIPropertyMetadata(string.Empty));

        public FormMetadata Metadata
        {
            get { return (FormMetadata)GetValue(MetadataProperty); }
            set { SetValue(MetadataProperty, value); }
        }

        public static readonly DependencyProperty MetadataProperty =
            DependencyProperty.Register("Metadata", typeof(FormMetadata), typeof(FormInstance), new UIPropertyMetadata(null));

        public RelatedFile Signature
        {
            get { return (RelatedFile)GetValue(SignatureProperty); }
            set { SetValue(SignatureProperty, value); }
        }

        public static readonly DependencyProperty SignatureProperty =
            DependencyProperty.Register("Signature", typeof(RelatedFile), typeof(FormInstance), new UIPropertyMetadata(null));

        public string OutboxStatus
        {
            get { return (string)GetValue(OutboxStatusProperty); }
            set { SetValue(OutboxStatusProperty, value); }
        }

        public static readonly DependencyProperty OutboxStatusProperty =
            DependencyProperty.Register("OutboxStatus", typeof(string), typeof(FormInstance), new UIPropertyMetadata(Cloudmaster.WCS.Classes.OutboxStatus.Pending));

        public string CreateUniqueFilename()
        {
            return Id.ToString();
        }

        public DateTime LastSaved
        {
            get { return (DateTime)GetValue(LastSavedProperty); }
            set { SetValue(LastSavedProperty, value); }
        }

        public static readonly DependencyProperty LastSavedProperty =
            DependencyProperty.Register("LastSaved", typeof(DateTime), typeof(FormInstance), new UIPropertyMetadata(null));

        public ObservableCollection<Section> Sections
        {
            get { return (ObservableCollection<Section>)GetValue(SectionsProperty); }
            set { SetValue(SectionsProperty, value); }
        }

        public static readonly DependencyProperty SectionsProperty =
            DependencyProperty.Register("Sections", typeof(ObservableCollection<Section>), typeof(FormInstance), new UIPropertyMetadata(null));
    }
}
