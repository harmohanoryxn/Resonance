using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Windows;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WCS.Core;
using WCS.Core.Composition;
using WCS.Shared;
using WCS.Shared.Controls;

namespace Cloudmaster.WCS.Drone
{
	public class DroneViewModel : ViewModelBase, IDeviceIdentity
	{
		private IWcsAsyncInvoker _invoker;

		public CompositionContainer MefContainer { get; set; }

		public DroneViewModel(string deviceName, Screen screen)
		{
			DeviceName = deviceName;
            ApplicationVersion = DefaultDeviceIdentity.AppVersion;
			Screen = screen;

			Activities = new ObservableCollection<string>();

			MefContainer = new CompositionContainer(new TypeCatalog(new[] { typeof(WcsExceptionHandler), typeof(WcsAsyncInvoker), typeof(WcsClientLogger) }),true);
			MefContainer.ComposeExportedValue<IDeviceIdentity>("IDeviceIdentity", this);
			MefContainer.ComposeParts(this);

			_invoker = MefContainer.GetExportedValue<IWcsAsyncInvoker>();
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				this.DoRaisePropertyChanged(() => Name, RaisePropertyChanged);
			}
		}

		public string DeviceName { get; set; }
		public string ApplicationVersion { get; set; }
		Screen Screen { get; set; }

		private BaseDroneEngineViewModel _product;
		public BaseDroneEngineViewModel Product
		{
			get { return _product; }
			set
			{
				_product = value;
				this.DoRaisePropertyChanged(() => Product, RaisePropertyChanged);
				this.DoRaisePropertyChanged(() => IsOn, RaisePropertyChanged);
			}
		}

		public bool IsOn { get { return Product != null; } }

		public ObservableCollection<string> Activities { get; set; }


		public RelayCommand ToggleOnOffCommand
		{
			get { return new RelayCommand(ToggleOnOff); }
		}

		public RelayCommand StartCommand
		{
			get { return new RelayCommand(DoStart); }
		}


		public RelayCommand StopCommand
		{
			get { return new RelayCommand(DoStop); }
		}

		private void ToggleOnOff()
		{
			if (!IsOn)
				DoStart();
			else
				DoStop();

		}

		private void DoStart()
		{
			if (_invoker == null) return;
			if (Screen == Screen.Department)
				_invoker.GetConfigurationsAsync(DeviceName, RecieveDeviceConfiguration);
			else if (Screen == Screen.Ward)
				_invoker.GetConfigurationsAsync(DeviceName, RecieveDeviceConfiguration);
			else if (Screen == Screen.Cleaning)
				_invoker.GetConfigurationsAsync(DeviceName, RecieveDeviceConfiguration);
		}

		private void DoStop()
		{
			Product.ReportActivity -= HandleReportActivity;
			Product.Dispose();
			Product = null;
		}

		public void RecieveDeviceConfiguration(DeviceConfiguration configuration)
		{
				var defaultConfig = configuration.Instances.FirstOrDefault();
				if (defaultConfig != null)
				{
					switch (defaultConfig.Type)
					{
						case DeviceConfigurationType.Department:
							var departmentDrone = new DepartmentDroneEngineViewModel(_invoker, defaultConfig, defaultConfig.PollingTimeouts, DeviceName);
							departmentDrone.ReportActivity += HandleReportActivity;
							Application.Current.Dispatcher.InvokeIfRequired((() =>
							{
								Product = departmentDrone;
							}));
							return;

						case DeviceConfigurationType.Ward:
							var wardDrone = new WardDroneEngineViewModel(_invoker, defaultConfig, defaultConfig.PollingTimeouts, DeviceName);
							wardDrone.ReportActivity += HandleReportActivity;
							Application.Current.Dispatcher.InvokeIfRequired((() =>
							{
								Product = wardDrone;
							}));
							return;


						case DeviceConfigurationType.Cleaning:
							var cleaningDrone = new CleaningDroneEngineViewModel(_invoker, defaultConfig, defaultConfig.PollingTimeouts, DeviceName);
							cleaningDrone.ReportActivity += HandleReportActivity;
							Application.Current.Dispatcher.InvokeIfRequired((() =>
							{
								Product = cleaningDrone;
							}));
							return;

					}
				}
		}

		void HandleReportActivity(string news)
		{
			Application.Current.Dispatcher.InvokeIfRequired((() =>
			{
				Activities.Insert(0, news);
			}));
		}

		//private IEnumerable<Screen> GetDepartmentScreenSequence()
		//{
		//    var screens = new[] { Screen.Department };
		//    for (int i = 0; i < screens.Length; i++)
		//    {
		//        yield return screens[i];
		//    }
		//}

		//private IEnumerable<Screen> GetWardScreenSequence()
		//{
		//    var screens = new[] { Screen.Ward };
		//    for (int i = 0; i < screens.Length; i++)
		//    {
		//        yield return screens[i];
		//    }
		//}

		//private IEnumerable<Screen> GetCleaningcreenSequence()
		//{
		//    var screens = new[] { Screen.Cleaning };
		//    for (int i = 0; i < screens.Length; i++)
		//    {
		//        yield return screens[i];
		//    }
		//}
	}
}
