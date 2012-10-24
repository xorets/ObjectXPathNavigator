using System;
using System.Collections;
using System.Xml.Serialization;

namespace sdf.XPath.Test.Models
{
	[XmlRoot( "enumerable" )]
	public class ObjEnumerableProperty
	{
		private string[] _children;

		public ObjEnumerableProperty()
		{
		}

		public ObjEnumerableProperty( bool init )
		{
			_children = new string[] { "red", "green", "blue" };	
		}

		[XmlAnyElement]
		public EnumerableChildren Children
		{
			get{ return new EnumerableChildren( _children ); }
		}
	}

	public class EnumerableChildren : IEnumerable
	{
		IEnumerable target;

		public EnumerableChildren( IEnumerable target )
		{
			this.target = target;	
		}

		public IEnumerator GetEnumerator()
		{
			return target.GetEnumerator();
		}

		public void Add( object o ){}
	}

}
