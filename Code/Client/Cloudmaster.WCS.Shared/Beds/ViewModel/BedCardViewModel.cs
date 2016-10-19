using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace WCS.Shared.Beds
{
	public class BedCardViewModel : ViewModelBase
	{

		public BedCardViewModel(BedViewModel bed)
		{
			Bed = bed;
		}

		public BedViewModel Bed { get; private set; }

		//public string BedName
		//{
		//    get { return _bed.Name; }
		//}

	}
}
