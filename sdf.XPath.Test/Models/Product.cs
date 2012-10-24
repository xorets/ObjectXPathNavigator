using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace sdf.XPath.Test.Models
{
	/// <exclude/>
	[XmlRoot( "product", Namespace=Namespaces.SDF )]
	public class Product
	{
		private int _productId;
		private string _name;
		private string _description;
		private DateTime _date;
		private XmlElement _xml;

		public Product()
		{}

		public static Product Create()
		{
			Product p = new Product();
			p._productId = 7;
			p._name = "Red button";
			p._description = "Real RED button.";
			p._date = new DateTime( 2004, 8, 18, 19, 46, 13 );

			XmlDocument doc = new XmlDocument();
			p._xml = doc.CreateElement( "fragment" );
			p._xml.InnerXml =
				@"<comments>Very powerful tool for a government usage.</comments><restrict-to><president/><int-affair/></restrict-to>";
			return p;
		}

		[XmlAttribute( "id" )]
		public int ProductId
		{
			get { return _productId; }
			set { _productId = value; }
		}

		[XmlAttribute( "name" )]
		public string Field1
		{
			get { return _name; }
			set { _name = value; }
		}

		[XmlElement( "description" )]
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		[XmlElement( "Date", Form=XmlSchemaForm.Unqualified )]
		[Converter( typeof( SimpleConverter ))]
		public DateTime Date
		{
			get { return _date; }
			set { _date = value; }
		}

		[XmlAnyElement]
		public XmlElement Xml
		{
			get { return _xml; }
			set { _xml = value; }
		}
	}
}