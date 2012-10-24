using System.Xml;
using NUnit.Framework;

namespace sdf.XPath.Test
{
	[TestFixture, Category( "Acceptance" )]
	public class NavigatorUtilsTest
	{
		[Test]
		public void EqualNavigators()
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml( @"
				<root xmlns:u='uuu'>
					<u:element1 attr1='1' attr2='b'>
						<subelement1>uhus</subelement1>
						<!-- uhus was here -->
					</u:element1>
				</root>
				<!-- and here -->
			" );

			NavigatorUtils.AreEqual( doc.CreateNavigator(), doc.CreateNavigator() );
		}

		[Test]
		[ExpectedException( typeof( AssertionException ) )]
		public void DiffrentValue()
		{
			XmlDocument doc1 = new XmlDocument();
			doc1.LoadXml( @"
				<root>
					<element1 attr1='1' attr2='b'>
						<subelement1>uhus</subelement1>
					</element1>
				</root>
			" );

			XmlDocument doc2 = new XmlDocument();
			doc2.LoadXml( @"
				<root>
					<element1 attr1='1' attr2='b'>
						<subelement1>juja</subelement1>
					</element1>
				</root>
			" );

			NavigatorUtils.AreEqual( doc1.CreateNavigator(), doc2.CreateNavigator() );
		}

		[Test]
		[ExpectedException( typeof( AssertionException ) )]
		public void DiffrentNamespaces()
		{
			XmlDocument doc1 = new XmlDocument();
			doc1.LoadXml( @"
				<root xmlns:u='uuu'>
					<u:element1 attr1='1' attr2='b'>
						<subelement1>uhus</subelement1>
					</u:element1>
				</root>
			" );

			XmlDocument doc2 = new XmlDocument();
			doc2.LoadXml( @"
				<root xmlns:u='uuu1'>
					<u:element1 attr1='1' attr2='b'>
						<subelement1>uhus</subelement1>
					</u:element1>
				</root>
			" );

			NavigatorUtils.AreEqual( doc1.CreateNavigator(), doc2.CreateNavigator() );
		}

		[Test]
		[ExpectedException( typeof( AssertionException ) )]
		public void NoSubelement()
		{
			XmlDocument doc1 = new XmlDocument();
			doc1.LoadXml( @"
				<root>
					<element1 attr1='1' attr2='b'>
						<subelement1>uhus</subelement1>
					</element1>
				</root>
			" );

			XmlDocument doc2 = new XmlDocument();
			doc2.LoadXml( @"
				<root>
					<element1 attr1='1' attr2='b'>
					</element1>
				</root>
			" );

			NavigatorUtils.AreEqual( doc1.CreateNavigator(), doc2.CreateNavigator() );
		}

		[Test]
		[ExpectedException( typeof( AssertionException ) )]
		public void NoAttribute()
		{
			XmlDocument doc1 = new XmlDocument();
			doc1.LoadXml( @"
				<root>
					<element1 attr1='1' attr2='b'>
						<subelement1>uhus</subelement1>
					</element1>
				</root>
			" );

			XmlDocument doc2 = new XmlDocument();
			doc2.LoadXml( @"
				<root>
					<element1 attr1='1'>
						<subelement1>uhus</subelement1>
					</element1>
				</root>
			" );

			NavigatorUtils.AreEqual( doc1.CreateNavigator(), doc2.CreateNavigator() );
		}

		[Test]
		[ExpectedException( typeof( AssertionException ) )]
		public void AttrValue()
		{
			XmlDocument doc1 = new XmlDocument();
			doc1.LoadXml( @"
				<root>
					<element1 attr1='1'>
						<subelement1>uhus</subelement1>
					</element1>
				</root>
			" );

			XmlDocument doc2 = new XmlDocument();
			doc2.LoadXml( @"
				<root>
					<element1 attr1='2'>
						<subelement1>uhus</subelement1>
					</element1>
				</root>
			" );

			NavigatorUtils.AreEqual( doc1.CreateNavigator(), doc2.CreateNavigator() );
		}

		[Test]
		[ExpectedException( typeof( AssertionException ) )]
		public void ExtraSubelement()
		{
			XmlDocument doc1 = new XmlDocument();
			doc1.LoadXml( @"
				<root>
					<element1 attr1='1' attr2='b'>
						<subelement1>uhus</subelement1>
						<!-- uhus was here -->
					</element1>
				</root>
			" );

			XmlDocument doc2 = new XmlDocument();
			doc2.LoadXml( @"
				<root>
					<element1 attr1='1' attr2='b'>
						<subelement1>uhus</subelement1>
						<subelement2 a='a'/>
					</element1>
				</root>
			" );

			NavigatorUtils.AreEqual( doc1.CreateNavigator(), doc2.CreateNavigator() );
		}

		[Test]
		[ExpectedException( typeof( AssertionException ) )]
		public void ExtraSubSubelement()
		{
			XmlDocument doc1 = new XmlDocument();
			doc1.LoadXml( @"
				<root>
					<element1 attr1='1' attr2='b'>
						<subelement1>uhus</subelement1>
					</element1>
				</root>
			" );

			XmlDocument doc2 = new XmlDocument();
			doc2.LoadXml( @"
				<root>
					<element1 attr1='1' attr2='b'>
						<subelement1>uhus
							<subelement2 a='a'/>
						</subelement1>
					</element1>
				</root>
			" );

			NavigatorUtils.AreEqual( doc1.CreateNavigator(), doc2.CreateNavigator() );
		}

		[Test]
		[ExpectedException( typeof( AssertionException ) )]
		public void ExtraAttribute()
		{
			XmlDocument doc1 = new XmlDocument();
			doc1.LoadXml( @"
				<root>
					<element1 attr1='1' attr2='b'>
						<subelement1>uhus</subelement1>
					</element1>
				</root>
			" );

			XmlDocument doc2 = new XmlDocument();
			doc2.LoadXml( @"
				<root>
					<element1 attr1='1' attr2='b' attr3='fsfwfwfw'>
					</element1>
				</root>
			" );

			NavigatorUtils.AreEqual( doc1.CreateNavigator(), doc2.CreateNavigator() );
		}

		[Test]
		[ExpectedException( typeof( AssertionException ) )]
		public void NoAttributesInFirst()
		{
			XmlDocument doc1 = new XmlDocument();
			doc1.LoadXml( @"
				<root>
					<element1>
						<subelement1>uhus</subelement1>
					</element1>
				</root>
			" );

			XmlDocument doc2 = new XmlDocument();
			doc2.LoadXml( @"
				<root>
					<element1 attr1='1'>
						<subelement1>uhus</subelement1>
					</element1>
				</root>
			" );

			NavigatorUtils.AreEqual( doc1.CreateNavigator(), doc2.CreateNavigator() );
		}

		[Test]
		[ExpectedException( typeof( AssertionException ) )]
		public void NoElementInFirst()
		{
			XmlDocument doc1 = new XmlDocument();
			doc1.LoadXml( @"
				<root>
				</root>
			" );

			XmlDocument doc2 = new XmlDocument();
			doc2.LoadXml( @"
				<root>
					<element1 attr1='1'>
						<subelement1>uhus</subelement1>
					</element1>
				</root>
			" );

			NavigatorUtils.AreEqual( doc1.CreateNavigator(), doc2.CreateNavigator() );
		}

		[Test]
		public void Subset()
		{
			XmlDocument doc1 = new XmlDocument();
			doc1.LoadXml( @"
				<root>
					<element1 attr2='2'>
						<uhus/>
					</element1>
				</root>
			" );

			XmlDocument doc2 = new XmlDocument();
			doc2.LoadXml( @"
				<root>
					<element1 attr1='1' attr2='2'>
						<subelement1>uhus</subelement1>
						<uhus/>
					</element1>
				</root>
			" );

			NavigatorUtils.IsSubsetOf( doc1.CreateNavigator(), doc2.CreateNavigator() );
		}

	}
}