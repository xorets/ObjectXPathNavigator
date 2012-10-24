using System.Xml;
using System.Xml.Serialization;

namespace sdf.XPath.Test.Models
{
	/// <exclude/>
	[XmlRoot( "t1" )]
	public class TwoTransparentXmlFragments
	{
		private string elem1;
		private XmlDocumentFragment _frag;
		private XmlDocumentFragment _frag2;
		private string elem3;

		public TwoTransparentXmlFragments()
		{}

		public static TwoTransparentXmlFragments Create1()
		{
			TwoTransparentXmlFragments ttxf = new TwoTransparentXmlFragments();
			ttxf.elem1 = "3";
			ttxf.elem3 = "4";
			XmlDocument doc = new XmlDocument();
			ttxf._frag = doc.CreateDocumentFragment();
			ttxf._frag.AppendChild( doc.CreateElement( "sub1-1" ) ).InnerText = "1";
			ttxf._frag.AppendChild( doc.CreateElement( "sub1-2" ) ).InnerText = "2";
			ttxf._frag2 = doc.CreateDocumentFragment();
			ttxf._frag2.AppendChild( doc.CreateElement( "sub2" ) ).InnerText = "3";
			ttxf._frag2.AppendChild( doc.CreateElement( "sub2" ) ).InnerText = "4";
			return ttxf;
		}

		public static TwoTransparentXmlFragments Create2()
		{
			TwoTransparentXmlFragments ttxf = new TwoTransparentXmlFragments();
			ttxf.elem1 = null;
			ttxf.elem3 = null;
			XmlDocument doc = new XmlDocument();
			ttxf._frag = doc.CreateDocumentFragment();
			ttxf._frag2 = doc.CreateDocumentFragment();
			ttxf._frag2.AppendChild( doc.CreateElement( "sub2" ) ).InnerText = "3";
			ttxf._frag2.AppendChild( doc.CreateElement( "sub2" ) ).InnerText = "4";
			return ttxf;
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
			get { return (XmlDocumentFragment)_frag; }
			set
			{}
		}

		[XmlAnyElement( "sub2" )]
		public XmlDocumentFragment Frag2
		{
			get { return _frag2; }
			set
			{}
		}

		[XmlElement( "child3", Namespace=Namespaces.SDF )]
		public string Elem2
		{
			get { return elem3; }
			set { elem3 = value; }
		}
	}

	/// <exclude/>
	[XmlRoot( "t1" )]
	public class TwoTransparent2
	{
		private XmlDocumentFragment _frag;
		private XmlDocumentFragment _frag2;

		public TwoTransparent2()
		{}

		public static TwoTransparent2 Create()
		{
			TwoTransparent2 ttxf = new TwoTransparent2();
			XmlDocument doc = new XmlDocument();
			ttxf._frag = doc.CreateDocumentFragment();
			ttxf._frag2 = doc.CreateDocumentFragment();
			ttxf._frag2.AppendChild( doc.CreateElement( "sub2" ) ).InnerText = "3";
			ttxf._frag2.AppendChild( doc.CreateElement( "sub2" ) ).InnerText = "4";
			return ttxf;
		}

		[XmlAnyElement]
		public XmlDocumentFragment Frag
		{
			get { return (XmlDocumentFragment)_frag; }
			set
			{}
		}

		[XmlAnyElement( "sub2" )]
		public XmlDocumentFragment Frag2
		{
			get { return _frag2; }
			set
			{}
		}
	}
}