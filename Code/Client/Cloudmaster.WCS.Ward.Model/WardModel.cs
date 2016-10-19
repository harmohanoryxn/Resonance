using System; 
using Cloudmaster.WCS.Model;
using System.Windows; 

namespace Cloudmaster.WCS.Ward.Model
{
    public class WardModel : ModelBase
	{
		private DepartmentFeeds _feeds;
		private Alerts _alerts;
		private WardLabels _wardLabels;

		#region Singleton Instance

        private WardModel()
        {

        }

        private static WardModel _instance;

        public static WardModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException("Must initialize an instance of the model.");
                }
                else
                {
                    return _instance;
                }
            }
		}

		#endregion

        public static void Initialize(string wardDisplayName)
        {
            _instance = new WardModel();

            _instance.NavigationViewModel = new NavigationViewModel();

            _instance.SecurityViewModel = new DepartmentSecurityViewModel();
            _instance.Feeds = new DepartmentFeeds();
            _instance.Alerts = new Alerts();
            _instance.WardLabels = new WardLabels();

            _instance.WardLabels.WardDisplayName = wardDisplayName;
        }

		public DepartmentFeeds Feeds
		{
			get { return _feeds; }
			set
			{
				_feeds = value;
				RaisePropertyChanged("Feeds");
			}
		}

		public Alerts Alerts
		{
			get { return _alerts; }
			set
			{
				_alerts = value;
				RaisePropertyChanged("Alerts");
			}
		}

		public WardLabels WardLabels
		{
			get { return _wardLabels; }
			set
			{
				_wardLabels = value;
				RaisePropertyChanged("DepartmentLabels");
			}
		}
      }
}
