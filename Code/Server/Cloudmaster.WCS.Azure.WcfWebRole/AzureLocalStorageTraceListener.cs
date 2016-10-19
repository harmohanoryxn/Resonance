using System;
using System.Diagnostics;
using System.IO;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Cloudmaster.WCS.Azure.WcfWebRole
{
	public class AzureLocalStorageTraceListener : XmlWriterTraceListener
	{
		public AzureLocalStorageTraceListener()
			: base(Path.Combine(AzureLocalStorageTraceListener.GetLogDirectory().Path, "Cloudmaster.WCS.Azure.WcfWebRole.svclog"))
		{
		}

		public static DirectoryConfiguration GetLogDirectory()
		{
			DirectoryConfiguration directory = new DirectoryConfiguration();
			directory.Container = "wad-tracefiles";
			directory.DirectoryQuotaInMB = 10;
			directory.Path = RoleEnvironment.GetLocalResource("Cloudmaster.WCS.Azure.WcfWebRole.svclog").RootPath;
			return directory;
		}
	}
}
