using System.Xml;
using System.Xml.Serialization;

namespace sdf.XPath.Test.Models
{
	/// <exclude/>
	[XmlRoot( "t1" )]
	public class TransparentXmlFragment
	{
		private string elem1;
		private XmlDocumentFragment _frag;
		private string elem3;

		public TransparentXmlFragment()
		{}

		public TransparentXmlFragment( bool init )
		{
			elem1 = "3";
			elem3 = "4";
			XmlDocument doc = new XmlDocument();
			_frag = doc.CreateDocumentFragment();
			XmlNode node = _frag.AppendChild( doc.CreateElement( "sub1" ) );
			node.AppendChild( doc.CreateElement( "extra" ) );
			_frag.AppendChild( doc.CreateElement( "sub2" ) ).InnerText = "2";
		}

		[XmlElement( "child1" )]
		public string Elem1
		{
			get { return elem1; }
			set { elem1 = value; }
		}

		[XmlAnyElement]
		public XmlDocumentFragment Frag
		{
			get { return _frag; }
			set { _frag = (XmlDocumentFragment)value; }
		}

		[XmlElement( "child3", Namespace=Namespaces.SDF )]
		public string Elem2
		{
			get { return elem3; }
			set { elem3 = value; }
		}
	}

}