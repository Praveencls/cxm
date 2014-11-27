using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.ItemWebApi.Pipelines.Request;
using Sitecore.ItemWebApi;

namespace CoreLib.WebItemAPI
{
    public class SwitchToXmlSerializer : RequestProcessor
    {
        public override void Process([Sitecore.NotNull] RequestArgs arguments)
        {
            if (System.Web.HttpContext.Current.Request.QueryString["type"] == "xml")
            {
                Context.Current.Serializer = new XmlSerializer();
            }
        }
    }
}
