using System;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using System.Xml.Serialization;

namespace sdf.XPath.Test.Models
{
	/// <exclude/>
	[XmlRoot( "simple" )]
	public class ListWithNullReference
	{
		private IList _array;

		public ListWithNullReference( bool init )
		{
			_array = new object[1];
		}

		[XmlElement( "entries" )]
		[Transparent( false )]
		public IList Array
		{
			get { return _array; }
			set { _array = value; }
		}
	}

	/// <exclude/>
	[XmlRoot( "simple" )]
	public class DictionaryWithNullReference
	{
		private IDictionary _dictonary;

		public DictionaryWithNullReference( bool init )
		{
			_dictonary = new ListDictionary();
			_dictonary.Add( 1, null );
		}

		[XmlElement( "entries" )]
		public IDictionary Array
		{
			get { return _dictonary; }
			set { _dictonary = value; }
		}
	}

	/// <exclude/>
	[XmlRoot( "simple" )]
	public class ExceptionsInMembers
	{
		public ExceptionsInMembers( bool init )
		{}

		[XmlElement( "array" )]
		public IDictionary Array
		{
			get { throw new NullReferenceException( "No array here." ); }
		}

		[XmlElement( "string" )]
		public string Str
		{
			get { throw new NotImplementedException(); }
		}
	}

	[XmlRoot( "obj",
		 Namespace="http://emsinet.com/schemas/Order/Data" )]
	public class ObjNullStringProperty
	{
		private string clientOrderId_ = null;
		private string submittedDate_ = "2005-01-13T13:00:00+03:00";

		[XmlAttribute("ClientOrderId")] 
		public string ClientOrderId3
		{
			get { return clientOrderId_; }
			set { clientOrderId_ = value; }
		}
 
		[XmlElement("ClientOrderId",
			 Namespace="http://emsinet.com/schemas/Order/Data",
			 IsNullable=true )] 
		public string ClientOrderId 
		{
			get { return clientOrderId_; }
			set { clientOrderId_ = value; }
		}
 
		[XmlElement("ClientOrderId2",
			 Namespace="http://emsinet.com/schemas/Order/Data",
			 IsNullable=false )] 
		public string ClientOrderId2
		{
			get { return clientOrderId_; }
			set { clientOrderId_ = value; }
		}
 
		[XmlElement("SubmittedDate",
			 Namespace="http://emsinet.com/schemas/Order/Data")]
		public string SubmittedDate
		{
			get { return submittedDate_; } 
			set { submittedDate_ = value; }
		}
	}
}