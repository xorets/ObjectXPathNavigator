using System;
using System.Xml.Serialization;

namespace sdf.XPath.Test.Models
{
	[XmlRoot( "root" )]
	public class PropertyWithFormatter
	{
		public string Name = "John Doe";
		[Converter( typeof( AgeConverter ) )] public int Age = 45;
	}

	public class AgeConverter : IConverter
	{
		public AgeConverter( Type type )
		{}

		public string ToString( object obj )
		{
			return obj.ToString() + " full years";
		}

		public object ParseString( string str )
		{
			throw new NotImplementedException();
		}
	}

}