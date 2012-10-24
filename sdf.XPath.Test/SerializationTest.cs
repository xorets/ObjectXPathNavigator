using System;
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;
using sdf.XPath.Test.Models;

namespace sdf.XPath.Test
{
	/// <exclude/>
	[TestFixture]
	public class SerializationTest
	{
		[Test]
		public void SerializeSimple()
		{
			SimpleObject o = new SimpleObject( true );
			SerializeAndCompare( o );
		}

		[Test]
		public void SerializeTransparent()
		{
			TransparentXmlFragment o = new TransparentXmlFragment( true );
			SerializeAndCompare( o );
		}

		[Test]
		public void SerializeTwoTransparent()
		{
			TwoTransparentXmlFragments o = TwoTransparentXmlFragments.Create1();
			SerializeAndCompare( o );
		}

		[Test]
		public void SerializeObjXmlElement()
		{
			ObjXmlElement o = new ObjXmlElement();
			SerializeAndCompare( o );
		}

		[Test]
		[Ignore( "Our requirements differ from standard serialization ones." )]
		public void SerializeProduct()
		{
			Product o = Product.Create();
			SerializeAndCompare( o );
		}


		[Test]
		public void SerializeObjChildrenNS()
		{
			SerializeAndCompare( new ObjChildrenNS( true ) );
		}

		[Test]
		public void SerializeObjChildrenNSTypeAttr()
		{
			SerializeAndCompare( new ObjChildrenNSTypeAttr( true ) );
		}

		[Test]
		public void SerializeObjChildrenNSInherited()
		{
			SerializeAndCompare( new ObjChildrenNSInherit( true ) );
		}

		[Test]
		public void SerializeListProperty()
		{
			ObjListProperty o = new ObjListProperty( true );
			SerializeAndCompare( o );
		}

		[Test]
		public void SerializeDefaultNsAnyElement()
		{
			ObjDefaultNsAnyElement o = new ObjDefaultNsAnyElement( true );
			SerializeAndCompare( o );
		}

		[Test]
		public void SerializeArrays()
		{
			ObjTwoArrays o = new ObjTwoArrays( true );
			SerializeAndCompare( o );
		}

		[Test, Ignore]
		public void SerializeEnumerableProperty()
		{
			ObjEnumerableProperty o = new ObjEnumerableProperty( true );
			SerializeAndCompare( o );
		}

		private static void DeserializeAndCompare( object o )
		{
			ObjectXPathContext context = new ObjectXPathContext();
			context.NamespaceManager.AddNamespace( "sdf", Namespaces.SDF );

			XPathNavigator nav = context.CreateNavigator( o );
			nav.MoveToRoot();

			NavigatorUtils.PrintNavigator( nav );

			nav.MoveToRoot();
			//			object o2 = Deserialize( o.GetType(), nav );
			object o2 = NavigatorUtils.Deserialize( o.GetType(), NavigatorUtils.Serialize( o ) );

			XPathNavigator nav2 = context.CreateNavigator( o2 );

			nav2.MoveToRoot();
			nav.MoveToRoot();

			NavigatorUtils.AreEqual( nav, nav2 );
		}

		private static void SerializeAndCompare( object o )
		{
			XPathNavigator nav1 = NavigatorUtils.Serialize( o );

			ObjectXPathContext context = new ObjectXPathContext();
			context.NamespaceManager.AddNamespace( "sdf", Namespaces.SDF );
			XPathNavigator nav2 = context.CreateNavigator( o );
			NavigatorUtils.PrintNavigator( nav2 );
			nav2.MoveToRoot();

			NavigatorUtils.AreEqual( nav2, nav1 );
		}

	}
}