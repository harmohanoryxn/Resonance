
using System;
using System.Linq;
using System.Reflection;
using WCS.Core;
using WCS.Core.Composition;

namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
	public partial class DeviceConfiguration : IIdentifable, IDeviceIdentity
	{
		public string ApplicationVersion
		{
			get { return DefaultDeviceIdentity.AppVersion; }
		}

		public int Id
		{
			get { return 1; }
		}
		public int GetFingerprint()
		{
			var fp = 1;
			if (!string.IsNullOrEmpty(DeviceName))
				fp = fp ^ DeviceName.GetHashCode();

			if (Instances.Any()) fp = fp ^ Instances.Select(i => i.GetFingerprint()).ToList().Aggregate((accFp, nextFp) => nextFp + accFp);
			return fp;

		}
	}
}