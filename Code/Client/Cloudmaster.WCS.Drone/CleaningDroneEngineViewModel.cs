using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Core.Composition;
using WCS.Shared.Beds;
using WCS.Shared.Cleaning.Schedule;
using WCS.Shared.Schedule;


namespace Cloudmaster.WCS.Drone
{
	public	class CleaningDroneEngineViewModel: BaseDroneEngineViewModel
	{
		private CleaningViewModel _cvm;

		public CleaningDroneEngineViewModel(IWcsAsyncInvoker invoker, DeviceConfigurationInstance config, PollingTimeouts timeouts, string deviceName)
			: base(invoker)
		{

			_cvm = new CleaningViewModel(false, DefaultDeviceIdentity.AppVersion, deviceName, DefaultDeviceIdentity.ServerName);
			_cvm.InitialiseConfiguration(config, timeouts);
		}

	 
		protected override void DoRandomShit(long tick)
		{
			if (_cvm == null || _cvm.ScheduleViewModel.ScheduleRooms == null || !_cvm.ScheduleViewModel.ScheduleRooms.UnfilteredCollection.Any()) return;

			var beds = _cvm.ScheduleViewModel.ScheduleRooms.UnfilteredCollection.SelectMany(order => order.ScheduleItems).ToList();

			if (RollDice(20))	// 1-in-20 clean bed
			{
				var b = GetMeARandomDirtyMuthaUnkinBedNow(beds);
				if (b != null)
				{
					b.MarkAsDirtyCommand.Execute(null);
					//if (_invoker != null)
					//    _invoker.MarkBedAsCleanAsync(b.Id, BedCallback);

					Report(string.Format("Cleaned Bed[{0}] ", b.Id));
				}
			}
			if (RollDice(20))	// 1-in-20 dirty bed
			{
				var b = GetMeARandomCleanMuthaUnkinBedNow(beds);
				if (b != null)
				{
					b.MarkAsCleanCommand.Execute(null);
					//if (_invoker != null)
					//    _invoker.MarkBedAsCleanAsync(b.Id, BedCallback);

					Report(string.Format("Dirtied Bed[{0}] ", b.Id));
				}
			}

			if (RollDice(60))	// 1-in-60 add note
			{
				var b = GetMeARandomMuthaUnkinBedNow(beds);
				if (b != null)
				{
					var note = string.Format("Note {0}", DateTime.Now.ToString("HH:mm"));
					b.ConversationNotesViewModel.NoteText = note;
					b.ConversationNotesViewModel.AddNewNoteCommand.Execute(null);
		
					//if (_invoker != null)
					//    _invoker.AddNoteAsync(b.Id, note, BedCallback);

					Report(string.Format("Added Note '{0}' to Bed [{1}] ", note, b.Id));
				}
			}
 
		}

		private BedViewModel GetMeARandomMuthaUnkinBedNow(List<BedViewModel> beds)
		{
			if (beds.Any())
				return beds[Random.Next(0, beds.Count-1)];
			return null;
		}

		private BedViewModel GetMeARandomDirtyMuthaUnkinBedNow(List<BedViewModel> beds)
		{
			return GetMeARandomMuthaUnkinBedNow(beds.Where(b => b.Status != BedStatus.Clean).ToList());
		}

		private BedViewModel GetMeARandomCleanMuthaUnkinBedNow(List<BedViewModel> beds)
		{
			return GetMeARandomMuthaUnkinBedNow(beds.Where(b => b.Status == BedStatus.Clean).ToList());
		}
	}
}

