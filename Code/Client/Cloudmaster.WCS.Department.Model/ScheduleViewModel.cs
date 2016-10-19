using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS.Controls;
using Cloudmaster.WCS.Controls.Browser;
using Cloudmaster.WCS.Controls.ObjectEditor;
using Cloudmaster.WCS.Controls.Scheduling;
using Cloudmaster.WCS.Model.Feeds;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Cloudmaster.WCS.Department.Model
{
	public class ScheduleViewModel : ViewModelBase, IBrowserItem
	{
		private bool _isHead;
		private bool _isRoot;

		private FilteredObservableCollection<AppointmentViewModel> _appointments;
		private ObservableCollection<WardViewModel> _wards;
		private CollectionViewSource _source;
		private AppointmentViewModel _selectedAppointment;
		private WardViewModel _selectedWard;
		private readonly WardViewModel _defaultWard;
		 
		private string _status;
		private DateTime? _lastSynchronised;

		public ScheduleViewModel()
		{
			Orders = new OrdersFeed();
			OrderRequests = new OrderRequestsFeed();
			Appointments = new FilteredObservableCollection<AppointmentViewModel>();
			Source = new CollectionViewSource();
			Source.Source = Appointments;
			Source.Filter += Source_Filter;

			var defaultWard = Application.Current.Properties["DefaultModality"].ToString();

			_wards = new ObservableCollection<WardViewModel>();
			var wardSection = ConfigurationManager.GetSection("wards") as ConfigWardSection;
			foreach (ConfigWard ward in wardSection.Wards)
			{
				var wardvm = new WardViewModel(ward);
				wardvm.RequiresSelection += HandleNewWardSelection;
				_wards.Add(wardvm);
			}
			_defaultWard = _wards.First(w => String.CompareOrdinal(w.Code, defaultWard) == 0);
			SelectedWard = _defaultWard;
		}

		public DepartmentViewModel Department { get; set; }
		public OrderRequestsFeed OrderRequests { get; set; }
		public OrdersFeed Orders { get; set; }

		public void MergeResults(ServerInformation results)
		{
			Orders.MergeResults(results);
			var appointments = Orders.Items.ToList().Select(o => new AppointmentViewModel(o, Department.ExplodeAppointment));

			// TODO FIX how this collection is constructed
			var departmentAppointmentViewModels = new FilteredObservableCollection<AppointmentViewModel>();
			appointments.ForEach(departmentAppointmentViewModels.Add);
			Appointments = departmentAppointmentViewModels;
			Source.Source = Appointments;
			Source.View.Refresh();

			Wards.ForEach(w => w.Synchronise(Orders));

			LastSynchronised = Orders.LastSyncronized;
		}


		public AppointmentViewModel SelectedAppointment
		{
			get { return _selectedAppointment; }
			set
			{
				_selectedAppointment = value;
				this.DoRaisePropertyChanged(() => SelectedAppointment, RaisePropertyChanged);
			}
		}

		public WardViewModel DefaultWard
		{
			get { return _defaultWard; }
		}

		public WardViewModel SelectedWard
		{
			get { return _selectedWard; }
			set
			{
				_selectedWard = value;
				this.DoRaisePropertyChanged(() => SelectedWard, RaisePropertyChanged);
				RefreshAppointmentsForWard();

				// for some reason - we hide the wards when CAR is seleced 
				Wards.ForEach(w => w.Visibility = (String.CompareOrdinal(_selectedWard.Code, OrderServiceType.CAR) == 0)?Visibility.Collapsed:Visibility.Visible);
			}
		}

		public ObservableCollection<WardViewModel> Wards
		{
			get { return _wards; }
			set
			{
				_wards = value;
				this.DoRaisePropertyChanged(() => Wards, RaisePropertyChanged);
			}
		}

		public FilteredObservableCollection<AppointmentViewModel> Appointments
		{
			get { return _appointments; }
			set
			{
				_appointments = value;
				this.DoRaisePropertyChanged(() => Appointments, RaisePropertyChanged);
			}
		}

		public CollectionViewSource Source
		{
			get { return _source; }
			set
			{
				_source = value;
				this.DoRaisePropertyChanged(() => Source, RaisePropertyChanged);
			}
		}

		public DateTime? LastSynchronised
		{
			get { return _lastSynchronised; }
			set
			{
				_lastSynchronised = value;
				this.DoRaisePropertyChanged(() => LastSynchronised, RaisePropertyChanged);

				TimeSpan twoMinutes = TimeSpan.FromMinutes(2);
				TimeSpan oneHour = TimeSpan.FromHours(1);
				TimeSpan oneDay = TimeSpan.FromDays(1);

				DateTime now = DateTime.Now;


				if (!_lastSynchronised.HasValue)
				{
					Status = "Loading";
				}
				else
				{
					if (now.Subtract(_lastSynchronised.Value) < twoMinutes)
					{
						Status = "Up-to-date";
					}
					else if (now.Subtract(_lastSynchronised.Value) < oneHour)
					{
						int mintues = now.Subtract(_lastSynchronised.Value).Minutes;

						Status = string.Format("Last updated {0} minutes ago", mintues);
					}
					else if (now.Subtract(_lastSynchronised.Value) < oneDay)
					{
						int hours = now.Subtract(_lastSynchronised.Value).Hours;

						Status = string.Format("Last updated {0} hours ago", hours);
					}
					else
					{
						Status = string.Format("Last updated over 1 day ago");
					}
				}
			}
		}

		public string Status
		{
			get { return _status; }
			set
			{
				_status = value;
				this.DoRaisePropertyChanged(() => Status, RaisePropertyChanged);
			}
		}

		#region IBrowser Members

		public event Action<IBrowserItem> RequestClose;

		public event Action<IBrowserItem> LookForFocus;

		public bool IsHead
		{
			set
			{
				_isHead = value;
				this.DoRaisePropertyChanged(() => IsHead, RaisePropertyChanged);
			}
			get { return _isHead; }
		}

		public bool IsRoot
		{
			set
			{
				_isRoot = value;
				this.DoRaisePropertyChanged(() => IsRoot, RaisePropertyChanged);
			}
			get { return _isRoot; }
		}

		public void TryLookForFocus()
		{
			var lff = LookForFocus;
			if (lff != null)
				lff(this);
		}

		public RelayCommand LookForFocusCommand
		{
			get { return new RelayCommand(DoLookForFocusCommand); }
		}

		private void DoLookForFocusCommand()
		{
			TryLookForFocus();
		}

		#endregion

		private void Source_Filter(object sender, FilterEventArgs e)
		{
			var appointment = e.Item as AppointmentViewModel;
			if (appointment == null)
				throw new InvalidOperationException("Wrong List Item Type - FIX");

			bool isCompleted = (appointment.ObservationEndDateTime.HasValue);

			if (!isCompleted)
			{
				e.Accepted = (String.CompareOrdinal(appointment.Service, SelectedWard.Code) == 0);
			}
			else
			{
				e.Accepted = false;
			}
		}

		public void RefreshAppointmentsForWard()
		{
			Source.View.Refresh();
		}

		public void ResetDefaultWard()
		{
			SelectedWard = _defaultWard;
	
		}
		
		private void HandleNewWardSelection(WardViewModel ward)
		{
			SelectedWard = ward;
		}
	}
}
