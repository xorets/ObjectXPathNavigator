using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using NUnit.Framework;

namespace sdf.XPath.Test
{
	/// <exclude/>
	public class NavigatorUtils
	{
		public static void PrintNavigator( XPathNavigator nav )
		{
			XmlWriter
				.Create( Console.OpenStandardOutput() )
				.WriteNode( nav, false );
		}

		public static void AreEqual( XPathNavigator first, XPathNavigator second )
		{
			AreEqual( first, second, false );
		}

		public static void IsSubsetOf( XPathNavigator first, XPathNavigator second )
		{
			AreEqual( first, second, true );
		}

		/// <summary>
		/// Checks if first navigator is equal to or subset of second.
		/// </summary>
		/// <param name="first">First navigator.</param>
		/// <param name="second">Second navigator.</param>
		/// <param name="checkSubset"><b>true</b> if we want to check that first 
		/// navigator and second has equal contents. <b>false</b> if contents of
		/// first iterator should be subset of second iterator's contents.</param>
		private static void AreEqual( XPathNavigator first, XPathNavigator second, bool checkSubset )
		{
			do
			{
				Assert.AreEqual( first.NodeType, second.NodeType, string.Format( "Types of nodes '{0}' and '{1}' must match.", MakePath( first ), MakePath( second ) ) );
				if( first.NodeType != XPathNodeType.Root
					&& first.NodeType != XPathNodeType.Whitespace
					&& first.NodeType != XPathNodeType.Text
					&& first.NodeType != XPathNodeType.Comment
					)
				{
					Assert.AreEqual( first.LocalName, second.LocalName, string.Format( "Names of nodes '{0}' and '{1}' must match.", MakePath( first ), MakePath( second ) ) );
					Assert.AreEqual( first.NamespaceURI, second.NamespaceURI, string.Format( "Namespaces of nodes '{0}' and '{1}' must match.", MakePath( first ), MakePath( second ) ) );
				}
				if( first.NodeType != XPathNodeType.Root &&
					first.NodeType != XPathNodeType.Element &&
					first.NodeType != XPathNodeType.Whitespace
					)
					Assert.AreEqual( first.Value, second.Value, string.Format( "Values of nodes '{0}' and '{1}' must match.", MakePath( first ), MakePath( second ) ) );

				if( MoveToFirstAttribute( first ) )
				{
					if( ( !checkSubset && !MoveToFirstAttribute( second ) )
						|| ( checkSubset && !second.MoveToAttribute( first.LocalName, first.NamespaceURI ) ) )
						throw new AssertionException( string.Format( "Missing node '{0}' in second navigator.", MakePath( first ) ) );

					do
					{
						Assert.AreEqual( first.NodeType, second.NodeType );
						Assert.AreEqual( first.LocalName, second.LocalName );
						Assert.AreEqual( first.NamespaceURI, second.NamespaceURI );
						Assert.AreEqual( first.Value, second.Value, string.Format( "Values of attributes {0}:{1} must match.", first.Prefix, first.LocalName ) );

						if( !MoveToNextAttribute( first ) )
						{
							if( !checkSubset && MoveToNextAttribute( second ) )
								throw new AssertionException( string.Format( "Extra node '{0}' in second navigator.", MakePath( second ) ) );
							break;
						}
						if( !checkSubset )
						{
							if( !MoveToNextAttribute( second ) )
								throw new AssertionException( string.Format( "Missing node '{0}' in second navigator.", MakePath( first ) ) );
						}
						else
						{
							second.MoveToParent();
							if( !second.MoveToAttribute( first.LocalName, first.NamespaceURI ) )
								throw new AssertionException( string.Format( "Missing node '{0}' in second navigator.", MakePath( first ) ) );
						}
					}
					while( true );

					first.MoveToParent();
					second.MoveToParent();
				}
				else if( !checkSubset && MoveToFirstAttribute( second ) ) 
				{
					throw new AssertionException( string.Format( "Nodes '{0}' and '{1}' must match.", MakePath( first ), MakePath( second ) ) );
				}


				/*
				* We don't care how namespaces are defined.
				* 
				if( first.MoveToFirstNamespace( XPathNamespaceScope.Local ) )
				{
					if( !second.MoveToFirstNamespace( XPathNamespaceScope.Local ) )
						throw new AssertionException( string.Format( "Nodes '{0}' and '{1}' must match.", MakePath( first ), MakePath( second ) ) );
					do
					{
						Assert.AreEqual( first.NodeType, second.NodeType );
						Assert.AreEqual( first.LocalName, second.LocalName );
						Assert.AreEqual( first.NamespaceURI, second.NamespaceURI );
						Assert.AreEqual( first.Value, second.Value );

						if( !first.MoveToNextNamespace( XPathNamespaceScope.Local ) )
						{
							if( second.MoveToNextNamespace( XPathNamespaceScope.Local ) )
								throw new AssertionException( string.Format( "Nodes '{0}' and '{1}' must match.", MakePath( first ), MakePath( second ) ) );
							break;
						}
						if( !second.MoveToNextNamespace( XPathNamespaceScope.Local ) )
							throw new AssertionException( string.Format( "Nodes '{0}' and '{1}' must match.", MakePath( first ), MakePath( second ) ) );
					}
					while( true );

					first.MoveToParent();
					second.MoveToParent();
				}
				*/

				if( first.MoveToFirstChild() )
				{
					if( !second.MoveToFirstChild() )
						throw new AssertionException( string.Format( "Missing node '{0}' in second navigator.", MakePath( first ) ) );
					if( checkSubset
						&& !CompareNodes( first, second )
						&& !FindNextElement( second, first ) )
						throw new AssertionException( string.Format( "Missing node '{0}' in second navigator.", MakePath( first ) ) );
					continue;
				}
				else if( !checkSubset && second.MoveToFirstChild() )
					throw new AssertionException( string.Format( "Nodes '{0}' and '{1}' must match.", MakePath( first ), MakePath( second ) ) );

				// Move to next
				if( first.MoveToNext() )
				{
					if( !checkSubset )
					{
						if( !second.MoveToNext() )
							throw new AssertionException( string.Format( "Nodes '{0}' and '{1}' must match.", MakePath( first ), MakePath( second ) ) );
					}
					else
					{
						// Find next subelement with corresponding name
						if( !FindNextElement( second, first ) )
							throw new AssertionException( string.Format( "Missing node '{0}' in second navigator.", MakePath( first ) ) );
					}
					continue;
				}
				else if( !checkSubset && second.MoveToNext() )
					throw new AssertionException( string.Format( "Nodes '{0}' and '{1}' must match.", MakePath( first ), MakePath( second ) ) );

				// Move up
				do
				{
					if( first.MoveToParent() )
					{
						if( !second.MoveToParent() )
							throw new AssertionException( string.Format( "Nodes '{0}' and '{1}' must match.", MakePath( first ), MakePath( second ) ) );
					}
					else
						goto Finish;

					if( first.MoveToNext() )
					{
						if( !second.MoveToNext() )
							throw new AssertionException( string.Format( "Nodes '{0}' and '{1}' must match.", MakePath( first ), MakePath( second ) ) );
						break;
					}
					else if( !checkSubset && second.MoveToNext() )
						throw new AssertionException( string.Format( "Nodes '{0}' and '{1}' must match.", MakePath( first ), MakePath( second ) ) );
				}
				while( true );
			}
			while( true );

			Finish:
			return;
		}

