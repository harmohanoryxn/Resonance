using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Cloudmaster.WCS.IO;
using Cloudmaster.WCS.Ward.Processing;
using Cloudmaster.WCS;
using Cloudmaster.WCS.Ward.Model;

namespace Cloudmaster.WCS.Ward
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            WardModel.Initialize(Cloudmaster.WCS.Ward.Properties.Settings.Default.WardDisplayName);
            WardProcessorViewModel.Initialize(Cloudmaster.WCS.Ward.Properties.Settings.Default.WardIdentifier);

            this.Resources.Add("model", WardModel.Instance);
            this.Resources.Add("processor", WardModel.Instance);

            ConnectionStringsSection connectionStringsSection = ConfigurationHelper.GetConnectionStringsAndEncryptIfNotProtected("WCS.Ward.exe");

            WardProcessorViewModel.AdmissionsProviderConnectionString = connectionStringsSection.ConnectionStrings["IAdmissionsProviderConnectionString"].ConnectionString;

            WardProcessorViewModel.Instance.GetIguanaWardFeedsProcessor.ExecuteInBackround();

            WardModel.Instance.WardLabels.UpdateOrderStatus();

            int securityLockInterval = 0;

            int.TryParse(Cloudmaster.WCS.Ward.Properties.Settings.Default.SecurityLockInterval, out securityLockInterval);

            WardModel.Instance.SecurityViewModel.Initialize(securityLockInterval);
        }
    }
}
