using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WcfJsonRestService
{
    class Program
    {

        //Use this bookmark. Need to be placed in a html page. Chrome will allow you to drag and drop it. Not tested with other browser. 
        //<a href="javascript:function call(){var e=document,n=e.createElement("scr"+"ipt"),r=e.body,i=e.location;k=e.title;try{if(!r){throw 0};e.title="(Saving...) "+e.title;n.setAttribute("src",i.protocol+"//lled/storeurl/store/"+i.href.replace(/\./g,"*").replace(/\//g,"@").replace(/\?/g,"$").replace(/\=/g,"£").replace(/\#/g,"@@@@"));r.appendChild(n);}catch(s){alert("Please wait until the page has loaded.")}e.title='Saved';}call();void(0)">Bookmark Me</a>

        static void Main(string[] args)
        {

            try
            {
                using (ServiceHost host = new ServiceHost(typeof(StoreUrls)))
                {

                    AddServiceEndPoint(host, "https://{0}/storeurl", true, "lled");
                    AddServiceEndPoint(host, "http://{0}/storeurl", false);

                    host.Open();

                    Console.WriteLine("Service host running......");
                    Console.WriteLine("Press Any key at any time to exit...");
                    Console.WriteLine("Listening on");

                    foreach (ServiceEndpoint sep in host.Description.Endpoints)
                    {
                        
                        Console.WriteLine("endpoint: {0} - BindingType: {1}",
                                          sep.Address, sep.Binding.Name);
                    }


                    Console.Read();

                    host.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.Read();
            }


        }

        private static void AddServiceEndPoint(ServiceHost host, string url, bool useSSLTLS, string certSubjectName="")
        {
            string addressHttp = String.Format(url,
                System.Net.Dns.GetHostEntry("").HostName);


            WebHttpBinding binding;


            if (useSSLTLS)
            {

                binding = new WebHttpBinding(WebHttpSecurityMode.Transport);
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                binding.HostNameComparisonMode = HostNameComparisonMode.WeakWildcard;
                binding.CrossDomainScriptAccessEnabled = true;
            }
            else
            {
                binding = new WebHttpBinding(WebHttpSecurityMode.None);
                binding.CrossDomainScriptAccessEnabled = true;
            }

            // You must create an array of URI objects to have a base address.
            Uri a = new Uri(addressHttp);
            Uri[] baseAddresses = new Uri[] { a };

            WebHttpBehavior behaviour = new WebHttpBehavior();
      

            // Add an endpoint to the service. Insert the thumbprint of an X.509 
            // certificate found on your computer. 
            host.AddServiceEndpoint(typeof(IStoreUrls), binding, a).EndpointBehaviors.Add(behaviour);

            

            if (useSSLTLS)
            {
                host.Credentials.ServiceCertificate.SetCertificate(
                    StoreLocation.LocalMachine,
                    StoreName.My,
                    X509FindType.FindBySubjectName,
                    certSubjectName);
            }
        }
    }
}
