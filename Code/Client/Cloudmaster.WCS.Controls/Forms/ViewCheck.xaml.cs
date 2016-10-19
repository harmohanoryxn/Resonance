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
using System.IO;
using Cloudmaster.WCS.Controls.Forms.Commands;

namespace Cloudmaster.WCS.Controls.Forms
{
    /// <summary>
    /// Interaction logic for ViewForms.xaml
    /// </summary>
    public partial class ViewCheck : UserControl
    {
        public ViewCheck()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(CheckCommands.NextCheckCommand, CheckCommands.NextCheckCommandExecuted, CheckCommands.NextCheckCommandCanExecute));
            CommandBindings.Add(new CommandBinding(CheckCommands.BackCheckCommand, CheckCommands.BackCheckCommandExecuted, CheckCommands.BackCheckCommandCanExecute));

            // Images

            CommandBindings.Add(new CommandBinding(ImageOperationCommands.OpenImageCommand, ImageOperationCommands.OpenImageCommandExecuted, ImageOperationCommands.OpenImageCommandCanExecute));
            CommandBindings.Add(new CommandBinding(ImageOperationCommands.CaptureImageCommand, ImageOperationCommands.CaptureImageCommandExecuted, ImageOperationCommands.CaptureImageCommandCanExecute));
            CommandBindings.Add(new CommandBinding(ImageOperationCommands.CreateSketchCommand, ImageOperationCommands.CreateSketchCommandExecuted, ImageOperationCommands.CreateSketchCommandCanExecute));

            

            //CommandBindings.Add(new CommandBinding(NavigationButtonCommands.EditAssetNumberCommand, NavigationButtonCommands.EditAssetNumberCommandExecuted, NavigationButtonCommands.EditAssetNumberCommandCanExecute));

            
        }


        
    }
}
