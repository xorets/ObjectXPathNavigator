using System;
using System.Collections;
using System.Xml.Serialization;

namespace sdf.XPath.Test.Models
{
	[XmlRoot( "col" )]
	[XmlInclude( typeof( SimpleObject ))]
	[XmlInclude( typeof( ObjChildrenNSInherit ))]
	public class ObjListProperty
	{
		private IList _list;
		public string Child;

		public ObjListProperty()
		{
		}

		public ObjListProperty( bool init )
		{
			Child = "child value";

			_list = new ArrayList();
			_list.Add( "string1" );
			SimpleObject o;
			o = new SimpleObject( true );
			o.attr1 = 11;
			_list.Add( o );
			o = new SimpleObject( true );
			o.attr1 = 12;
			_list.Add( o );
			_list.Add( new ObjChildrenNSInherit( true ) );
		}

		[XmlElement( "list" )]
		public IList List1
		{
			get { return _list; }
			set { _list = value; }
		}

/*
		public IList List2
		{
			get { return _list; }
			set { _list = value; }
		}

		public string[] Strings
		{
			get { return new string[] { "first-str", "second-str" };}
			set {}
		}

		[XmlElement( "coll", Type=typeof( ArrayList ) )]
		public ICollection Collection
		{
			get { return _list; }
			set { _list = (IList)value; }
		}
*/

/*
		[XmlArray( "array" )]
		[XmlArrayItem( typeof( string ) )]
		[XmlArrayItem( "simple", typeof( SimpleObject ) )]
        public IList List2
		{
			get { return _list; }
			set { _list = value; }
		}
*/
	}	

	public class ObjTwoArrays
	{
		private string[] _a1;
		private string[] _a2;

		public ObjTwoArrays()
		{}

		public ObjTwoArrays( bool init )
		{
			_a1 = new string[] { "value1", "value2" };
			_a2 = new string[] { "value3", "value4" };
		}

		[XmlElement( "array1" )]
		public string[] A1
		{
			get { return _a1; }
			set { _a1 = value; }
		}

/*
		public string[] A2
		{
			get { return _a2; }
			set { _a2 = value; }
		}
*/
	}
}

