using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WCS.Core;
using WCS.Core.Composition;
using WCS.Core.Instrumentation;
using WCS.Shared;
using WCS.Shared.Admissions.Schedule;
using WCS.Shared.Cleaning.Schedule;
using WCS.Shared.Controls;
using WCS.Shared.Department.Schedule;
using WCS.Shared.Discharge.Schedule;
using WCS.Shared.Physio.Schedule;
using WCS.Shared.Schedule;
using WCS.Shared.Ward.Schedule;

namespace Cloudmaster.WCS.Galway
{
	/// <summary>
	/// This is the topmost container. it 'contains' either the Department or the Ward or the Cleaning
	/// </summary>
	public sealed class ShellViewModel : ViewModelBase
	{
		public event Action<List<DeviceConfigurationInstance>> NewDeviceConfigurationAvailable;

		private WcsMef _wcsMef;
		private ObservableCollection<LogMessage> _logMessages;
		private ConfigurationRecurringPoller _configPoller;
		private WcsViewModel _screen;

		private IDictionary<int, DeviceConfigurationInstance> _configs;

		public ShellViewModel()
		{
			_wcsMef = new WcsMef(); //initialise the MEF container

			Screen = new WcsViewModel(false, DefaultDeviceIdentity.AppVersion, new DefaultDeviceIdentity().DeviceName, DefaultDeviceIdentity.ServerName);

			LogMessages = new ObservableCollection<LogMessage>();

			var logger = WcsMef.Container.GetExportedValue<IWcsClientLogger>();
			if (logger != null)
			{
				logger.MessageArrived += HandleNewLogMessage;
				logger.CallToServerDelta += HandleNumberOfCallsOpenToServerChanged;
			}

			GetConfiguration();
		}

		public WcsViewModel Screen
		{
			get { return _screen; }
			set
			{
				_screen = value;
				this.DoRaisePropertyChanged(() => Screen, RaisePropertyChanged);
			}
		}

		public RelayCommand ShowTraceLogCommand
		{
			get { return new RelayCommand(DoShowTraceLog); }
		}

		public RelayCommand<int> ShowNewScreenFromShortcutCommand
		{
			get { return new RelayCommand<int>(DoShowNewScreenFromShortcut); }
		}

		public ObservableCollection<LogMessage> LogMessages
		{
			get { return _logMessages; }
			set
			{
				_logMessages = value;
				this.DoRaisePropertyChanged(() => LogMessages, RaisePropertyChanged);
			}
		}

		private void GetConfiguration()
		{
			var invoker = WcsMef.Container.GetExportedValue<IWcsAsyncInvoker>();
			if (invoker != null)
			{
				// get configuration from server
				invoker.GetConfigurationsAsync(new DefaultDeviceIdentity().DeviceName, RecieveDeviceConfiguration);

				// subscribe to changes in configuration
				_configPoller = new ConfigurationRecurringPoller(invoker, 1, 60);
				var configSource = Observable.FromEvent<PollingTimeouts>(ev => _configPoller.PollingTimeoutUpdateAvailable += ev, ev => _configPoller.PollingTimeoutUpdateAvailable -= ev);
				configSource.Subscribe(HandleNewPollingTimeouts);
			}
		}

		private void DoShowNewScreenFromShortcut(int shortcut)
		{
			_configPoller.SetShortcut(shortcut);

			if (_configs != null)
			{
				if (_configs.Keys.Contains(shortcut))
				{
					var config = _configs[shortcut];

					if (!config.IsValid())
					{
						ProcessInvalidConfiguration(config);
						return;
					}
					 
					switch (config.Type)
					{
						case DeviceConfigurationType.Department:
							ProcessDepartment(config);
							return;
						case DeviceConfigurationType.Ward:
							ProcessWard(config);
							return;
						case DeviceConfigurationType.Cleaning:
							ProcessCleaning(config);
							return;
						case DeviceConfigurationType.Physio:
							ProcessPhysio(config);
							return;
						case DeviceConfigurationType.Discharge:
							ProcessDischarge(config);
							return;
						case DeviceConfigurationType.Admissions:
							ProcessAdmissions(config);
							return;
					}
				}
				else
				{
				}
			}
			else
			{
			}
		}

		private void DoShowTraceLog()
		{
			try
			{
				dynamic c = Screen;
				c.PopupDiagnostics(LogMessages);
			}
			catch (Exception e)
			{
				HandleNewLogMessage(new LogMessage(DateTime.Now, string.Format("Error Opening Trace Log: {0}", e.Message)));
			}
		}

