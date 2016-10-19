using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WCS.Shared.Ward.Schedule;

namespace WCS.Shared.Location
{
	public class RfidCardViewModel : ViewModelBase
	{
		public RfidCardViewModel(PatientViewModel patient)
		{
			Patient = patient;
			 
		}

		public PatientViewModel Patient { get; private set; }
		
	  
		 
	}
}
