using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using System.Configuration;

namespace H7Message
{
    public static class HL7ToApi
    {


        public static string consume(string filename, string type)
        {
            using (var client = new HttpClient())
            {
                try
                {


                    string uri = ConfigurationManager.AppSettings["URIValue"];
                    uri = uri + "?filename=" + filename;
                    var stringContent = new StringContent(filename);
                    var response = client.PostAsync(uri, stringContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        // by calling .Result you are performing a synchronous call
                        var responseContent = response.Content;

                        // by calling .Result you are synchronously reading the result
                        string responseString = responseContent.ReadAsStringAsync().Result;
                        
                        return responseString;
                    }
                    else
                    {
                        HL7messageToFile.Exceptionhandler(response.Content.ToString(), "Error");
                        return "Error";
                    }
                }
                catch(Exception ex)
                {
                    HL7messageToFile.Exceptionhandler(ex.Message, ex.InnerException.ToString());
                    return ex.Message;
                }
            }
        }

    }
}
