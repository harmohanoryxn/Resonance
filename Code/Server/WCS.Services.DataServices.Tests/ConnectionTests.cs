using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net;
using System.Diagnostics;

namespace WCS.Services.DataServices.Tests
{
    [TestClass]
    public class ConnectionTests
    {
        [TestMethod]
        public void GetOrders()
        {
            using (DataServicesClient.DataServicesClient client = new DataServicesClient.DataServicesClient())
            {
                AddMessageHeaders(client);

                // When using Restful Services parameters must be strings

                // Threfore in order to pass date the universal time standard as used in iCal should be used

                DataServicesClient.Order[] orders = client.GetOrders(DateTime.Now.ToUniversalTime().ToString("yyyyMMdd\\THHmmss\\Z"));
            }
        }

        private static void AddMessageHeaders(DataServicesClient.DataServicesClient client)
        {
            string computerName = System.Environment.MachineName;
            string os = System.Environment.OSVersion.VersionString;

            long privateMemoryUsage = Process.GetCurrentProcess().PrivateMemorySize64;

            using (OperationContextScope scope = new OperationContextScope(client.InnerChannel))
            {
                HttpRequestMessageProperty httpRequestProperty = new HttpRequestMessageProperty();

                httpRequestProperty.Headers.Add(HttpRequestHeader.UserAgent, "WCS");
                httpRequestProperty.Headers.Add(WCSHttpRequestHeaderNames.DeviceName, computerName);
                httpRequestProperty.Headers.Add(WCSHttpRequestHeaderNames.OS, os);
                httpRequestProperty.Headers.Add(WCSHttpRequestHeaderNames.Location, "ACC1");
                httpRequestProperty.Headers.Add(WCSHttpRequestHeaderNames.Description, "Mother Teresa Ward");
                httpRequestProperty.Headers.Add(WCSHttpRequestHeaderNames.PrivateMemorySize, privateMemoryUsage.ToString());

                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
            }
        }
    }
}
