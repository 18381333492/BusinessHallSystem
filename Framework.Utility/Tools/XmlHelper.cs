using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Framework.Utility.Tools
{
    public class XmlHelper
    {

        public static string getTextByNode(string sXmlContent, string sName)
        {
            XElement xElement = XElement.Parse(sXmlContent);
            var xResultElement = xElement.Element(sName);
            return xResultElement == null ? string.Empty : xResultElement.Value;
        }
    }
}
