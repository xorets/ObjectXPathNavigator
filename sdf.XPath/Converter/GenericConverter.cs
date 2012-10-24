using System;
using System.Xml;

namespace sdf.XPath
{
	internal class EmptyConverter : IConverter
	{
		public string ToString( object obj )
		{
			return string.Empty;
		}

		public object ParseString( string str )
		{
			throw new NotImplementedException();
		}
	}

	internal class GenericConverter : IConverter
	{
		public string ToString( object obj )
		{
			return obj.ToString();
		}

		public object ParseString( string str )
		{
			return null;
		}
	}

	internal class BooleanConverter : IConverter
	{
		public string ToString( object obj )
		{
			return XmlConvert.ToString( (bool)obj );
		}

		public object ParseString( string str )
		{
			return XmlConvert.ToBoolean( str );
		}
	}

	internal class DateTimeConverter : IConverter
	{
		public string ToString( object obj )
		{
			return XmlConvert.ToString( (DateTime)obj, "yyyy-MM-ddTHH:mm:sszzzzzz" );
		}

		public object ParseString( string str )
		{
			return XmlConvert.ToDateTime( str, XmlDateTimeSerializationMode.RoundtripKind );
		}
	}

	internal class SingleConverter : IConverter
	{
		public string ToString( object obj )
		{
			return XmlConvert.ToString( (Single)obj );
		}

		public object ParseString( string str )
		{
			return XmlConvert.ToSingle( str );
		}
	}

	internal class DoubleConverter : IConverter
	{
		public string ToString( object obj )
		{
			return XmlConvert.ToString( (double)obj );
		}

		public object ParseString( string str )
		{
			return XmlConvert.ToDouble( str );
		}
	}

	internal class DecimalConverter : IConverter
	{
		public string ToString( object obj )
		{
			return XmlConvert.ToString( (Decimal)obj );
		}

		public object ParseString( string str )
		{
			return XmlConvert.ToDecimal( str );
		}
	}
}