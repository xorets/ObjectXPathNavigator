using System;
using System.Xml.Serialization;
using NUnit.Framework;

namespace sdf.XPath.Test
{
	[TestFixture, Category( "Acceptance" )]
	public class ConverterTest
	{
		private ConverterFactory _factory;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			_factory = new ConverterFactory();
		}

		[Test]
		public void TryString()
		{
			IConverter c = _factory.GetConverter( typeof( string ) );
			Assert.AreEqual( "uhus", c.ToString( "uhus" ) );
			Assert.AreEqual( null, c.ParseString( "uhus" ) );
		}

		[Test]
		public void TryBoolean()
		{
			IConverter c = _factory.GetConverter( typeof( bool ) );
			Assert.AreEqual( "true", c.ToString( true ) );
			Assert.AreEqual( "false", c.ToString( false ) );
			Assert.AreEqual( true, c.ParseString( "true" ) );
			Assert.AreEqual( false, c.ParseString( "false" ) );
		}

		[Test]
		public void TrySingle()
		{
			IConverter c = _factory.GetConverter( typeof( Single ) );
			Assert.AreEqual( "123.45", c.ToString( 123.45F ) );
			Assert.AreEqual( "123", c.ToString( 123F ) );
			Assert.AreEqual( 123.45F, c.ParseString( "123.45" ) );
			Assert.AreEqual( 123F, c.ParseString( "123" ) );
		}

		[Test]
		public void TryDouble()
		{
			IConverter c = _factory.GetConverter( typeof( Double ) );
			Assert.AreEqual( "123.45", c.ToString( 123.45D ) );
			Assert.AreEqual( "123", c.ToString( 123D ) );
			Assert.AreEqual( 123.45D, c.ParseString( "123.45" ) );
			Assert.AreEqual( 123D, c.ParseString( "123" ) );
		}

		[Test]
		public void TryDecimal()
		{
			IConverter c = _factory.GetConverter( typeof( Decimal ) );
			Assert.AreEqual( "123.45", c.ToString( 123.45M ) );
			Assert.AreEqual( "123", c.ToString( 123M ) );
			Assert.AreEqual( 123.45M, c.ParseString( "123.45" ) );
			Assert.AreEqual( 123M, c.ParseString( "123" ) );
		}

		[Test]
		public void TryDateTime()
		{
			IConverter c = _factory.GetConverter( typeof( DateTime ) );
			DateTime date = new DateTime( 2004, 1, 1, 19, 46, 13 );
			TimeSpan offset = TimeZone.CurrentTimeZone.GetUtcOffset( date );
			string dStr = string.Format( "2004-01-01T19:46:13{0:+00;-00;00}:{1:00}", offset.Hours, offset.Minutes );
			Assert.AreEqual( dStr, c.ToString( date ) );
			Assert.IsTrue( date.CompareTo( c.ParseString( dStr ) ) == 0 );
		}

		[Test]
		public void TrySdfRelationType()
		{
			IConverter c = _factory.GetConverter( typeof( SdfRelationType ) );
			Assert.AreEqual( "parent-child", c.ToString( SdfRelationType.ParentChild ) );
			Assert.AreEqual( SdfRelationType.ParentChild, c.ParseString( "parent-child" ) );
		}

		[Test]
		public void TryTypeWithConverterTypeAttr()
		{
			IConverter c = _factory.GetConverter( typeof( SampleConvertible ) );
			Assert.AreEqual( "77", c.ToString( new SampleConvertible( 77 ) ) );
		}

		[Test]
		public void TryDerivedTypeWithoutOwnAttr()
		{
			IConverter c = _factory.GetConverter( typeof( DerivedSampleConvertible ) );
			Assert.AreEqual( "123", c.ToString( new DerivedSampleConvertible() ) );
		}

		[Test]
		public void TryDerivedTypeWithOwnAttr()
		{
			IConverter c = _factory.GetConverter( typeof( DerivedSampleConvertibleOwnConverter ) );
			Assert.AreEqual( "123...", c.ToString( new DerivedSampleConvertibleOwnConverter() ) );
		}

