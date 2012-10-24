using System;
using System.Xml.Serialization;

namespace sdf.XPath.Test.Models
{
	[XmlRoot( "person" )]
	public interface IPerson
	{
		[XmlElement( "name") ]
		string Name
		{
			get;
		}

		[XmlElement( "age" )]
		int Age
		{
			get;
		}
	}

	public class Person : IPerson
	{
		public string Name
		{
			get { return "John Smith"; }
		}

		public int Age
		{
			get { return 25; }
		}
	}
}
