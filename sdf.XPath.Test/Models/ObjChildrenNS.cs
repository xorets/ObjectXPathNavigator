using System.Collections;
using System.Collections.Specialized;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace sdf.XPath.Test.Models
{
	/// <exclude/>
	[XmlRoot( "XmlObjectWithAttribs", Namespace=Namespaces.SDF )]
	public class ObjChildrenNS
	{
		[XmlAttribute( "uhus", Namespace=Namespaces.SDF )] public int attr1;
		[XmlAttribute( "attrib2" )] public string attr2;
		[XmlAttribute( "attrib3", Form = XmlSchemaForm.Qualified )] public string attr3;
		[XmlAttribute( "attrib4",Namespace="" )] public string attr4;
		[XmlAttribute( "attrib5", Form = XmlSchemaForm.Qualified, Namespace=Namespaces.SDF )] public string attr5;
		[XmlElement( "child1" )] public string elem1; 
		[XmlElement( "child2", Namespace=Namespaces.SDF )] public string elem2;
		[XmlElement( "child3", Namespace="" )] public string elem3;		
		[XmlElement( "child4", Form = XmlSchemaForm.Unqualified )] public string elem4;
		public string Elem5;

		public ObjChildrenNS()
		{}

		public ObjChildrenNS( bool init )
		{
			attr1 = 1;
			attr2 = "2";
			attr3 = "3";
			attr4 = "a4";
			attr5 = "a5";
			elem1 = "4";
			elem2 = "5";
			elem3 = "6";
			elem4 = "7";
			Elem5 = "elem5";
		}
	}

	[XmlType( Namespace=Namespaces.SDF + "Type" )]
	[XmlRoot( "XmlObjectWithAttribs", Namespace=Namespaces.SDF )]
	public class ObjChildrenNSTypeAttr
	{
		[XmlAttribute( "uhus", Namespace=Namespaces.SDF )] public int attr1;
		[XmlAttribute( "attrib2" )] public string attr2;
		[XmlAttribute( "attrib3", Form = XmlSchemaForm.Qualified )] public string attr3;
		[XmlAttribute( "attrib4",Namespace="" )] public string attr4;
		[XmlAttribute( "attrib5", Form = XmlSchemaForm.Qualified, Namespace=Namespaces.SDF )] public string attr5;
		[XmlElement( "child1" )] public string elem1; 
		[XmlElement( "child2", Form=XmlSchemaForm.Qualified )] public string elem2;
		[XmlElement( "child3", Namespace="" )] public string elem3;		
		[XmlElement( "child4", Form = XmlSchemaForm.Unqualified )] public string elem4;

		public ObjChildrenNSTypeAttr() {}

		public ObjChildrenNSTypeAttr( bool init ) 
		{
			attr1 = 1;
			attr2 = "2";
			attr3 = "3";
			attr4 = "3.5";
			attr5 = "a5";
			elem1 = "4";
			elem2 = "5";
			elem3 = "6";
			elem4 = "7";
		}
	}

	[XmlType( "TheType", Namespace=Namespaces.SDF + "Type" )]
	[XmlRoot( "XmlObjectWithAttribs", Namespace=Namespaces.SDF )]
	public class ObjChildrenNSInherit : ObjChildrenNS
	{
		[XmlAttribute( "uhus2", Namespace=Namespaces.SDF )] public int attr12;
		[XmlAttribute( "attrib22" )] public string attr22;
		[XmlAttribute( "attrib32", Form = XmlSchemaForm.Qualified )] public string attr32;
		[XmlAttribute( "attrib42",Namespace="" )] public string attr42;
		[XmlElement( "child12" )] public string elem12; 
		[XmlElement( "child22", Namespace=Namespaces.SDF )] public string elem22;
		[XmlElement( "child32", Namespace="" )] public string elem32;		
		[XmlElement( "child42", Form = XmlSchemaForm.Unqualified )] public string elem42;

		public ObjChildrenNSInherit() {}

		public ObjChildrenNSInherit( bool init ) :
			base( init )
		{
			attr12 = 8;
			attr22 = "9";
			attr32 = "10";
			attr42 = "11";
			elem12 = "12";
			elem22 = "13";
			elem32 = "14";
			elem42 = "15";
		}
	}

}