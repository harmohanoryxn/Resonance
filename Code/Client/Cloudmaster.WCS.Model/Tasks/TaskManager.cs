using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Cloudmaster.WCS.Model.Tasks
{
    public class TaskManager: DependencyObject
    {
        private static TaskManager instance;

        public static TaskManager Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new InvalidOperationException("Must initialize an instance of the task manager.");
                }
                else
                {
                    return instance;
                }
            }
        }

        public event ExitTaskEventHandler ExitTaskEvent;

        public delegate void ExitTaskEventHandler(object sender, TaskWizardCompletedEventArgs e);

        public void OnExitTaskEvent(TaskWizardCompletedEventArgs e)
        {
            ExitTaskEvent(this, e);
        }

        public static void Initialize()
        {
            instance = new TaskManager();
        }

        public bool IsWizardActive
        {
            get { return (bool)GetValue(IsWizardActiveProperty); }
            set { SetValue(IsWizardActiveProperty, value); }
        }

        public static readonly DependencyProperty IsWizardActiveProperty =
            DependencyProperty.Register("IsWizardActive", typeof(bool), typeof(TaskManager), new UIPropertyMetadata(false));

        public string Target
        {
            get { return (string)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(string), typeof(TaskManager), new UIPropertyMetadata(string.Empty));

        public int NavigationIndex
        {
            get { return (int)GetValue(NavigationIndexProperty); }
            set { SetValue(NavigationIndexProperty, value); }
        }

        public static readonly DependencyProperty NavigationIndexProperty =
            DependencyProperty.Register("NavigationIndex", typeof(int), typeof(TaskManager), new UIPropertyMetadata(0));
    }
}
