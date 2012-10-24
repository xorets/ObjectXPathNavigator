using System.Collections;
using System.Collections.Specialized;
using System.Xml.Serialization;

namespace sdf.XPath.Test.Models
{
	/// <exclude/>
	[XmlRoot( "simple" )]
	public class SimpleObject
	{
		[XmlAttribute( "uhus", Namespace=Namespaces.SDF )] 
		public int attr1;
		private string attr2;
		private string elem1;
		private string elem2;

		public SimpleObject()
		{}

		public SimpleObject( bool init )
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
	[XmlRoot( "col" )]
	public class CollectionOfSimple
	{
		private IDictionary _dic;

		public CollectionOfSimple() {}

		public CollectionOfSimple( bool init )
		{
			_dic = new ListDictionary();
			SimpleObject o;
			o = new SimpleObject( true );
			o.attr1 = 11;
			_dic.Add( "1", o );
			o = new SimpleObject( true );
			o.attr1 = 12;
			_dic.Add( "2", o );
			o = new SimpleObject( true );
			o.attr1 = 13;
			_dic.Add( "3", o );
		}

		[XmlElement( "entries" )]
		public IDictionary Dic
		{
			get { return _dic; }
			set { _dic = value; }
		}
	}
}