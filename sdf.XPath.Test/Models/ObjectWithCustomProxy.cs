using System.Xml.Serialization;

namespace sdf.XPath.Test.Models
{
	/// <exclude/>
	[NodePolicy( typeof( CustomProxy ) )]
	[XmlRoot( "simple" )]
	public class ObjectWithCustomProxy
	{
		[XmlAttribute( "uhus", Namespace=Namespaces.SDF )] public int attr1;
		private string attr2;
		private string elem1;
		private string elem2;

		public ObjectWithCustomProxy()
		{}

		public ObjectWithCustomProxy( bool init )
		{
			attr1 = 1;
			attr2 = "2";
			elem1 = "3";
			elem2 = "4";
		}

		[XmlAttribute( "juja" )]
		public string Attr2
		{
			get { return attr2; }
			set { attr2 = value; }
		}

		[XmlElement( "child1" )]
		public string Elem1
		{
			get { return elem1; }
			set { elem1 = value; }
		}

		[XmlElement( "child2", Namespace=Namespaces.SDF )]
		public string Elem2
		{
			get { return elem2; }
			set { elem2 = value; }
		}
	}

	/// <exclude/>
	[XmlRoot( "custproxy" )]
	public class ObjCustomProxyAtProperty
	{
		[XmlAttribute( "a" )]
		public string a;
		private SimpleObject o;

		public ObjCustomProxyAtProperty( bool init )
		{
			a = "attribute";
			o = new SimpleObject( true );
		}

		[XmlElement( "child" )]
		[sdf.XPath.NodePolicy( typeof( CustomProxy ))]
		public SimpleObject Child
		{
			get { return o; }
			set { o = value; }
		}
	}
}