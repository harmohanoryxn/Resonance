using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS.Department.Model;
using Cloudmaster.WCS.Department.Processing.Feeds;
using Cloudmaster.WCS.IO;
using Cloudmaster.WCS.Processing;
using System.Windows;

namespace Cloudmaster.WCS.Department.Processing
{
	/// <summary>
	/// View Model for frequent background polling from feeds to populate the Department View Model
	/// </summary>
	public class DepartmentProcessor //: BaseProcessorViewModel
	{
		public event Action<ServerInformation> NewDataAvailable;

		private IIguanaFeedsProcessor _iguanaFeedsProcessor;

		public DepartmentProcessor()
		{
			_instance = this;

#if DISCONNECTED
			_iguanaFeedsProcessor = new DisconnectedIguanaFeedsProcessor();
#else
			_iguanaFeedsProcessor = new GetIguanaFeedsProcessor();
#endif

			RefreshOrders();

			InitializeRefreshOrdersTimer();

		}

		private static DepartmentProcessor _instance;
		public static DepartmentProcessor Instance
		{
			get { return _instance; }
		}


		private void InitializeRefreshOrdersTimer()
		{
			DispatcherTimer timer = new DispatcherTimer();

			timer.Tick += RefreshOrders_Tick;
			timer.Interval = TimeSpan.FromSeconds(10);
			timer.Start();
		}

		void RefreshOrders_Tick(object sender, EventArgs e)
		{
			RefreshOrders();
		}

		public void RefreshOrders()
		{
			_iguanaFeedsProcessor.GetAsync(RecieveAsyncResults);
		}

		private void RecieveAsyncResults(ServerInformation results)
		{
			var evt = NewDataAvailable;
			if (evt != null)
			{
				evt(results);
			}
		}
	}
}
