using System;
using System.Xml.Serialization;

namespace sdf.XPath.Test.Models
{
	public class XmlTextString
	{
		[XmlAttribute]
		public string Attrib = "attribute1";
		public string Child1 = "child1";
		[XmlText]
		public string Text = "text1";
		public string Child2 = "child2";
	}

	public class XmlTextNull
	{
		[XmlAttribute]
		public string Attrib = "attribute1";
		[XmlText]
		public string Text = null;
	}

	public class XmlTextNullContainer
	{
		public XmlTextNull Type;
		public string Name;

		public XmlTextNullContainer()
		{
			Type = new XmlTextNull();
			Name = "Hello";
		}
	}

}