		/// <summary>
		/// Receives the device configuration from the server
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		public void RecieveDeviceConfiguration(DeviceConfiguration configuration)
		{
			if (configuration == null)
			{
				ProcessInvalidConfiguration();
				return;
			}

			// Hide all popups that might be on the screen
			Application.Current.Dispatcher.InvokeIfRequired((() =>
																{
																	if (Screen != null)
																		Screen.Tombstone();
																}));

			WcsViewModel.MefContainer = new CompositionContainer(new TypeCatalog(new[] { typeof(WcsExceptionHandler), typeof(WcsAsyncInvoker), typeof(WcsClientLogger) }), true);
			WcsViewModel.MefContainer.ComposeExportedValue<IDeviceIdentity>("IDeviceIdentity", configuration);
			WcsViewModel.MefContainer.ComposeParts(this);

			var logger = WcsViewModel.MefContainer.GetExportedValue<IWcsClientLogger>();
			if (logger != null)
			{
				logger.MessageArrived += HandleNewLogMessage;
				logger.CallToServerDelta += HandleNumberOfCallsOpenToServerChanged;
			}

			_configs = configuration.Instances.OrderBy(c => c.ShortcutKey).ToDictionary(c => c.ShortcutKey, c => c);
			SetConfigurationsInContent(_configs.Values.ToList());

			// set screen shortcut bindings
			if (configuration.Instances.Count() > 1)
			{
				var ndca = NewDeviceConfigurationAvailable;
				if (ndca != null)
					ndca(configuration.Instances.ToList());
			}

			// load the default config
			var defaultConfig = _configs.FirstOrDefault();
			if (defaultConfig.Value != null)
			{

				if (!defaultConfig.Value.IsValid())
				{
					ProcessInvalidConfiguration(defaultConfig.Value);
					return;
				}

				switch (defaultConfig.Value.Type)
				{
					case DeviceConfigurationType.Department:
						ProcessDepartment(defaultConfig.Value);
						return;
					case DeviceConfigurationType.Ward:
						ProcessWard(defaultConfig.Value);
						return;
					case DeviceConfigurationType.Cleaning:
						ProcessCleaning(defaultConfig.Value);
						return;
					case DeviceConfigurationType.Physio:
						ProcessPhysio(defaultConfig.Value);
						return;
					case DeviceConfigurationType.Discharge:
						ProcessDischarge(defaultConfig.Value);
						return;
					case DeviceConfigurationType.Admissions:
						ProcessAdmissions(defaultConfig.Value);
						return;
				}
			}
			else
			{
				// if it gets this far then there are no configurations that match so show the default error
				ProcessNoConfiguration();
			}
		}

