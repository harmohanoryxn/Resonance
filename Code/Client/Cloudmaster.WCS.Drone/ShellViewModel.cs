using System.Collections.ObjectModel;
using System.Linq;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight;
using WCS.Core;
using WCS.Shared;

namespace Cloudmaster.WCS.Drone
{
	public sealed class ShellViewModel : ViewModelBase
	{
		private WcsMef _wcsMef;
	
		public ObservableCollection<DroneViewModel> Drones { get; set; }

		public ShellViewModel()
		{
			_wcsMef = new WcsMef();			//initialise the MEF container

			Drones = new ObservableCollection<DroneViewModel>();
            Drones.Add(new DroneViewModel("DRONE CT", Screen.Department) { Name = "CT" });
			Drones.Add(new DroneViewModel("DRONE CARD", Screen.Department) { Name = "CARD" });
			Drones.Add(new DroneViewModel("DRONE TH1", Screen.Department) { Name = "TH1" });
			Drones.Add(new DroneViewModel("DRONE ER", Screen.Department) { Name = "ER" });

			Drones.Add(new DroneViewModel("DRONE WARD1", Screen.Ward) { Name = "WARD1" });
			Drones.Add(new DroneViewModel("DRONE WARD2", Screen.Ward) { Name = "WARD2" });
			Drones.Add(new DroneViewModel("DRONE WARD3", Screen.Ward) { Name = "WARD3" });

			Drones.Add(new DroneViewModel("DRONE CLEANER WARD1", Screen.Cleaning) { Name = "Cleaning WARD1" });
			Drones.Add(new DroneViewModel("DRONE CLEANER WARD2", Screen.Cleaning) { Name = "Cleaning WARD2" });
			Drones.Add(new DroneViewModel("DRONE CLEANER WARD3", Screen.Cleaning) { Name = "Cleaning WARD3" });

			Drones.Add(new DroneViewModel("DRONE Physio", Screen.Cleaning) { Name = "Physio" });
			
			Drones.ToList().ForEach(d => d.StartCommand.Execute(null));
		}
	}
}