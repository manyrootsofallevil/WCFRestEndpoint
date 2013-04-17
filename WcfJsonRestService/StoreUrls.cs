using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Configuration;

namespace WcfJsonRestService
{
   
    public class StoreUrls : IStoreUrls
    {
        [WebInvoke(Method = "GET",
                    ResponseFormat = WebMessageFormat.Json,
                    UriTemplate = "store/{url}")]
        public bool Store(string url)
        {
            string directory = ConfigurationManager.AppSettings["Directory"];

            WebPage.Store(url.Replace("@@@@", "#").Replace('*', '.').Replace('@', '/').Replace('$', '?').Replace('£', '=').Replace('€', '#'));

            return true;
        }

    }
}
