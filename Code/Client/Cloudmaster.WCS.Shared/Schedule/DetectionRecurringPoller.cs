using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;

namespace WCS.Shared.Schedule
{
	/// <summary>
	/// View Model for frequent background polling for RFID Detections
	/// </summary>
	public class DetectionRecurringPoller
	{
		public event Action<IList<Detection>> DetectionsUpdateAvailable;

		private Timer _timer;
		private int _timeout;

		private IWcsAsyncInvoker _invoker;
		private PatientCodes _patients;

		public DetectionRecurringPoller(IWcsAsyncInvoker invoker, int timeoutInSeconds)
		{
			_invoker = invoker;
			_timeout = timeoutInSeconds;

			_timer = new Timer(HandleTick);

			_timer.Change(0, _timeout * 1000);

			_patients = new PatientCodes();
		}

		internal void SetPatients(IEnumerable<string> patients)
		{
			_patients.TryMerge(patients);
				HitServerForData();

				_timer.Change(_timeout * 1000, _timeout * 1000);
		}

		void HandleTick(object sender)
		{
			HitServerForData();
		}

		public void HitServerForData()
		{
			if (_patients.Any())
				_invoker.GetEntireDetectionHistoryAsync(DateTime.Today, DateTime.Today.AddDays(1), _patients, RecieveAsyncResults);
		}

		public void SetTimeout(int timeoutInSeconds)
		{
			if (_timeout == timeoutInSeconds) return;

			_timer.Change(0, timeoutInSeconds * 1000);
		}

		private void RecieveAsyncResults(IList<Detection> results)
		{
			var evt = DetectionsUpdateAvailable;

			if (results == null || !results.Any() || evt == null) return;

			var todaysDetections = results.Where(detection => detection.Timestamp.Date == DateTime.Today.Date).ToList();
			if (!todaysDetections.Any()) return;

			evt(todaysDetections);

		}
	}
}
