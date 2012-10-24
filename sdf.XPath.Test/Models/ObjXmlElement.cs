using System;
using System.Xml;
using System.Xml.Serialization;

namespace sdf.XPath.Test.Models
{
	[XmlRoot( "obj" )]
	public class ObjXmlElement
	{
		private XmlElement details;
		public ObjXmlElement()
		{
			XmlDocument doc = new XmlDocument();
			details = doc.CreateElement( "descr", "urn:some-namespace" );
			details.InnerXml = 
				@"<price for='kg'>$17.98</price
				><store amount='516' reserved='20'/>";
		}

		[XmlElement( "details", Namespace="http://www.namespaces.org" )]
		public XmlElement Details
		{
			get { return details; }
			set {}
		}
	}
}
