using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight;
using WCS.Core;

namespace WCS.Shared.Schedule
{
	public class LocationClockViewModel : ViewModelBase
	{
        private string _locationCode;
		private Detection _detection;
		private string _currentLocation;
		private int _partHour;
        private int _totalMins;
		private int _timeUnitInCurrentLocation;
		private string _timeUnit;
        private string _timeUnitCombined;
		private bool _isInCurrentLocation;
		private IDisposable _tickSource;

        public LocationClockViewModel(string locationCode)
		{
            _locationCode = locationCode;
			_currentLocation = "Unknown";
			_timeUnitInCurrentLocation = 0;
			_timeUnit = "";
            _timeUnitCombined = "";
			_isInCurrentLocation = false;
		}

		public string CurrentLocation
		{
			get { return _currentLocation; }
			set
			{
				_currentLocation = value;
				this.DoRaisePropertyChanged(() => CurrentLocation, RaisePropertyChanged);
			}
		}

        public int TotalMins
        {
            get { return _totalMins; }
            set
            {
                _totalMins = value;
                this.DoRaisePropertyChanged(() => TotalMins, RaisePropertyChanged);
            }
        }

		public int PartHour
		{
			get { return _partHour; }
			set
			{
				_partHour = value;
				this.DoRaisePropertyChanged(() => PartHour, RaisePropertyChanged);
			}
		}

		public int TimeUnitInCurrentLocation
		{
			get { return _timeUnitInCurrentLocation; }
			set
			{
				_timeUnitInCurrentLocation = value;
				this.DoRaisePropertyChanged(() => TimeUnitInCurrentLocation, RaisePropertyChanged);
			}
		}
		public string TimeUnitType
		{
			get { return _timeUnit; }
			set
			{
				_timeUnit = value;
				this.DoRaisePropertyChanged(() => TimeUnitType, RaisePropertyChanged);
			}
		}

        public string TimeUnitCombined
        {
            get { return _timeUnitCombined; }
            set
            {
                _timeUnitCombined = value;
                this.DoRaisePropertyChanged(() => TimeUnitCombined, RaisePropertyChanged);
            }
        }

		public bool IsInCurrentLocation
		{
			get { return _isInCurrentLocation; }
			set
			{
				_isInCurrentLocation = value;
				this.DoRaisePropertyChanged(() => IsInCurrentLocation, RaisePropertyChanged);
			}
		}

		/// <summary>
		/// Updates the location and stopwatch depending if the inputted detection is different from the previous one
		/// </summary>
		/// <param name="detection">The detection.</param>
		public void SetLatestDetection(Detection detection)
		{
			_detection = detection;

            bool isAlreadyIn = _locationCode == CurrentLocation;

		    CurrentLocation = detection.Direction == DetectionDirection.In
		                          ? detection.DetectionLocation.LocationCode
		                          : "Unknown";


            IsInCurrentLocation = detection.DetectionLocation.LocationCode == _locationCode;
			
            SetText();

            if (isAlreadyIn && !IsInCurrentLocation)		// turn stopwatch off
			{
				_tickSource.Dispose();
			}
            else if (!isAlreadyIn && IsInCurrentLocation)		// turn stopwatch on
			{
				var tickSource = Observable.Interval(TimeSpan.FromSeconds(5));
				_tickSource = tickSource.Subscribe(UpdateStopWatch);
			}
		}
		private void UpdateStopWatch(long tick)
		{
			SetText();
		}

		private void SetText()
		{
			var mins = Convert.ToInt32(Math.Floor((DateTime.Now - _detection.Timestamp).TotalMinutes));

		    TotalMins = mins;
            PartHour = mins % 60;
			TimeUnitInCurrentLocation = mins < 60 ? mins : mins/60;
			TimeUnitType = mins < 60 ? "min" : "hour";

            TimeUnitCombined = string.Format("{0}{1}", TimeUnitInCurrentLocation, mins < 60 ? string.Empty : "hr");
		}
	}
}