		[Test]
		public void TrySampleConvertibleImpl()
		{
			IConverter c = _factory.GetConverter( typeof( SampleConvertibleImpl ) );
			Assert.AreEqual( "515", c.ToString( new SampleConvertibleImpl() ) );
		}

		[Test]
		public void ReplaceDatetimeConverter()
		{
			ConverterFactory f = new ConverterFactory();
			f.RemoveConverter( typeof( DateTime ) );
			f.AddConverter( typeof( DateTime ), new ConverterStub( null ) );
			IConverter c = f.GetConverter( typeof( DateTime ) );
			DateTime now = DateTime.Now;
			Assert.AreEqual( "Stub: " + now.ToString(), c.ToString( now ) );
		}

		[Test()]
		public void CheckConverterHashing()
		{
			IConverter c1 = _factory.GetConverter( typeof( SampleConverter1 ), typeof( SampleConvertible ) );
			IConverter c2 = _factory.GetConverter( typeof( SampleConverter1 ), typeof( SampleConvertible ) );
			Assert.AreSame( c1, c2, "Must be the same converter instance." );
			IConverter c3 = _factory.GetConverter( typeof( SampleConverter1 ), typeof( DerivedSampleConvertible ) );
			Assert.IsFalse( c2 == c3, "Must be another converter instance." );
		}
	}

	/* -------------------------------------------------- */

	/// <exclude/>
	internal enum SdfRelationType
	{
		[XmlEnum( "parent-child" )] ParentChild = 1,

		[XmlEnum( "link" )] Link = 2,

		[XmlEnum( "aggregate" )] Aggregate = 3
	}


	/* Sample convertible types */

	/// <exclude/>
	[Converter( typeof( SampleConverter1 ) )]
	internal class SampleConvertible
	{
		public int Value;

		public SampleConvertible( int value )
		{
			Value = value;
		}
	}

	/// <exclude/>
	internal class DerivedSampleConvertible : SampleConvertible
	{
		public DerivedSampleConvertible()
			: base( 123 )
		{}
	}

	/// <exclude/>
	[Converter( typeof( SampleConverter2 ) )]
	internal class DerivedSampleConvertibleOwnConverter : SampleConvertible
	{
		public DerivedSampleConvertibleOwnConverter()
			: base( 123 )
		{}
	}

	/// <exclude/>
	[Converter( typeof( SampleConverter3 ) )]
	internal interface ISampleConvertible
	{
		int Value { get; }
	}

	/// <exclude/>
	internal class SampleConvertibleImpl : ISampleConvertible
	{
		public int Value
		{
			get { return 515; }
		}
	}

	/* Converters */

	/// <exclude/>
	internal class ConverterStub : IConverter
	{
		public ConverterStub( Type type )
		{}

		public string ToString( object obj )
		{
			return "Stub: " + obj.ToString();
		}

		public object ParseString( string str )
		{
			return null;
		}
	}

	/// <exclude/>
	internal class SampleConverter1 : IConverter
	{
		public SampleConverter1( Type type )
		{}

		public string ToString( object obj )
		{
			return ( (SampleConvertible)obj ).Value.ToString();
		}

		public object ParseString( string str )
		{
			throw new NotImplementedException();
		}
	}

	/// <exclude/>
	internal class SampleConverter2 : IConverter
	{
		public SampleConverter2( Type type )
		{}

		public string ToString( object obj )
		{
			return ( (SampleConvertible)obj ).Value.ToString() + "...";
		}

		public object ParseString( string str )
		{
			throw new NotImplementedException();
		}
	}

	/// <exclude/>
	internal class SampleConverter3 : IConverter
	{
		public SampleConverter3( Type type )
		{}

		public string ToString( object obj )
		{
			return ( (ISampleConvertible)obj ).Value.ToString();
		}

		public object ParseString( string str )
		{
			throw new NotImplementedException();
		}
	}
}