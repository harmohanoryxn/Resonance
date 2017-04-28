using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase.Protocol;
using NHapi.Base.Parser;
using NHapiTools.Base.Parser;
using NHapiTools.Base.Validation;
using NHapi.Base.Model;
using System.Configuration;
using NHapi.Base.Util;
using System.Xml;

namespace HL7Fuse.Protocol
{
    public class HL7RequestInfoParser : IRequestInfoParser<HL7RequestInfo>
    {
        #region Private properties
        private bool HandleEachMessageAsEvent
        {
            get
            {
                bool result = true;
                if (!bool.TryParse(ConfigurationManager.AppSettings["HandleEachMessageAsEvent"], out result))
                    result = true;

                return result;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Parse the message to a RequestInfo class
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public HL7RequestInfo ParseRequestInfo(string message)
        {
            return ParseRequestInfo(message, string.Empty);
        }

        public HL7RequestInfo ParseRequestInfo(string message, string protocol)
        {
            HL7RequestInfo result = new HL7RequestInfo();
            PipeParser parser = new PipeParser();

            try
            {
                ConfigurableContext configContext = new ConfigurableContext(parser.ValidationContext);
                parser.ValidationContext = configContext;
            }
            catch(Exception ex)
            {
                // Ignore any error, since the config is probably missing
            }

            try
            {


                IMessage hl7Message = parser.Parse(message);
                result = new HL7RequestInfo();
                if (HandleEachMessageAsEvent)
                    result.Key = "V" + hl7Message.Version.Replace(".", "") + "." + hl7Message.GetStructureName();
                else
                    result.Key = "V" + hl7Message.Version.Replace(".", "") + ".MessageFactory";

                if (!string.IsNullOrEmpty(protocol))
                    result.Key += protocol;

                result.Message = hl7Message;
            }
            catch(Exception ex)
            {
                XmlDocument xml = HL7ToXmlConverter.ConvertToXml(message);
                var version = xml.GetElementsByTagName("MSH.11").Item(0);
                result.versionname = Convert.ToString(version.InnerText);
                result.feild10 = Convert.ToString(xml.GetElementsByTagName("MSH.9").Item(0).InnerText);
                result.sendingapp =Convert.ToString(xml.GetElementsByTagName("MSH.2").Item(0).InnerText);
                result.sendingEnvironment = Convert.ToString(xml.GetElementsByTagName("MSH.3").Item(0).InnerText);
                result.feild11= Convert.ToString(xml.GetElementsByTagName("MSH.10").Item(0).InnerText);
                result.feild15 = Convert.ToString(xml.GetElementsByTagName("MSH.14").Item(0).InnerText);
                result.feild16 = Convert.ToString(xml.GetElementsByTagName("MSH.15").Item(0).InnerText);

            }

            // Parse the message
            return result;
        }
        #endregion
    }
}
