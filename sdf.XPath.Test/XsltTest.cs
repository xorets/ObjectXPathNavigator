using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Mvp.Xml.Common.Xsl;
using NUnit.Framework;
using sdf.XPath.Test.Models;

namespace sdf.XPath.Test
{
	[TestFixture, Category( "Acceptance" )]
	public class XsltTest
	{
		[Test]
		public void IdentityTransform()
		{
			XmlDocument xsltDoc = new XmlDocument();
			xsltDoc.LoadXml( @"
				<xsl:stylesheet version='1.0' 
					xmlns:xsl='http://www.w3.org/1999/XSL/Transform'>
				<xsl:output method='xml' omit-xml-declaration='no' encoding='UTF-8'/>

				<xsl:template match='@*|node()'>
					<xsl:copy>
						<xsl:apply-templates select='@*|node()'/>
					</xsl:copy>
				</xsl:template>

				</xsl:stylesheet>
			" );
			XslTransform xslt = new XslTransform();
			xslt.Load( xsltDoc );
			StringBuilder builder = new StringBuilder();
			XmlWriter writer = new XmlTextWriter( new StringWriter( builder ));

			CollectionOfSimple c = new CollectionOfSimple( true );
			ObjectXPathContext context = new ObjectXPathContext();
			context.NamespaceManager.AddNamespace( "sdf", "http://www.byte-force.com/schemas/SDF" );
			XPathNavigator nav = context.CreateNavigator( c );

			XmlDocument original = new XmlDocument();
			original.Load( nav.ReadSubtree() );
			nav.MoveToRoot();
			Console.WriteLine( original.InnerXml );
			string originalXml = original.InnerXml;

			xslt.Transform( nav, null, writer, null );
			writer.Close();

			Console.Write( builder.ToString() );

			Assert.AreEqual( originalXml, builder.ToString() );
		}

		[Test]
		public void MatchRoot()
		{
			XmlDocument xsltDoc = new XmlDocument();
			xsltDoc.LoadXml( @"
				<xsl:stylesheet version='1.0' 
					xmlns:xsl='http://www.w3.org/1999/XSL/Transform'>
				<xsl:output method='xml' omit-xml-declaration='no' encoding='UTF-8'/>

				<xsl:template match='/'><root>Matched</root></xsl:template>

				</xsl:stylesheet>
			" );
			XslTransform xslt = new XslTransform();
			xslt.Load( xsltDoc );

			StringBuilder builder = new StringBuilder();
			XmlWriter writer = new XmlTextWriter( new StringWriter( builder ) );

			CollectionOfSimple c = new CollectionOfSimple( true );
			ObjectXPathContext context = new ObjectXPathContext();
			context.DetectLoops = true;
			XPathNavigator nav = context.CreateNavigator( c );

			xslt.Transform( nav, null, writer, null );
			writer.Close();

			Assert.AreEqual( "<root>Matched</root>", builder.ToString() );
		}

		[Test]
		public void MatchRootMvpXml()
		{
			XmlDocument xsltDoc = new XmlDocument();
			xsltDoc.LoadXml( @"
				<xsl:stylesheet version='1.0' 
					xmlns:xsl='http://www.w3.org/1999/XSL/Transform'>
				<xsl:output method='xml' omit-xml-declaration='no' encoding='UTF-8'/>

				<xsl:template match='/'><root>Matched</root></xsl:template>

				</xsl:stylesheet>
			" );

			MvpXslTransform transform = new MvpXslTransform();
			var resolver = new XmlUrlResolver();

			transform.Load( xsltDoc, XsltSettings.TrustedXslt, resolver );

			StringBuilder builder = new StringBuilder();
			XmlWriter writer = new XmlTextWriter( new StringWriter( builder ) );

			CollectionOfSimple c = new CollectionOfSimple( true );
			ObjectXPathContext context = new ObjectXPathContext();
			context.DetectLoops = true;
			XPathNavigator nav = context.CreateNavigator( c );

			transform.Transform(
				new XmlInput( nav, resolver ),
				null,
				new XmlOutput( writer ) );

			writer.Close();

			Assert.AreEqual( "<root>Matched</root>", builder.ToString() );
		}
	}
}