		private static bool MoveToFirstAttribute( XPathNavigator nav )
		{
			bool res = nav.MoveToFirstAttribute();
			if( res && nav.LocalName == "type" && nav.NamespaceURI == "http://www.w3.org/2001/XMLSchema-instance" ) 
			{
				res = nav.MoveToNextAttribute();
				if( !res )
					nav.MoveToParent();
			}
			return res;
		}

		private static bool MoveToNextAttribute( XPathNavigator nav )
		{
			bool res = nav.MoveToNextAttribute();
			if( res && nav.LocalName == "type" && nav.NamespaceURI == "http://www.w3.org/2001/XMLSchema-instance" ) 
				res = nav.MoveToNextAttribute();
			return res;
		}

		private static bool FindNextElement( XPathNavigator second, XPathNavigator first )
		{
			bool match = false;
			while( second.MoveToNext() )
			{
				match = CompareNodes( first, second );
				if( match )
					break;
			}
			return match;
		}

		private static bool CompareNodes( XPathNavigator first, XPathNavigator second )
		{
			bool match;
			if( first.NodeType == XPathNodeType.Element )
				match = first.LocalName == second.LocalName
					&& first.NamespaceURI == second.NamespaceURI;
			else
				match = first.NodeType == second.NodeType;
			return match;
		}

		private static string MakePath( XPathNavigator nav )
		{
			string res = string.Empty;
			nav = nav.Clone();
			do
			{
				res = ( nav.Prefix != string.Empty ? nav.Prefix + ":" : "" )
					+ ( nav.NodeType == XPathNodeType.Attribute ? "@" : "" )
					+ nav.LocalName
					+ ( nav.NodeType == XPathNodeType.Text ? "\"" + nav.Value + "\"" : "" )
					+ ( res != string.Empty ? "/" : "" )
					+ res;
			}
			while( nav.MoveToParent() );
			return res;
		}

		public static XPathNavigator Serialize( object o )
		{
			XmlSerializer serializer = new XmlSerializer( o.GetType() );
			StringBuilder builder = new StringBuilder();
			XmlWriter writer = new XmlTextWriter( new StringWriter( builder ) );
			XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
			xsn.Add( "sdf", "http://www.byte-force.com/schemas/SDF" );
			serializer.Serialize( writer, o );

			XmlDocument doc = new XmlDocument();
			doc.LoadXml( builder.ToString() );
			Console.WriteLine( doc.InnerXml );
			return doc.CreateNavigator();
		}

		public static object Deserialize( Type type, XPathNavigator nav )
		{
			XmlSerializer serializer = new XmlSerializer( type );
			return serializer.Deserialize( nav.ReadSubtree() );
		}
	}

}