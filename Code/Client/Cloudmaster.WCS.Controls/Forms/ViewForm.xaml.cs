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
using System.Collections;
using System.Windows.Media.Animation;
using Cloudmaster.WCS.Controls.Forms.Commands;

namespace Cloudmaster.WCS.Controls.Forms
{
    /// <summary>
    /// Interaction logic for ViewOperation.xaml
    /// </summary>
    public partial class ViewForm : UserControl
    {
        public ViewForm()
        {
            InitializeComponent();

            // Office

            CommandBindings.Add(new CommandBinding(OfficeCommands.ExportExcelCommand, OfficeCommands.ExportExcelCommandExecuted, OfficeCommands.ExportExcelCommandCanExecute));
            CommandBindings.Add(new CommandBinding(OfficeCommands.ExportWordCommand, OfficeCommands.ExportWordCommandExecuted, OfficeCommands.ExportWordCommandCanExecute));

            // Form 

            CommandBindings.Add(new CommandBinding(FormCommands.SubmitFormCommand, FormCommands.SubmitFormCommandExecuted, FormCommands.SubmitFormCommandCanExecute));


            // Checks

            CommandBindings.Add(new CommandBinding(CheckCommands.OpenCheckCommand, CheckCommands.OpenCheckCommandExecuted, CheckCommands.OpenCheckCommandCanExecute));

            CommandBindings.Add(new CommandBinding(SectionCommands.NextSectionCommand, SectionCommands.NextSectionCommandExecuted, SectionCommands.NextSectionCommandCanExecute));
            CommandBindings.Add(new CommandBinding(SectionCommands.BackSectionCommand, SectionCommands.BackSectionCommandExecuted, SectionCommands.BackSectionCommandCanExecute));
        }
    }
}
