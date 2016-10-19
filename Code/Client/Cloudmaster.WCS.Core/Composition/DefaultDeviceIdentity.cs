using System;
using System.Reflection;
using System.Configuration;
using System.ServiceModel.Configuration;


namespace WCS.Core.Composition
{
	public class DefaultDeviceIdentity : IDeviceIdentity
	{

		public string DeviceName
		{
			get
			{
 				return Environment.MachineName; 
			}
		}


        public static string ServerName
        {
            get
            {
                ClientSection clientSection = (ClientSection) ConfigurationManager.GetSection("system.serviceModel/client");

                Uri serverEndpoint = clientSection.Endpoints[0].Address;

                return serverEndpoint.Host;
            }
        }

		private static object _lock = new object();
		private static string _appVersion;
		public static string AppVersion
		{
			get
			{
				lock (_lock)
				{
					if (string.IsNullOrEmpty(_appVersion))
					{
						if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
						{
							Version version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
							_appVersion = version.ToString();
						}
						else
						{
							_appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
						}
					}
					return _appVersion;
				}
			}
		}

		public string ApplicationVersion
		{
			get { return AppVersion; }
		}
	}
}
