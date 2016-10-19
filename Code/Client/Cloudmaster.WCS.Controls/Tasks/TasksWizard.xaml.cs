using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cloudmaster.WCS.Controls.Tasks.Commands;

namespace Cloudmaster.WCS.Controls.Tasks
{
    /// <summary>
    /// Interaction logic for TasksWizard.xaml
    /// </summary>
    public partial class TasksWizard : UserControl
    {
        public TasksWizard()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(WizardCommands.NextCommand, WizardCommands.NextCommandExecuted, WizardCommands.NextCommandCanExecute));
            CommandBindings.Add(new CommandBinding(WizardCommands.BackCommand, WizardCommands.BackCommandExecuted, WizardCommands.BackCommandCanExecute));
        }
    }
}
