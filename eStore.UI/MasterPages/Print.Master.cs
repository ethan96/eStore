using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.IO;
using System.Net;
using System.Text;

namespace eStore.UI.MasterPages
{
    public partial class Print :Presentation.eStoreBaseControls.eStoreBaseMasterPage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
  
        }
        //Submit
        protected void btn_submit_Click(object sender, EventArgs e)
        {
            String path = Request.Url.AbsoluteUri;
            HttpWebRequest request = HttpWebRequest.Create(path) as HttpWebRequest;

            request.Method = "GET";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, System.Text.Encoding.GetEncoding("UTF-8"));
            string html = reader.ReadToEnd();
            reader.Close();


        }
    }
}