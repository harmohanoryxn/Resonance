using System.Collections.Generic;

namespace Cloudmaster.WCS.IO
{
	/// <summary>
	/// Acts a a base for types that connect to WCF services
	/// </summary>
    public abstract class ExternalServicesBase 
    {
        public static Dictionary<string, string> ParseConnectionString(string connectionString)
        {
            Dictionary<string, string> connectionStringValues = new Dictionary<string, string>();

            string[] conenctionStringNameValuePairs = connectionString.Split(';');

            foreach (string pair in conenctionStringNameValuePairs)
            {
                string[] values = pair.Split('=');

                connectionStringValues.Add(values[0].Trim(), values[1].Trim());
            }

            return connectionStringValues;
        }
    }
}
