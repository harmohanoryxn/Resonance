using System;
using System.Configuration;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.WindowsAPICodePack.ApplicationServices;
using WCS.Core;
using WCS.Core.Composition;
using WCS.Core.Instrumentation;
using WCS.Shared;
using WCS.Shared.Alerts;
using WCS.Shared.Department.Schedule;
using WCS.Shared.Schedule;
using WCS.Shared.Ward.Schedule;

namespace Cloudmaster.WCS.Galway
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

			Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 30 });

			RegisterApplicationRecoveryAndRestart();
		}
		protected override void OnExit(ExitEventArgs e)
		{

			UnregisterApplicationRecoveryAndRestart();
		}

		void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
																	
			//var ex = e.ExceptionObject as Exception;

			//var shellViewModel = Windows[0].Resources["ShellVM"] as ShellViewModel;// Windows[0].TryFindResource("ShellVM") as ShellView;
			//if (shellViewModel == null) 
			//    return;
			////var shellViewModel = shellView.DataContext as ShellViewModel;
			////if (shellViewModel == null) 
			////    return;

			//if (shellViewModel.WcsProductSelector.IsAlpha)
			//{
			//    var mvm = (shellViewModel.WcsProductSelector.Alpha as DepartmentViewModel);
			//    mvm.ErrorBarViewModel.Summary = ex.Message;
			//    mvm.ErrorBarViewModel.HasErrorMessage = true;
			//}
			//if (shellViewModel.WcsProductSelector.IsOmega)
			//{
			//    var mvm = (shellViewModel.WcsProductSelector.Alpha as WardViewModel);
			//    mvm.ErrorBarViewModel.Summary = ex.Message;
			//    mvm.ErrorBarViewModel.HasErrorMessage = true;
			//}

			var ex = e.ExceptionObject as Exception;

			new Logger("App", false).Error(ex.BuildExceptionInfo());

		//	MessageBox.Show(ex.Message, "Unhandled Thread Exception", MessageBoxButton.OK, MessageBoxImage.Error);

		//	Environment.Exit(0);
		//	Environment.FailFast("Unhandled Exception", ex);
		}

		#region ARR

		private void RegisterApplicationRecoveryAndRestart()
		{
			// register for Application Restart
			RestartSettings restartSettings = new RestartSettings("/restart", RestartRestrictions.None);
			ApplicationRestartRecoveryManager.RegisterForApplicationRestart(restartSettings);

			// register for Application Recovery
			RecoverySettings recoverySettings = new RecoverySettings(new RecoveryData(PerformRecovery, null), 5000);
			ApplicationRestartRecoveryManager.RegisterForApplicationRecovery(recoverySettings);
		}

		/// <summary>
		/// Performs recovery by saving the state 
		/// </summary>
		/// <param name="parameter">Unused.</param>
		/// <returns>Unused.</returns>
		private int PerformRecovery(object parameter)
		{
			try
			{
				ApplicationRestartRecoveryManager.ApplicationRecoveryInProgress();
				// Save your work here for recovery

				ApplicationRestartRecoveryManager.ApplicationRecoveryFinished(true);
			}
			catch
			{
				ApplicationRestartRecoveryManager.ApplicationRecoveryFinished(false);
			}

			return 0;
		}

		private void UnregisterApplicationRecoveryAndRestart()
		{
			ApplicationRestartRecoveryManager.UnregisterApplicationRestart();
			ApplicationRestartRecoveryManager.UnregisterApplicationRecovery();
		}
		#endregion
	}
}
