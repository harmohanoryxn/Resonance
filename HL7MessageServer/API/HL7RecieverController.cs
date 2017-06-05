using H7Message;
using HL7Messages;
using HL7MessageServer.Classes;
using NHapi.Base.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.Description;

namespace HL7MessageServer.API
{
    public class HL7RecieverController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>

        
        public void Post(HLMessage obj)
        {
            HLMessageToDB hl7 = new HLMessageToDB();
            
            if(obj==null)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("We cannot proccess empty HL7 message")),
                    ReasonPhrase = "HL7 Message is empty",

                };
            }
            else
            {
                hl7.HL7MessageToDB(obj.HL7Message);
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("User already enrolled for this course")),
                    ReasonPhrase = "User already enrolled for this course",

                };
            }
           

            return;
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}