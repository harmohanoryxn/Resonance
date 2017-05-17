using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HL7MessageServer
{
    public partial class ErrorReader : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                ddlfill();
            }
        }

        protected void btnShowerrors_Click(object sender, EventArgs e)
        {
            
            IList<errorfeilds> gridval = new List<errorfeilds>();
            string directory = "";
            if(ddlErrorfolders.Items.Count>0)
            {
                directory = ddlErrorfolders.SelectedValue;
            }
            else
            {
                directory= ConfigurationManager.AppSettings["ErrorFolder"];
            }
            DirectoryInfo info = new DirectoryInfo(ddlErrorfolders.SelectedValue);
            FileInfo[] files = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
            foreach (string file in Directory.EnumerateFiles(ddlErrorfolders.SelectedValue))
            {
                string message = File.ReadAllText(file);
                errorfeilds err = new errorfeilds();
                err.error = message;
                err.filename = file;
                err.filemodified=  File.GetLastWriteTime(file);
                gridval.Add(err);
            }
            griderror.DataSource = gridval;
            griderror.DataBind();
        }
        protected void ddlfill()
        {
            string querystring = ConfigurationManager.AppSettings["ErrorFolder"]; ;
            var directories = Directory.GetDirectories(querystring);
            ddlErrorfolders.DataSource = directories;
            ddlErrorfolders.DataBind();
        }

    }
    public class errorfeilds
    {
        public string error { get; set; }
        public string filename { get; set; }
        public DateTime filemodified { get; set; }

    }
}