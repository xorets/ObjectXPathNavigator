using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using NUnit.Framework;
using sdf.XPath.Test.Models;

namespace sdf.XPath.Test
{
	[TestFixture]
	[Ignore( "Run only when really needed." )]
	public class PerformanceTest
	{
		[Test]
		public void LoadInDocument()
		{
			CollectionOfSimple c = new CollectionOfSimple( true );
			ObjectXPathContext context = new ObjectXPathContext();
			context.NamespaceManager.AddNamespace( "sdf", Namespaces.SDF );
			XPathNavigator nav = context.CreateNavigator( c );
			for( int i=0; i<100000; i++ ) 
			{
				nav.MoveToRoot();
				XmlDocument original = new XmlDocument();
				original.Load( nav.ReadSubtree() );
			}
		}

		[Test]
		public void SelectAllNodes()
		{
			// Generate simple objects
			var objects = new List<SimpleObject>();
			int totalIterations = 100000;
			for( int i=0; i<totalIterations; i++ )
			{
				SimpleObject o = new SimpleObject();
				o.Attr2 = i.ToString();
				o.Elem1 = "Elem1" + i;
				o.Elem2 = "Elem2" + i;
				objects.Add( o );
			}

			// Create navigator
			var context = new ObjectXPathContext();
			context.NamespaceManager.AddNamespace( "sdf", Namespaces.SDF );
			XPathNavigator nav = context.CreateNavigator( objects );

			// Select nodes
			var nodes = nav.Select( "//* | //@*" );
			int j = 0;
			while( nodes.MoveNext() )
			{
				var val = nodes.Current.Value;
				j++;
			}

			Assert.AreEqual( totalIterations * 5 + 1, j );
		}

		[Test]
		public void Xslt()
		{
			XmlDocument xsltDoc = new XmlDocument();
			xsltDoc.LoadXml( @"
				<xsl:stylesheet version='1.0' 
					xmlns:xsl='http://www.w3.org/1999/XSL/Transform'>
				<xsl:output method='xml' omit-xml-declaration='no' encoding='UTF-8'/>

				<xsl:template match='@*|node()'>
					<xsl:copy>
						<xsl:apply-templates select='@*'/>
						<xsl:apply-templates select='node()'/>
					</xsl:copy>
				</xsl:template>

				</xsl:stylesheet>
			" );
			XslTransform xslt = new XslTransform();
			xslt.Load( xsltDoc );

			CollectionOfSimple c = new CollectionOfSimple( true );
			ObjectXPathContext context = new ObjectXPathContext();
			context.NamespaceManager.AddNamespace( "sdf", Namespaces.SDF );
			XPathNavigator nav = context.CreateNavigator( c );

			for( int i=0; i<100000; i++ ) 
			{
				StringBuilder builder = new StringBuilder();
				XmlWriter writer = new XmlTextWriter( new StringWriter( builder ));

				XsltArgumentList xslArg = new XsltArgumentList();
				nav.MoveToRoot();
				xslt.Transform( nav, xslArg, writer, null );
				writer.Close();
			}
		}
	}
}
