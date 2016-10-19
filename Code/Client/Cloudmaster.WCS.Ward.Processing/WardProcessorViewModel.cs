using System;
using Cloudmaster.WCS.Processing;
using Cloudmaster.WCS.Ward.Feeds;

namespace Cloudmaster.WCS.Ward.Processing
{
	public class WardProcessorViewModel : BaseProcessorViewModel
	{
		private GetIguanaWardFeedsProcessor _getIguanaWardFeedsProcessor;

		#region Singleton Members

		private WardProcessorViewModel()
		{
		}

		private static WardProcessorViewModel _instance;

		public static WardProcessorViewModel Instance
		{
			get
			{
				if (_instance == null)
				{
					throw new InvalidOperationException("Must initialize an instance of the processor.");
				}
				else
				{
					return _instance;
				}
			}
		}

		#endregion

		public static void Initialize(string wardIdentifier)
		{
			_instance = new WardProcessorViewModel();

			WardProcessorViewModel.Instance.GetIguanaWardFeedsProcessor = new GetIguanaWardFeedsProcessor(wardIdentifier);
		}

		public GetIguanaWardFeedsProcessor GetIguanaWardFeedsProcessor
		{
			get { return _getIguanaWardFeedsProcessor; }
			set
			{
				_getIguanaWardFeedsProcessor = value;
				RaisePropertyChanged("GetIguanaWardFeedsProcessor");
			}
		}


		public void RefreshOrders()
		{
			GetIguanaWardFeedsProcessor.ExecuteInBackround();
		}
	}
}
