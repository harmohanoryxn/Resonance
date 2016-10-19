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
using System.Windows.Shapes;
using CatenaLogic.Windows.Presentation.WebcamPlayer;
using System.IO;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS.Controls.Forms.Commands;
using IdentityMine.Windows.Essentials;

namespace Cloudmaster.WCS.Controls.Windows
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SignOffWindow : Window
    {
        public static SignOffWindow Instance
        {
            get
            {
                // if null, create and initialize the one and only window instance
                if (_instance == null)
                {
                    _instance = new SignOffWindow();

                    _instance.InitializeComponent();
                }
                return _instance;
            }
        }

        public SignOffWindow()
        {
            InitializeComponent();
        }

        private void InitializeCommands()
        {
            CommandBindings.Add(new CommandBinding(CloseWindow, CloseWindowExecuted));
            CommandBindings.Add(new CommandBinding(ToggleMinimizedState, MinimizeWindowExecuted));
            CommandBindings.Add(new CommandBinding(ToggleMaximizedState, MaximizeWindowExecuted));

            CommandBindings.Add(new CommandBinding(ImageOperationCommands.SignFormCommand, ImageOperationCommands.SignFormCommandExecuted, ImageOperationCommands.SignFormCommandCanExecute));
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            InitializeCommands();
        }

        #region Window Commands

        #region CloseWindow

        public static RoutedUICommand CloseWindow = new RoutedUICommand("Close", "CloseWindow", typeof(ChromelessWindow));
        
        private void CloseWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region ToggleMaximizedState

        public static RoutedUICommand ToggleMaximizedState = new RoutedUICommand("Maximize", "ToggleMaximizedState", typeof(ChromelessWindow));
        
        private void MaximizeWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SignOffWindow.Instance.WindowState = (SignOffWindow.Instance.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
        }

        #endregion

        #region ToggleMinimizedState

        public static RoutedUICommand ToggleMinimizedState = new RoutedUICommand("Minimize", "ToggleMinimizedState", typeof(ChromelessWindow));
        private void MinimizeWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SignOffWindow.Instance.WindowState = (SignOffWindow.Instance.WindowState == WindowState.Minimized) ? WindowState.Normal : WindowState.Minimized;
        }

        #endregion

        #endregion

        private static SignOffWindow _instance = null;

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;

            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
