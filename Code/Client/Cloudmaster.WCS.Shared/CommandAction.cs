﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

// lifted shamelessly from  http://sachabarber.net/?p=510

namespace Cloudmaster.WCS.Controls
{
    /// <summary>
    /// The CommandAction allows the user to route a FrameworkElement's 
    /// routed event to a Command.
    /// For instance this makes it possible to specify--in Xaml--that 
    /// right-clicking on a Border element should execute the Application.Close 
    /// command (this example may not make much sense, but it does illustrate 
    /// what's possible).
    /// 
    /// CommandParameter and CommandTarget properties are provided for 
    /// consistency with the Wpf Command pattern.
    /// 
    /// The action's IsEnabled property will be updated according to the 
    /// Command's CanExecute value.
    /// 
    /// In addition a SyncOwnerIsEnabled property allows the user to specify 
    /// that the owner element should be enabled/disabled whenever the action 
    /// is enabled/disabled.
    /// </summary>
    public class CommandAction :
        TargetedTriggerAction<FrameworkElement>,
        ICommandSource
    {
        #region DPs

        #region Command DP
        /// <summary>
        /// The actual Command to fire when the 
        /// EventTrigger occurs, thus firing this 
        /// CommandAction
        /// </summary>
        [Category("Command Properties")]
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                "Command", typeof(ICommand), typeof(CommandAction),
                    new PropertyMetadata(
                        (ICommand)null, OnCommandChanged));

        private static void OnCommandChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var action = (CommandAction)d;
            action.OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        #region Command implementation

        /// <summary>
        /// This is a strong reference to the Command.CanExecuteChanged event handler. 
        /// The commanding system uses a weak reference and if we don't enforce a 
        /// strong reference then the event handler will be gc'ed.
        /// </summary>
        private EventHandler CanExecuteChangedHandler;



        private void OnCommandChanged(ICommand oldCommand, ICommand newCommand)
        {
            if (oldCommand != null)
                UnhookCommand(oldCommand);
            if (newCommand != null)
                HookCommand(newCommand);
        }

        private void UnhookCommand(ICommand command)
        {
            command.CanExecuteChanged -= CanExecuteChangedHandler;
            UpdateCanExecute();
        }

        private void HookCommand(ICommand command)
        {
            // Save a strong reference to the Command.CanExecuteChanged event handler. 
            // The commanding system uses a weak reference and if we don't save a strong 
            // reference then the event handler will be gc'ed.
            CanExecuteChangedHandler = new EventHandler(OnCanExecuteChanged);
            command.CanExecuteChanged += CanExecuteChangedHandler;
            UpdateCanExecute();
        }

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            UpdateCanExecute();
        }

        private void UpdateCanExecute()
        {
            if (Command != null)
            {
                RoutedCommand command = Command as RoutedCommand;
                if (command != null)
                    IsEnabled = command.CanExecute(CommandParameter, CommandTarget);
                else
                    IsEnabled = Command.CanExecute(CommandParameter);
                if (Target != null && SyncOwnerIsEnabled)
                    Target.IsEnabled = IsEnabled;
            }
        }

        #endregion


        #endregion

        #region CommandParameter DP
        /// <summary>
        /// For consistency with the Wpf Command pattern
        /// </summary>
        [Category("Command Properties")]
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                "CommandParameter", typeof(object), typeof(CommandAction),
                    new PropertyMetadata());
        #endregion

        #region CommandTarget DP
        /// <summary>
        /// For consistency with the Wpf Command pattern
        /// </summary>
        [Category("Command Properties")]
        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register(
                "CommandTarget", typeof(IInputElement), typeof(CommandAction),
                    new PropertyMetadata());
        #endregion

        #region SyncOwnerIsEnabled DP
        /// <summary>
        /// Allows the user to specify that the owner element should be 
        /// enabled/disabled whenever the action is enabled/disabled.
        /// </summary>
        [Category("Command Properties")]
        public bool SyncOwnerIsEnabled
        {
            get { return (bool)GetValue(SyncOwnerIsEnabledProperty); }
            set { SetValue(SyncOwnerIsEnabledProperty, value); }
        }

        /// <summary>
        /// When SyncOwnerIsEnabled is true then changing CommandAction.IsEnabled 
        /// will automatically update the owner (Target) IsEnabled property.
        /// </summary>
        public static readonly DependencyProperty SyncOwnerIsEnabledProperty =
            DependencyProperty.Register(
                "SyncOwnerIsEnabled", typeof(bool), typeof(CommandAction),
                    new PropertyMetadata());
        #endregion

        #endregion

        #region overrides
        /// <summary>
        /// Invoke is called when the EventTrigger associated with this
        /// TargetedTriggerAction occurs. So we can obtain the associated 
        /// ICommand and simply execute it
        /// </summary>
        protected override void Invoke(object o)
        {
            if (Command != null)
            {
                var command = Command as RoutedCommand;
                if (command != null)
                    command.Execute(CommandParameter, CommandTarget);
                else
                    Command.Execute(CommandParameter);
            }
        }
        #endregion
    }
}
