using System;
using System.Xml.Serialization;

namespace sdf.XPath
{
	/// <summary>
	/// Abstract base class for holding type or member decorations with attributes.
	/// </summary>
	public abstract class NodeInfo
	{
		// Attributes
		private NodePolicyAttribute _nodePolicy;// = null;
		private ConverterAttribute _converter;// = null;
		private TransparentAttribute _transparent;// = null;
		private SkipNavigableRootAttribute _skipNavigableRoot;// = null;
		private ChildXmlElementAttribute _childXmlElementName;// = null;
		private XmlRootAttribute _xmlRoot;// = null;
		private XmlAttributeAttribute _xmlAttribute;// = null;
		private XmlElementAttribute _xmlElement;// = null;
		private XmlAnyElementAttribute _xmlAnyElement;// = null;
		private XmlTextAttribute _xmlText;// = null;
		private XmlIgnoreAttribute _xmlIgnore;// = null;
		private XmlTypeAttribute _xmlType;// = null;
		// General
		internal string _nodeName;// = null;
		internal string _namespace;// = null;
		private Type _converterType;// = null;
		private bool _isNullable;// = false;

		internal void GetDecorations( System.Reflection.MemberInfo memberInfo )
		{
			var attrs = memberInfo.GetCustomAttributes( true );
			foreach( var attr in attrs )
				Add( attr );
		}

		/// <summary>
		/// Adds new attribute to the node decorations.
		/// </summary>
		/// <param name="attr">An attriubute.</param>
		protected void Add( object attr )
		{
			if( attr is NodePolicyAttribute )
			{
				if( _nodePolicy == null )
					_nodePolicy = (NodePolicyAttribute)attr;
			}
			else if( attr is ConverterAttribute )
			{
				if( _converter == null )
					_converter = (ConverterAttribute)attr;
			}
			else if( attr is TransparentAttribute )
				_transparent = (TransparentAttribute)attr;
			else if( attr is SkipNavigableRootAttribute )
				_skipNavigableRoot = (SkipNavigableRootAttribute)attr;
			else if( attr is ChildXmlElementAttribute )
				_childXmlElementName = (ChildXmlElementAttribute)attr;
			else if( attr is XmlRootAttribute )
			{
				if( _xmlRoot == null )
					_xmlRoot = (XmlRootAttribute)attr;
			}
			else if( attr is XmlAttributeAttribute )
			{
				if( NodeTypeUndefined() )
					_xmlAttribute = (XmlAttributeAttribute)attr;
			}
			else if( attr is XmlElementAttribute )
			{
				if( NodeTypeUndefined() )
				{
					_xmlElement = (XmlElementAttribute)attr;
					_isNullable = _xmlElement.IsNullable;
				}
			}
			else if( attr is XmlAnyElementAttribute )
			{
				if( NodeTypeUndefined() )
					_xmlAnyElement = (XmlAnyElementAttribute)attr;
			}
			else if( attr is XmlTextAttribute )
			{
				if( NodeTypeUndefined() )
					_xmlText = (XmlTextAttribute)attr;
			}
			else if( attr is XmlIgnoreAttribute )
				_xmlIgnore = (XmlIgnoreAttribute)attr;
			else if( attr is XmlTypeAttribute )
				_xmlType = (XmlTypeAttribute)attr;
		}

		private bool NodeTypeUndefined()
		{
			return _xmlAttribute == null && _xmlElement == null && _xmlText == null && _xmlAnyElement == null;
		}

		/// <summary>
		/// Gets the NodePolicyAttribute.
		/// </summary>
		/// <value>An instance of the attribute if one is specified.</value>
		public NodePolicyAttribute NodePolicy
		{
			get { return _nodePolicy; }
		}

		/// <summary>
		/// Gets the ConverterAttribute.
		/// </summary>
		/// <value>An instance of the attribute if one is specified.</value>
		public ConverterAttribute Converter
		{
			get { return _converter; }
		}

		/// <summary>
		/// Gets the TransparentAttribute.
		/// </summary>
		/// <value>An instance of the attribute if one is specified.</value>
		public TransparentAttribute Transparent
		{
			get { return _transparent; }
		}

		/// <summary>
		/// Gets the SkipNavigableRootAttribute.
		/// </summary>
		/// <value>An instance of the attribute if one is specified.</value>
		public SkipNavigableRootAttribute SkipNavigableRoot
		{
			get { return _skipNavigableRoot; }
		}

		/// <summary>
		/// Gets the ChildXmlElementAttribute.
		/// </summary>
		/// <value>An instance of the attribute if one is specified.</value>
		public ChildXmlElementAttribute ChildXmlElementName
		{
			get { return _childXmlElementName; }
		}

		/// <summary>
		/// Gets the XmlRootAttribute.
		/// </summary>
		/// <value>An instance of the attribute if one is specified.</value>
		public XmlRootAttribute XmlRoot
		{
			get { return _xmlRoot; }
		}

		/// <summary>
		/// Gets the XmlAttributeAttribute.
		/// </summary>
		/// <value>An instance of the attribute if one is specified.</value>
		public XmlAttributeAttribute XmlAttribute
		{
			get { return _xmlAttribute; }
		}

		/// <summary>
		/// Gets the XmlElementAttribute.
		/// </summary>
		/// <value>An instance of the attribute if one is specified.</value>
		public XmlElementAttribute XmlElement
		{
			get { return _xmlElement; }
		}

		/// <summary>
		/// Gets the XmlAnyElementAttribute.
		/// </summary>
		/// <value>An instance of the attribute if one is specified.</value>
		public XmlAnyElementAttribute XmlAnyElement
		{
			get { return _xmlAnyElement; }
		}

		/// <summary>
		/// Gets the XmlTextAttribute.
		/// </summary>
		/// <value>An instance of the attribute if one is specified.</value>
		public XmlTextAttribute XmlText
		{
			get { return _xmlText; }
		}

		/// <summary>
		/// Gets the XmlIgnoreAttribute.
		/// </summary>
		/// <value>An instance of the attribute if one is specified.</value>
		public XmlIgnoreAttribute XmlIgnore
		{
			get { return _xmlIgnore; }
		}

		/// <summary>
		/// Gets the XmlTypeAttribute.
		/// </summary>
		/// <value>An instance of the attribute if one is specified.</value>
		public XmlTypeAttribute XmlType
		{
			get { return _xmlType; }
		}

		/// <summary>
		/// Gets a value indicating whether this node is nullable.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this node is nullable; otherwise, <c>false</c>.
		/// </value>
		public bool IsNullable
		{
			get { return _isNullable; }
		}
		/// <summary>
		/// Gets the element namespace.
		/// </summary>
		/// <value>The quilified namespace of an XML element representing the instance 
		/// of the described class when it is mapped by the navigator.</value>
		/// <remarks>Namespace must be specified by means of 
		/// <see cref="System.Xml.Serialization.XmlRootAttribute"/>. <br/>
		/// Take in attention that this namespace will no be used when object is accessed
		/// through a property of other object. In this case it will have the namespace
		/// of the property.
		/// </remarks>
		public string Namespace
		{
			get { return _namespace ?? string.Empty; }
		}

		/// <summary>
		/// Type of converter for this type or member.
		/// </summary>
		protected Type ConverterType
		{
			get { return _converterType; }
			set { _converterType = value; }
		}
	}
}