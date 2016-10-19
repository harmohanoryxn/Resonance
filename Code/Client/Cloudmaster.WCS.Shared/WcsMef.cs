using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using Cloudmaster.WCS.DataServicesInvoker;
using WCS.Core.Composition;

namespace WCS.Shared
{
	public class WcsMef
	{
		public static CompositionContainer Container { get; private set; }

		public WcsMef()
		{
			Container = new CompositionContainer(new TypeCatalog(new[] { typeof(WcsExceptionHandler), typeof(WcsAsyncInvoker), typeof(WcsClientLogger) }),true);
			Container.ComposeExportedValue<IDeviceIdentity>("IDeviceIdentity", new DefaultDeviceIdentity());
		}
	}
}
