using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GlobalFiles
{
    public class ConfigReader
    {
        private XmlDocument xml = null;
        public ConfigReader(string fileName)
        {
            this.xml = new XmlDocument();
            xml.Load(System.IO.Path.Combine(Environment.CurrentDirectory, fileName));  
        }

        public void GetItem(string item, out Dictionary<string, List<string>> items)
        {
            items = new Dictionary<string, List<string>>();
            XmlNodeList nodes = xml.SelectSingleNode("Settings").SelectSingleNode(item).ChildNodes;
            foreach (XmlNode node in nodes)
            {
                List<string> ls = new List<string>();
                foreach (XmlNode nd in node.ChildNodes)
                {
                    ls.Add(nd.InnerText);
                }
                items[node.Name] = ls;
            }
        }
        public string GetItem(string item)
        {
            string value ="";
            XmlNodeList nodes = xml.SelectSingleNode("Settings").SelectSingleNode(item).ChildNodes;
            foreach (XmlNode node in nodes)
            {
                value = node.InnerText;
            }
            return value;
        }

    }
}
