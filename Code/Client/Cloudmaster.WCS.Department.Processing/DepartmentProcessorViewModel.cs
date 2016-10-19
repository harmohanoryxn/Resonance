using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloudmaster.WCS.Department.Processing.Feeds;
using Cloudmaster.WCS.Processing;
using System.Windows;

namespace Cloudmaster.WCS.Department.Processing
{
	/// <summary>
	/// View Model for frequent background polling from feeds to populate the Department View Model
	/// </summary>
    public class DepartmentProcessorViewModel : BaseProcessorViewModel
    {
    	private GetIguanaFeedsProcessor _getIguanaFeedsProcessor;

		#region Singleton Members

		private DepartmentProcessorViewModel()
		{

		}

		private static DepartmentProcessorViewModel instance;

		public static DepartmentProcessorViewModel Instance
		{
			get
			{
				if (instance == null)
				{
					throw new InvalidOperationException("Must initialize an instance of the processor.");
				}
				else
				{
					return instance;
				}
			}
		}

		#endregion

        public static void Initialize()
        {
            instance = new DepartmentProcessorViewModel();

            DepartmentProcessorViewModel.Instance.GetIguanaFeedsProcessor = new GetIguanaFeedsProcessor();
        }

    	public GetIguanaFeedsProcessor GetIguanaFeedsProcessor
    	{
    		get { return _getIguanaFeedsProcessor; }
    		set
    		{
    			_getIguanaFeedsProcessor = value;
				RaisePropertyChanged("GetIguanaFeedsProcessor");
    		}
    	}


		public void RefreshOrders()
		{
			GetIguanaFeedsProcessor.ExecuteInBackround();
		}
    }
}