		private void ProcessCleaning(DeviceConfigurationInstance configurationInstance)
		{
			var cleaningViewModel = new CleaningViewModel(configurationInstance.RequiresLoggingOn, DefaultDeviceIdentity.AppVersion, Environment.MachineName, DefaultDeviceIdentity.ServerName);

			cleaningViewModel.InitialiseConfiguration(configurationInstance, configurationInstance.PollingTimeouts);
			Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
			                                                      	{
																		Screen = cleaningViewModel;
																		SetConfigurationsInContent(_configs.Values.ToList());
																		Screen.ForceNavigationBarDisappear();
																	}));
		}

		private void ProcessWard(DeviceConfigurationInstance configurationInstance)
		{
			var wardViewModel = new WardViewModel(configurationInstance.RequiresLoggingOn, DefaultDeviceIdentity.AppVersion, new DefaultDeviceIdentity().DeviceName, DefaultDeviceIdentity.ServerName);

			wardViewModel.InitialiseConfiguration(configurationInstance, configurationInstance.PollingTimeouts);
			Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
			                                                      	{
																		Screen = wardViewModel;
																		SetConfigurationsInContent(_configs.Values.ToList());
																		Screen.ForceNavigationBarDisappear();
																	}));
		}

		private void ProcessDepartment(DeviceConfigurationInstance configurationInstance)
		{
			var departmentViewModel = new DepartmentViewModel(configurationInstance.RequiresLoggingOn, DefaultDeviceIdentity.AppVersion, new DefaultDeviceIdentity().DeviceName, DefaultDeviceIdentity.ServerName);

			departmentViewModel.InitialiseConfiguration(configurationInstance, configurationInstance.PollingTimeouts);
			Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
			                                                      	{
																		Screen = departmentViewModel;
																		SetConfigurationsInContent(_configs.Values.ToList());
																		Screen.ForceNavigationBarDisappear();
			                                                      	}));
		}

		private void ProcessPhysio(DeviceConfigurationInstance configurationInstance)
		{
			var physioViewModel = new PhysioViewModel(configurationInstance.RequiresLoggingOn, DefaultDeviceIdentity.AppVersion, new DefaultDeviceIdentity().DeviceName, DefaultDeviceIdentity.ServerName);

			physioViewModel.InitialiseConfiguration(configurationInstance, configurationInstance.PollingTimeouts);
			Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
			{
				Screen = physioViewModel;
				SetConfigurationsInContent(_configs.Values.ToList());
				Screen.ForceNavigationBarDisappear();
			}));
		}

		private void ProcessDischarge(DeviceConfigurationInstance configurationInstance)
		{
			var dischargeViewModel = new DischargeViewModel(configurationInstance.RequiresLoggingOn, DefaultDeviceIdentity.AppVersion, new DefaultDeviceIdentity().DeviceName, DefaultDeviceIdentity.ServerName);

			dischargeViewModel.InitialiseConfiguration(configurationInstance, configurationInstance.PollingTimeouts);
			Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
			{
				Screen = dischargeViewModel;
				SetConfigurationsInContent(_configs.Values.ToList());
				Screen.ForceNavigationBarDisappear();
			}));
		}

		private void ProcessAdmissions(DeviceConfigurationInstance configurationInstance)
		{
			var admissionsViewModel = new AdmissionsViewModel(configurationInstance.RequiresLoggingOn, DefaultDeviceIdentity.AppVersion, new DefaultDeviceIdentity().DeviceName, DefaultDeviceIdentity.ServerName);

			admissionsViewModel.InitialiseConfiguration(configurationInstance, configurationInstance.PollingTimeouts);
			Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
			{
				Screen = admissionsViewModel;
				SetConfigurationsInContent(_configs.Values.ToList());
				Screen.ForceNavigationBarDisappear();
			}));
		}


		private void ProcessNoConfiguration()
		{
			var details = new ErrorDetails
			{
				Message = string.Format("No configuration available for {0}", Environment.MachineName),
				CanRetry = true,
				RetryDelegate = GetConfiguration
			};

			Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
			{
				try
				{
					dynamic c = Screen;
					c.ErrorBarViewModel.SetError(details);
				}
				catch (Exception)
				{
					new Logger("SVM", false).Error("Unable to locate Screen control");
				}
			}));
		}


		private void ProcessInvalidConfiguration(DeviceConfigurationInstance config)
		{
			var details = new ErrorDetails
			{
				Message = !string.IsNullOrEmpty(config.LocationName) ? string.Format("The configuration for {0} is invalid on {1}", config.LocationName, Environment.MachineName) : string.Format("The default configuration is invalid for {0}", Environment.MachineName),
				CanRetry = true,
				RetryDelegate = GetConfiguration
			};

			Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
			{
				try
				{
					dynamic c = Screen;
					c.ErrorBarViewModel.SetError(details);
				}
				catch (Exception)
				{
					new Logger("SVM", false).Error("Unable to locate Screen control");
				}
			}));
		}
		private void ProcessInvalidConfiguration()
		{
			var details = new ErrorDetails
			{
				Message = string.Format("There is no configuration for {0}", Environment.MachineName),
				CanRetry = true,
				RetryDelegate = GetConfiguration
			};

			Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
			{
				try
				{
					dynamic c = Screen;
					c.ErrorBarViewModel.SetError(details);
				}
				catch (Exception)
				{
					new Logger("SVM", false).Error("Unable to locate Screen control");
				}
			}));
		}
		/// <summary>
		/// Handles the event that there is a new log message somewhere that needs to be tracked
		/// </summary>
		/// <param name="message">The message.</param>
		private void HandleNewLogMessage(LogMessage message)
		{
			lock ((_logMessages as ICollection).SyncRoot)
			{
				Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
				{
					try
					{
						LogMessages.Insert(0, message);
						while (LogMessages.Count > 1000)
						{
							LogMessages.RemoveAt(1000);
						}
					}
					catch (Exception)
					{
						new Logger("SVM", false).Error("Unable to locate Log message to trace screen");
					}
				}));
			}
		}


		/// <summary>
		/// Handles the event the Server-Access-Layer raises when a call is made and returned from the server. 1 outgoing call will result in a +1 parameter
		/// and a returned call will result in a -1 parameter. When there are no call there should be a balance of 0
		/// </summary>
		/// <param name="change">The change in the amount of calls</param>
		private void HandleNumberOfCallsOpenToServerChanged(int change)
		{
			if (change == 1)
				Interlocked.Increment(ref _serverCallBalance);
			else if (change == -1)
				Interlocked.Decrement(ref _serverCallBalance);

			this.DoRaisePropertyChanged(() => ShowCallToServerInProgress, RaisePropertyChanged);
		}

		private int _serverCallBalance;
		public bool ShowCallToServerInProgress
		{
			get { return _serverCallBalance > 0; }
		}

		/// <summary>
		/// Handles the event when the processor has new timeouts available for UI consumption
		/// </summary>
		/// <param name="timeouts">The timeouts</param>
		private void HandleNewPollingTimeouts(PollingTimeouts timeouts)
		{
			WcsViewModel wcsViewModel = null;
			Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
																	{
																		wcsViewModel = (Screen as WcsViewModel);
																	}));
			if (wcsViewModel != null)
				wcsViewModel.SetPollingTimeouts(timeouts);

		}

		private void SetConfigurationsInContent(List<DeviceConfigurationInstance> configurations)
		{
			Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
			{
				try
				{
					dynamic c = Screen;
					c.SetConfigurations(configurations);
					c.SetConfigurationCallback = (Action<int>)DoShowNewScreenFromShortcut;

				}
				catch (Exception)
				{
					new Logger("SVM", false).Error("Unable to locate Screen control");
				}
			}));
		}
 
	}
}
