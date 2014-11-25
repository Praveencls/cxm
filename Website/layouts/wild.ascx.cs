using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Website.Models;

namespace Website.layouts
{
    public partial class wild : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://dms72/-/item/v1/?sc_itemid={74516918-5271-43E2-9CC8-3A4060C9156B}&sc_database=master");
            var LastUrlString = Sitecore.Web.WebUtil.GetUrlName(0);

            Response.Write(String.Format("Last url value is wild card:  {0}  <br/>", LastUrlString));

            request.Headers["X-Scitemwebapi-Username"] = @"sitecore\admin";
            request.Headers["X-Scitemwebapi-Password"] = "b";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Response.Write(String.Format("Content length is {0}", response.ContentLength));
            Response.Write(String.Format("Content type is {0}", response.ContentType));
            // Get the stream associated with the response.
            Stream receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

            Response.Write("<br /> Response stream received. <br />");
           
            string json = readStream.ReadToEnd();
            var recs = JsonConvert.DeserializeObject<Record>(json);

            if (recs != null)
            {
                foreach (var item in recs.Result.Items) {
                    Response.Write(item.DisplayName + "<br/>");
                }
            }
            
        }
    }
}