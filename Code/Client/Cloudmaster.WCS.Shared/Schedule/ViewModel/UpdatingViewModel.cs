using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using GalaSoft.MvvmLight;

namespace WCS.Shared.Schedule
{
	/// <summary>
	/// Acts as a base type for classes that contact the server directly and receive updates independently from the poller
	/// </summary>
	public class UpdatingViewModel : ViewModelBase
	{
		private int _updateLock;

		public UpdatingViewModel()
		{
			_updateLock = 0;
		}
		protected void FlagAsAwaitingUpdate()
		{
			Interlocked.Exchange(ref _updateLock, 1);
		}

		protected void MarkUpdateAsArrived()
		{
			Interlocked.Exchange(ref _updateLock, 0);
		}

		protected bool CheckIfUpdateIsPending
		{
			get
			{
				return _updateLock == 1;
			}
		}

	}
}
