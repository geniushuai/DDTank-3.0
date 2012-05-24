using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace Bussiness
{
    public static class XmlExtends
    {
        public static string ToString(this XElement node, bool check)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.CheckCharacters = check;
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            using (XmlWriter xw = XmlWriter.Create(sb, xws))
            {
                node.WriteTo(xw);
            }

            return sb.ToString();
        }
    }
}
