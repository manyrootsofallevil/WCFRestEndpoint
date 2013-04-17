using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WcfJsonRestService
{
    public static class WebPage
    {
        static Tuple<string, string> pageDetails = null;

        static string directory = ConfigurationManager.AppSettings["Directory"];

        public static void Store(string url)
        {
            Console.WriteLine("Storing: {0}.", url);

            XDocument xdoc = null;
            string fileName = string.Format("{0}{1:yyyyMMdd}.xml", directory, DateTime.Now);

            pageDetails = GetContent(url);

            if (File.Exists(fileName))
            {
                xdoc = XDocument.Load(fileName);
                xdoc.Root.Add(new XElement("RssItem", new XAttribute("title", pageDetails.Item1)
                , new XAttribute("content", pageDetails.Item2), new XAttribute("URI", url)));
            }
            else
            {

                xdoc = new XDocument();
                xdoc.Add(new XElement("Root"));

                xdoc.Root.Add(new XElement("RssItem", new XAttribute("title", pageDetails.Item1)
                           , new XAttribute("content", pageDetails.Item2), new XAttribute("URI", url)));

            }

            xdoc.Save(fileName);

            Console.WriteLine("Stored: {0} on {1}.", url,fileName);
        }

        private static Tuple<string, string> GetContent(string url)
        {
            HtmlWeb web = new HtmlWeb();
            string title=string.Empty, content = string.Empty;

            try
            {
                HtmlDocument docx = web.Load(url);

                var titulo = docx.DocumentNode.DescendantsAndSelf().Where(x => x.Name == "title").FirstOrDefault();

                title = titulo != null ? titulo.InnerText : string.Empty;

                var contenido = docx.DocumentNode.DescendantsAndSelf()
                    .Where(x => x.Name == "meta" && x.Attributes.Contains("name")
                        && x.Attributes.Where(y => y.Value == "description").Count() == 1)
                    .FirstOrDefault();

                content = contenido != null ?
                    contenido.Attributes.Where(x => x.Name == "content").FirstOrDefault().Value : string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception processing {0}.\nEx:{1}.",url,ex);
            }

            return new Tuple<string, string>(title, content);
        }
    }
}
