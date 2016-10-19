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
using System.Diagnostics;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS.Controls.Forms.Commands;

namespace Cloudmaster.WCS.Controls.Forms
{
    /// <summary>
    /// Interaction logic for ViewForms.xaml
    /// </summary>
    public partial class ViewImage : UserControl
    {
        public ViewImage()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(ImageOperationCommands.SaveImageCommand, ImageOperationCommands.SaveImageCommandExecuted, ImageOperationCommands.SaveImageCommandCanExecute));
            CommandBindings.Add(new CommandBinding(ImageOperationCommands.RemoveImageCommand, ImageOperationCommands.RemoveImageCommandExecuted, ImageOperationCommands.RemoveImageCommandCanExecute));
            CommandBindings.Add(new CommandBinding(ImageOperationCommands.UndoImageCommand, ImageOperationCommands.UndoImageCommandExecuted, ImageOperationCommands.UndoImageCommandCanExecute));
        }
    }
}
