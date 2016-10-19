using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Shared.Controls;

namespace Cloudmaster.WCS.Galway
{
	public partial class ShellView : Window
	{
		public ShellView()
		{ 
			InitializeComponent();

			// set handler for setting input bindings when a new device configuration is available
			var shellViewModel = (Resources["ShellVM"] as ShellViewModel);
			shellViewModel.NewDeviceConfigurationAvailable += ShellView_NewDeviceConfigurationAvailable;

			DataContext = shellViewModel;

		}

		private void ShellView_NewDeviceConfigurationAvailable(List<DeviceConfigurationInstance> configs)
		{
			Dispatcher.BeginInvokeIfRequired((() =>
			{
				// remove all input bindings
				InputBindings.Clear();

				// set default/static input bindings
				InputBindings.Add(new InputBinding((Resources["ShellVM"] as ShellViewModel).ShowTraceLogCommand, new KeyGesture(Key.L, ModifierKeys.Control)));

				// set dynamic input bindings
				configs.ForEach(config =>
									{
										var possibleKey = DetermineShortcutKey(config.ShortcutKey);
										if (possibleKey.HasValue)
										{
											var keyGesture = new KeyGesture(possibleKey.Value, ModifierKeys.Control);
											var inputBinding = new InputBinding((Resources["ShellVM"] as ShellViewModel).ShowNewScreenFromShortcutCommand, keyGesture);
											inputBinding.CommandParameter = config.ShortcutKey;
											InputBindings.Add(inputBinding);
										}
										else
										{
										}
									});
 
			}));
		}

		private static Key? DetermineShortcutKey(int shortcut)
		{
			if (shortcut == 1)
				return Key.D1;
			if (shortcut == 2)
				return Key.D2;
			if (shortcut == 3)
				return Key.D3;
			if (shortcut == 4)
				return Key.D4;
			if (shortcut == 5)
				return Key.D5;
			if (shortcut == 6)
				return Key.D6;
			if (shortcut == 7)
				return Key.D7;
			if (shortcut == 8)
				return Key.D8;
			if (shortcut == 9)
				return Key.D9;

			return null;
		}
	}
}
