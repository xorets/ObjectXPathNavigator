using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace sdf.XPath.Test.Models
{
	[XmlRoot( "root", Namespace="root-ns" )]
	[XmlInclude( typeof( SimpleObject ))]
	public class ObjDefaultNsAnyElement
	{
		private XmlElement[] _list;
		public string Child;

		public ObjDefaultNsAnyElement()
		{
		}

		public ObjDefaultNsAnyElement( bool init )
		{
			Child = "child value";

			XmlDocument doc = new XmlDocument();
			_list = new XmlElement[3];

			XmlElement e;
			e = doc.CreateElement( "elem1" );
			e.InnerText = "value1";
			_list[0] = e;
			e = doc.CreateElement( "elem2", "root-ns" );
			e.InnerText = "value2";
			_list[1] = e;
			e = doc.CreateElement( "elem3" );
			e.InnerText = "value3";
			_list[2] = e;
		}

		public XmlElement Elem0
		{
			get { return _list[0]; }
			set { _list[0] = value; }
		}

		[XmlAnyElement( "elem2" )]
		public XmlElement Elem1
		{
			get { return _list[1]; }
			set { _list[1] = value; }
		}

		[XmlAnyElement()]
		public XmlElement[] Dic
		{
			get { return _list; }
			set { _list = value; }
		}
	}
}
