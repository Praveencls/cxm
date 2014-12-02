using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Sitecore.Analytics;
using Sitecore.Analytics.Pipelines.StartTracking;

namespace Elma.SitecoreUtil.Pipelines.StartTracking
{
    public class OverrideIPAddress
    {
        public string IpUrl
        {
            get;
            set;
        }

        public string Ip
        {
            get;
            set;
        }

        public void Process(StartTrackingArgs args)
        {
            //if (Tracker.CurrentVisit == null || Tracker.CurrentVisit.GeoIp == null || Tracker.CurrentVisit.Ip == null)
            if (Tracker.CurrentVisit == null || Tracker.CurrentVisit.Ip == null)
            {
                return;
            }

            //string ip = new IPAddress(Tracker.CurrentVisit.GeoIp.Ip).ToString();
            string ip = new IPAddress(Tracker.CurrentVisit.Ip).ToString();

            if (ip != "0.0.0.0" && ip != "127.0.0.1")
            {
                return;
            }

            string rawIpValue = "";

            if (Ip.IsNullOrEmpty())
            {
                if (IpUrl.IsNullOrEmpty())
                {
                    return;
                }
                else
                {
                    rawIpValue = Sitecore.Web.WebUtil.ExecuteWebPage(IpUrl);
                }
            }
            else
            {
                rawIpValue = Ip;
            }
            
            IPAddress address = IPAddress.Parse(rawIpValue);
            Tracker.CurrentVisit.GeoIp = Tracker.Visitor.DataContext.GetGeoIp(address.GetAddressBytes());
        }
    }
}
