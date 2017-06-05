using HL7MessageServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HL7MessageServer
{
    public partial class DeviceAddition : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_Click(object sender, EventArgs e)
        {
            WCSHL7Entities wcs = new WCSHL7Entities();
            Device dv = new Device();
            dv.name = "DESKTOP-08GEVV1";
            dv.description = "Dev Server";
            wcs.Devices.Add(dv);
            wcs.SaveChanges();
            int deviceid = wcs.Devices.Where(c => c.name == "DESKTOP-08GEVV1").Select(d => d.deviceId).FirstOrDefault();
            var configurations =  wcs.Configurations.Select(c => c.configurationId).DefaultIfEmpty().ToList();
            int shrcutkey = 1;
            foreach(var v in configurations)
            {
                DeviceConfiguration dc = new DeviceConfiguration();
                dc.configurationId = v;
                dc.deviceId = deviceid;
                dc.shortcutKeyNo = shrcutkey;
                dc.cleaningBedDataTimeout = 30;
                dc.admissionsTimeout = 30;
                dc.dischargeTimeout = 30;
                dc.orderTimeout = 10;
                dc.presenceTimeout = 30;
                dc.rfidTimeout = 30;
                wcs.DeviceConfigurations.Add(dc);
                wcs.SaveChanges();
                shrcutkey = shrcutkey + 1;
            }
            

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            DateTime datecreated = DateTime.Now;
            TimeSpan ts = TimeSpan.Parse("18:35");
            DateTime updatedtime = Convert.ToDateTime(datecreated.Date + ts);
            lbl.Text = updatedtime.ToString();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            WCSHL7Entities wcs = new WCSHL7Entities();
            var locationval = wcs.Locations;
            string s = "";
            foreach(var l in locationval)
            {
                s += "case '"+l.code+"':" +
                        "locvalue = '" + l.name + "';" +
                "break; ";
            }
            lbl.Text = s;
        }
    }
}