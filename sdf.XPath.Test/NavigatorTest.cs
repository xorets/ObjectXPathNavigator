using System;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using NUnit.Framework;
using sdf.XPath.Test.Models;

namespace sdf.XPath.Test
{
	[TestFixture, Category( "Acceptance" )]
	public class NavigatorTest
	{
		public static void Compare( object o, string xml )
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml( xml );

			ObjectXPathContext context = new ObjectXPathContext();
			context.NamespaceManager.AddNamespace( "sdf", Namespaces.SDF );
			XPathNavigator nav = context.CreateNavigator( o );
			nav.MoveToRoot();

			/*NavigatorUtils.PrintNavigator( nav );

			nav.MoveToRoot();*/

			NavigatorUtils.AreEqual( nav, doc.CreateNavigator() );
		}

		public static void IsSubset( object o, string xml )
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml( xml );

			ObjectXPathContext context = new ObjectXPathContext();
			context.NamespaceManager.AddNamespace( "sdf", Namespaces.SDF );
			XPathNavigator nav = context.CreateNavigator( o );
			nav.MoveToRoot();

			NavigatorUtils.IsSubsetOf(  doc.CreateNavigator(), nav );
		}

		private static string ConvertDate( DateTime date )
		{
			TimeSpan offset = TimeZone.CurrentTimeZone.GetUtcOffset( date );
			return string.Format( "{0}{1:+00;-00;00}:{2:00}", 
				XmlConvert.ToString( date, "yyyy-MM-ddTHH:mm:ss" ),
				offset.Hours, offset.Minutes );
		}

		[Test]
		public void SimpleObject()
		{
			Compare( new SimpleObject( true ), @"
				<simple sdf:uhus='1' juja='2' xmlns:sdf='http://www.byte-force.com/schemas/SDF'>
					<child1>3</child1>
					<sdf:child2>4</sdf:child2>
				</simple>
			" );
		}

		[Test]
		public void CollectionOfSimple()
		{
			Compare( new CollectionOfSimple( true ), @"
				<col>
					<entries>
						<entries sdf:uhus='11' juja='2' xmlns:sdf='http://www.byte-force.com/schemas/SDF'>
							<child1>3</child1>
							<sdf:child2>4</sdf:child2>
						</entries>
						<entries sdf:uhus='12' juja='2' xmlns:sdf='http://www.byte-force.com/schemas/SDF'>
							<child1>3</child1>
							<sdf:child2>4</sdf:child2>
						</entries>
						<entries sdf:uhus='13' juja='2' xmlns:sdf='http://www.byte-force.com/schemas/SDF'>
							<child1>3</child1>
							<sdf:child2>4</sdf:child2>
						</entries>
					</entries>
				</col>
			" );
		}

		[Test]
		public void TransparentElement()
		{
			Compare( new TransparentXmlFragment( true ), @"
				<t1 xmlns:sdf='http://www.byte-force.com/schemas/SDF'>
					<child1>3</child1>
					<sub1><extra/></sub1>
					<sub2>2</sub2>
					<sdf:child3>4</sdf:child3>
				</t1>
			" );
		}

		[Test]
		public void TwoTransparent()
		{
			Compare( TwoTransparentXmlFragments.Create1(), @"
				<t1 xmlns:sdf='http://www.byte-force.com/schemas/SDF'>
					<child1>3</child1>
					<sub1-1>1</sub1-1>
					<sub1-2>2</sub1-2>
					<sub2>3</sub2>
					<sub2>4</sub2>
					<sdf:child3>4</sdf:child3> 
				</t1>
			" );
		}

		[Test]
		public void TwoTransparentFirstEmpty()
		{
			Compare( TwoTransparent2.Create(), @"
				<t1 xmlns:sdf='http://www.byte-force.com/schemas/SDF'>
					<sub2>3</sub2>
					<sub2>4</sub2>
				</t1>
			" );
		}

		[Test]
		public void TryProduct()
		{
			DateTime date = new DateTime( 2004, 8, 18, 19, 46, 13 );
			//string dStr = ConvertDate( date );
			string dStr = XmlConvert.ToString( date );

			Compare( Product.Create(), @"
				<sdf:product id='7' name='Red button' xmlns:sdf='http://www.byte-force.com/schemas/SDF'>
					<sdf:description>Real RED button.</sdf:description>
					<Date>" + dStr + @"</Date>
					<fragment>
						<comments>Very powerful tool for a government usage.</comments>
						<restrict-to>
							<president/>
							<int-affair/>
						</restrict-to>
					</fragment>
				</sdf:product>
			" );
		}

		[Test]
		public void CustomPolicyRoot()
		{
			Compare( new ObjectWithCustomProxy( true ), @"
				<simple sdf:uhus='1' juja='2' xmlns:sdf='http://www.byte-force.com/schemas/SDF'>
					<child1>3</child1>
					<sdf:child2>4</sdf:child2>Uhus sux!</simple>
			" );
		}

		[Test]
		public void CustomPolicyProperty()
		{
			Compare( new ObjCustomProxyAtProperty( true ), @"
				<custproxy a='attribute'><simple sdf:uhus='1' juja='2' xmlns:sdf='http://www.byte-force.com/schemas/SDF'>
					<child1>3</child1>
					<sdf:child2>4</sdf:child2>Uhus sux!</simple></custproxy>
			" );
		}

		[Test]
		public void CustomPropertyConverter()
		{
			Compare( new PropertyWithFormatter(), @"
				<root>
					<Name>John Doe</Name>
					<Age>45 full years</Age>
				</root>
			" );
		}

		[Test]
		public void ListWithNullReference()
		{
			Compare( new ListWithNullReference( true ), @"
				<simple>
					<entries><!--Null reference--></entries>
				</simple>
			" );
		}

		[Test]
		public void DictionaryWithNullReference()
		{
			Compare( new DictionaryWithNullReference( true ), @"
				<simple>
					<entries><!--Null reference--></entries>
				</simple>
			" );
		}

		[Test]
		public void ExceptionsInMembers()
		{
			Compare( new ExceptionsInMembers( true ), @"
				<simple>
					<array><!--##NullReference: No array here.--></array>
					<string><!--##NotImplemented: The method or operation is not implemented.--></string>
				</simple>
			" );
		}
		[Test]
		public void NullStringProperty()
		{
			Compare( new ObjNullStringProperty(), @"
				<obj xmlns='http://emsinet.com/schemas/Order/Data'>
					<ClientOrderId xsi:nil='true' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'/>
					<SubmittedDate>2005-01-13T13:00:00+03:00</SubmittedDate>
				</obj>
			" );
		}

		[Test]
		public void XmlTextAttribute()
		{
			Compare( new XmlTextString(), @"
				<XmlTextString Attrib='attribute1'>
					<Child1>child1</Child1>text1<Child2>child2</Child2>
				</XmlTextString>
			" );
		}

		[Test]
		public void XmlTextNullAttribute()
		{
			Compare( new XmlTextNullContainer(), @"
                <XmlTextNullContainer>
                    <Type Attrib='attribute1'></Type>
                    <Name>Hello</Name>
                </XmlTextNullContainer>
                " );
		}

		[Test]
		public void DecoratedInterface()
		{
			Compare( new Person(), @"
				<person>
					<name>John Smith</name>
					<age>25</age>
				</person>
			" );
		}

		[Test]
		public void XmlElementProperty()
		{
			Compare( new ObjXmlElement(), @"
				<obj>
					<x:details xmlns:x='http://www.namespaces.org'>
						<z:descr xmlns:z='urn:some-namespace'>
							<price for='kg'>$17.98</price>
							<store amount='516' reserved='20'/>
						</z:descr>
					</x:details>
				</obj>
			" );
		}

		[Test]
		public void OrderTest()
		{
			//XPathObjectNavigator nav = new XPathObjectNavigator(tx);
			//for (bool go = nav.MoveToFirstChild(); go; go = nav.MoveToNext())
			//{
			//	string value = nav.Value;
			//	if (nav.HasChildren)
			//	{
			//		XPathNavigator child = nav.Clone();
			//		child.MoveToFirstChild();
			//		value = child.Value;
			//	}
			//}

			Order order = new Order( true ); 
			Compare( order, @"
				<Order xmlns='http://emsinet.com/schemas/Order/Data' 
					   xmlns:emsi='http://emsinet.com/schemas/BaseTypes'>
					<OrderId>0</OrderId>
					<ClientOrderId>1234</ClientOrderId>
					<SubmittedDate>" + ConvertDate( order.SubmittedDate ) + @"</SubmittedDate>
					<Requirement>
						<Type tc='11'>APS</Type>
						<ClientUniqueId>2468</ClientUniqueId>
						<ProcessingUniqueId>923232</ProcessingUniqueId>
						<State>
							<Code tc='75'>Accepted</Code>
							<SubCode tc='3'>Reviewing</SubCode>
						</State>
					</Requirement>
					<Requirement>
						<Type tc='4'>MVR</Type>
						<ClientUniqueId>124692</ClientUniqueId>
						<ProcessingUniqueId>18342</ProcessingUniqueId>
						<State>
							<Code tc='3'>Completed</Code>
							<SubCode xsi:nil='true' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'/>
						</State>
					</Requirement>
					<Transaction>
						<Timestamp>" + ConvertDate( ((OrderTransaction)order.Transactions[0]).Timestamp ) + @"</Timestamp>
						<Type tc='1'>Create</Type>
						<ConfirmationNumber>123456</ConfirmationNumber>
						<ClientRequest>
							<AnyClientRequest foo='bar' xmlns='kaitlyn'>Brenda</AnyClientRequest>
						</ClientRequest>
					</Transaction>
				</Order>
			");
		}

		[Test]
		public void XmlNodeTest()
		{
			string xml = 
				@"<Requirement>
					<Type tc='11'>APS</Type>
					<ClientUniqueId>2468</ClientUniqueId>
					<ProcessingUniqueId>923232</ProcessingUniqueId>
					<State>
						<Code tc='75'>Accepted</Code>
						<SubCode tc='3'>Reviewing</SubCode>
					</State>
				</Requirement>";
			XmlDocument doc = new XmlDocument();
			doc.LoadXml( xml );
			Compare( doc, xml );
		}

		[Test]
		public void XmlNodeWithNamespacesTest()
		{
			string xml = 
				@"<Requirement xmlns='ns-default' xmlns:a='ns-a' xmlns:b='ns-b'>
					<Type tc='11'>APS</Type>
					<ClientUniqueId>2468</ClientUniqueId>
					<ProcessingUniqueId xmlns:a='ns-a'>923232</ProcessingUniqueId>
					<State xmlns:b='ns-b'>
						<Code tc='75'>Accepted</Code>
						<SubCode tc='3'>Reviewing</SubCode>
					</State>
				</Requirement>";
			XmlDocument doc = new XmlDocument();
			doc.LoadXml( xml );
			Compare( doc, xml );
		}

		[Test]
		public void XmlNodeDefaultNamespace()
		{
			string xml = 
				@"<Requirement xmlns='http://www.byte-force.com/schemas/SDF' xmlns:a='ns-a' xmlns:b='ns-b'>
					<Type tc='11'>APS</Type>
					<ClientUniqueId>2468</ClientUniqueId>
					<ProcessingUniqueId xmlns:a='ns-a'>923232</ProcessingUniqueId>
					<State xmlns:b='ns-b'>
						<Code tc='75'>Accepted</Code>
						<SubCode tc='3'>Reviewing</SubCode>
					</State>
				</Requirement>";
			XmlDocument doc = new XmlDocument();
			doc.LoadXml( xml );
			ObjectXPathContext context = new ObjectXPathContext();
			context.NamespaceManager.AddNamespace( "sdf", Namespaces.SDF );
			XPathNavigator nav = context.CreateNavigator( doc );
			nav.MoveToFirstChild();
			Assert.AreEqual( "sdf", nav.Prefix );
			Assert.AreEqual( "Requirement", nav.LocalName );
			Assert.AreEqual( "sdf:Requirement", nav.Name );
		}

		[Test]
		public void TryTransparentAttribute()
		{
			Compare( new ObjTransparentAttribute( true ), @"
				<tran-attr>
					<Name>Joe</Name>
					<TheSimple sdf:uhus='1' juja='2' xmlns:sdf='http://www.byte-force.com/schemas/SDF'>
						<child1>3</child1>
						<sdf:child2>4</sdf:child2>
					</TheSimple>
				</tran-attr>
			" );
		}

		[Test]
		public void TrySkipNavigableAttribute()
		{
			Compare( new ObjSkipNavRoot( true ), @"
				<skip-attr>
					<Name>Jim</Name>
					<Details><details about='me'/></Details>
				</skip-attr>
			" );
		}

		[Test]
		public void TryEnumerableProperty()
		{
			Compare( new ObjEnumerableProperty( true ), @"
				<enumerable>
					<Children>red</Children>
					<Children>green</Children>
					<Children>blue</Children>
				</enumerable>
			" );
		}

		[Test, Ignore]
		public void TransformDataset()
		{
			var serializer = new XmlSerializer( typeof( DataSet ) );
			var dataset = serializer.Deserialize( new StringReader( 
				@"<?xml version='1.0' encoding='utf-8'?>
				<DataSet>
				  <xs:schema id='NewDataSet' xmlns='' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:msdata='urn:schemas-microsoft-com:xml-msdata'>
					<xs:element name='NewDataSet' msdata:IsDataSet='true' msdata:UseCurrentLocale='true'>
					  <xs:complexType>
						<xs:choice minOccurs='0' maxOccurs='unbounded'>
						  <xs:element name='Queries'>
							<xs:complexType>
							  <xs:sequence>
								<xs:element name='TableName' type='xs:string' minOccurs='0' />
								<xs:element name='Query' type='xs:string' minOccurs='0' />
							  </xs:sequence>
							</xs:complexType>
						  </xs:element>
						</xs:choice>
					  </xs:complexType>
					</xs:element>
				  </xs:schema>
				  <diffgr:diffgram xmlns:msdata='urn:schemas-microsoft-com:xml-msdata' xmlns:diffgr='urn:schemas-microsoft-com:xml-diffgram-v1'>
					<NewDataSet>
					  <Queries diffgr:id='Queries1' msdata:rowOrder='0' diffgr:hasChanges='inserted'>
						<TableName>Main</TableName>
						<Query />
					  </Queries>
					</NewDataSet>
				  </diffgr:diffgram>
				</DataSet>" )
			) as DataSet;

			IsSubset( dataset, @"
				<DataSet>
					<DesignMode>false</DesignMode>
					<RemotingFormat>Xml</RemotingFormat>
					<SchemaSerializationMode>IncludeSchema</SchemaSerializationMode>
					<CaseSensitive>false</CaseSensitive>
					<DefaultViewManager>
						<DefaultViewManager></DefaultViewManager>
					</DefaultViewManager>
					<EnforceConstraints>true</EnforceConstraints>
					<DataSetName>NewDataSet</DataSetName>
					<Namespace></Namespace>
					<Tables>
						<Table />
					</Tables>
				</DataSet>
			" );
		}

	}
}