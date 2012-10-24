using System;
using System.Xml;
using System.Xml.Serialization;

namespace sdf.XPath.Test.Models
{
	[XmlRoot( "skip-attr" )]
	public class ObjSkipNavRoot
	{
		public ObjSkipNavRoot( bool init )
		{
			Name = "Jim";
			XmlDocument doc = new XmlDocument();
			doc.LoadXml( @"<root><details about='me'/></root>" );
			Details = doc.DocumentElement;
		}

		public string Name;

		[SkipNavigableRoot]
		public XmlElement Details;
	}
}
