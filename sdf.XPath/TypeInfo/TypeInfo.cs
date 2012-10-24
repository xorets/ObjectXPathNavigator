using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using sdf.XPath.NodePolicy;

namespace sdf.XPath
{
	/// <summary>
	/// Contains an information about a type, which used for exposing members of 
	/// this type's instance to <see cref="ObjectXPathNavigator"/>.
	/// </summary>
	public class TypeInfo : NodeInfo
	{
		private static MemberInfo[] _emptyChildNodeInfos = new MemberInfo[0];
		private Type _type;
		private MemberInfo[] _elements;
		private MemberInfo[] _attributes;
		private HybridDictionary _attributesNamed;
		private string _typeName;// = null;
		private bool _isSimpleType;// = false;
		private bool _isXPathNavigable;// = false;
		private Type _nodePolicyType;// = null;
		private readonly object _locker = new object();

		/// <summary>
		/// Constructs a descriptor of given type.
		/// </summary>
		/// <param name="type">A type to build type info for.</param>
		public TypeInfo( Type type )
		{
			_type = type;
			FillTypeInformation();
		}

		/// <summary>
		/// Gets a <see cref="System.Type"/> described by this type info.
		/// </summary>
		public Type Type
		{
			get { return _type; }
		}

		/// <summary>
		/// Gets the element name.
		/// </summary>
		/// <value>The name of an XML element representing the instance of the described class 
		/// when it is mapped by the navigator.</value>
		/// <remarks>Name must be specified by means of 
		/// <see cref="System.Xml.Serialization.XmlRootAttribute"/>. <br/>
		/// Take in attention that this name will no be used when object is accessed
		/// through a property of other object. In this case it will be known by the 
		/// name of the property.
		/// </remarks>
		public string Name
		{
			get { return _nodeName ?? _typeName; }
		}

		/// <summary>
		/// Gets the array of all child elements.
		/// </summary>
		/// <returns>An array of <see cref="MemberInfo"/> objects.</returns>
		/// <remarks>Both elements and attributes consists of public members (fields and
		/// properties) of the class. Members marked with 
		/// <see cref="System.Xml.Serialization.XmlAttributeAttribute"/> attribute will 
		/// be represented by attribute nodes. Members with 
		/// <see cref="System.Xml.Serialization.XmlElementAttribute"/> attribute will 
		/// be rendered as child elements.</remarks>
		public MemberInfo[] GetElements()
		{
			if( _elements == null )
				lock( _locker )
					if( _elements == null )
						FillPropertiesInformation();

			return _elements;
		}

		/// <summary>
		/// Gets the array of all element's attributes.
		/// </summary>
		/// <returns>An array of <see cref="MemberInfo"/> objects.</returns>
		/// <remarks>Both elements and attributes consists of public members (fields and
		/// properties) of the class. Members marked with 
		/// <see cref="System.Xml.Serialization.XmlAttributeAttribute"/> attribute will 
		/// be represented by attribute nodes. Members with 
		/// <see cref="System.Xml.Serialization.XmlElementAttribute"/> attribute will 
		/// be rendered as child elements.</remarks>
		public MemberInfo[] GetAttributes()
		{
			if( _attributes == null )
				lock( _locker )
					if( _attributes == null )
						FillPropertiesInformation();

			return _attributes;
		}

		/// <summary>
		/// Searches for the attribute with specific name and namespace.
		/// </summary>
		/// <param name="name">The name of the attribute.</param>
		/// <param name="ns">The namespace of the attribute.</param>
		/// <returns>Returns a <see cref="MemberInfo"/> object, describing the 
		/// required attribute, or <see langword="null"/>.</returns>
		public MemberInfo GetAttribute( string name, string ns )
		{
			if( _attributes == null )
				lock( _locker )
					if( _attributes == null )
						FillPropertiesInformation();

			if( _attributesNamed == null )
				return null;

			return (MemberInfo)_attributesNamed[new XmlQualifiedName( name, ns )];
		}

		/// <summary>
		/// Indicates if this type is simple.
		/// </summary>
		/// <value><see langword="true"/> if this type is simple, 
		/// <see langword="false"/> otherwise.</value>
		/// <remarks>Simple types doesn't have attributes and child elements. Their
		/// value is directly mapped to the string.<br/>
		/// The type is simple if:
		/// <list type="bullet">
		/// <item><description>it is primitive (<see cref="System.Type.IsPrimitive"/>),</description></item>
		/// <item><description>it is <see cref="String"/> or <see cref="DateTime"/>,</description></item>
		/// <item><description>it is an enum.</description></item>
		/// </list>
		/// </remarks>
		public bool IsSimpleType
		{
			get { return _isSimpleType; }
		}

		/// <summary>
		/// Gets a value indicating whether this type support 
		/// <see cref="IXPathNavigable"/> interface.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this supports the interface; otherwise, <c>false</c>.
		/// </value>
		public bool IsXPathNavigable
		{
			get { return _isXPathNavigable; }
		}

		/// <summary>
		/// Gets the type of a proxy object (node).
		/// <seealso cref="NodePolicyAttribute"/>
		/// </summary>
		/// <value>The type of the proxy for instances of described type.</value>
		public Type PolicyType
		{
			get { return _nodePolicyType; }
		}

		/// <summary>
		/// Gets the type of the converter.
		/// <seealso cref="ConverterAttribute"/>
		/// </summary>
		/// <value>The type of the converter for values of this type.</value>
		public new Type ConverterType
		{
			get { return base.ConverterType; }
		}

		private void FillTypeInformation()
		{
			_typeName = _type.Name.Replace( "<", "" ).Replace( ">", "" ).Replace( "`", "" );

			// Is simple type
			if( _type.IsPrimitive || _type == typeof( string ) || _type == typeof( DateTime ) || _type == typeof( Decimal ) || _type.IsEnum )
				_isSimpleType = true;

			GetDecorations( _type );

			// Look in implemented interfaces
			foreach( var iface in _type.GetInterfaces() )
				GetDecorations( iface );

			if( XmlRoot != null )
			{
				_nodeName = XmlRoot.ElementName;
				_namespace = XmlRoot.Namespace;
			}

			if( NodePolicy != null )
				_nodePolicyType = NodePolicy.NodePolicyType;
			else
			{
				if( IsSimpleType )
					_nodePolicyType = typeof( TextNodePolicy );
				else if( typeof( IDictionary ).IsAssignableFrom( _type ) )
					_nodePolicyType = typeof( DictionaryNodePolicy );
				else if( typeof( IList ).IsAssignableFrom( _type ) )
					_nodePolicyType = typeof( ListNodePolicy );
				else if( typeof( XmlNode ).IsAssignableFrom( _type ) )
					_nodePolicyType = typeof( XmlNodeNodePolicy );
				else if( typeof( IEnumerable ).IsAssignableFrom( _type ) )
					_nodePolicyType = typeof( EnumerableNodePolicy );
				else
					_nodePolicyType = typeof( GenericNodePolicy );
			}

			if( Converter != null )
				base.ConverterType = Converter.ConverterType;

			// Is XPathNavigable
			if( typeof( IXPathNavigable ).IsAssignableFrom( _type ) )
				_isXPathNavigable = true;
		}

		private void FillPropertiesInformation()
		{
			var tmpElements = _emptyChildNodeInfos;
			var tmpAttributes = _emptyChildNodeInfos;

			if( IsSimpleType )
				return;

			var elements = new ArrayList();
			var attributes = new ArrayList();
			_attributesNamed = new HybridDictionary();
			int elementIndex = 0;
			int attributeIndex = 0;

			var members = _type.FindMembers( MemberTypes.Property | MemberTypes.Field,
											 BindingFlags.Instance | BindingFlags.Public, null, null );
			SortMembersArray( members );
			foreach( var mi in members )
			{
				// Create member info
				var pi = mi as System.Reflection.PropertyInfo;
				MethodInfo piGetMethod = null;
				MemberInfo nodeInfo;
				if( pi != null )
				{
					piGetMethod = pi.GetGetMethod();
					if( piGetMethod != null && piGetMethod.GetParameters().Length == 0 )
						nodeInfo = new PropertyInfo( pi );
					else
						// This is not a property without arguments.
						continue;
				}
				else
					nodeInfo = new FieldInfo( (System.Reflection.FieldInfo)mi );

				nodeInfo._nodeName = mi.Name;

				// Get decorations for the original MemberInfo
				nodeInfo.GetDecorations( mi );

				if( pi != null )
				{
					// Check for interface's attributes if the member is property
					foreach( var iface in _type.GetInterfaces() )
					{
						int accessorIndex = -1;
						var map = _type.GetInterfaceMap( iface );
						for( int i = 0; i < map.TargetMethods.Length; i++ )
							if( map.TargetMethods[i] == piGetMethod )
							{
								accessorIndex = i;
								break;
							}
						if( accessorIndex != -1 )
						{
							var ifaceMember = map.InterfaceMethods[accessorIndex];
							foreach( var ifaceProperty in iface.GetProperties() )
								if( ifaceProperty.GetGetMethod() == ifaceMember )
								{
									nodeInfo.GetDecorations( ifaceProperty );
									break;
								}
							break;
						}
					}
				}

				// Check for XmlIgnore attribute
				if( nodeInfo.XmlIgnore != null )
					continue;

				nodeInfo.ProcessDecorations();

				var declTypeInfo = TypeInfoCache.Instance.GetTypeInfo( mi.DeclaringType );
				string declSchemaNs = declTypeInfo.Namespace;
				if( declTypeInfo.XmlType != null && declTypeInfo.XmlType.Namespace != null )
					declSchemaNs = declTypeInfo.XmlType.Namespace;

				// Add member to the collection
				switch( nodeInfo.NodeType )
				{
					case XPathNodeType.Element:
						{
							if( ( nodeInfo.Form == XmlSchemaForm.None || nodeInfo.Form == XmlSchemaForm.Qualified ) &&
								nodeInfo._namespace == null )
								// Take NS from declaring type
								nodeInfo._namespace = declSchemaNs;

							goto case XPathNodeType.Text;
						}

					case XPathNodeType.Text:
						{
							// Add to array of elements
							nodeInfo.Index = elementIndex++;
							elements.Add( nodeInfo );
							break;
						}

					default: // Attribute
						{
							if( nodeInfo.Form == XmlSchemaForm.None )
							{
								if( nodeInfo._namespace == declSchemaNs )
									nodeInfo._namespace = null;
							}
							else if( nodeInfo.Form == XmlSchemaForm.Qualified && nodeInfo._namespace == null )
								// Take NS from declaring type
								nodeInfo._namespace = declSchemaNs;


							// Add to array of attributes
							nodeInfo.Index = attributeIndex++;
							attributes.Add( nodeInfo );
							_attributesNamed.Add( new XmlQualifiedName( nodeInfo.Name, nodeInfo.Namespace ), nodeInfo );
							break;
						}
				}
			}

			if( elements.Count > 0 )
				tmpElements = (MemberInfo[])elements.ToArray( typeof( MemberInfo ) );

			if( attributes.Count > 0 )
				tmpAttributes = (MemberInfo[])attributes.ToArray( typeof( MemberInfo ) );

			_elements = tmpElements;
			_attributes = tmpAttributes;
		}

		private static void SortMembersArray( System.Reflection.MemberInfo[] members )
		{
			var pairs = new Pair[members.Length];
			for( int i = 0; i < members.Length; i++ )
				pairs[i] = new Pair( members[i], i );
			Array.Sort( pairs, new PairComparer() );
			for( int i = 0; i < pairs.Length; i++ )
				members[i] = pairs[i].memberInfo;
		}

		private class Pair
		{
			public System.Reflection.MemberInfo memberInfo;
			public int index;

			public Pair( System.Reflection.MemberInfo memberInfo, int index )
			{
				this.memberInfo = memberInfo;
				this.index = index;
			}
		}

		private class PairComparer : IComparer
		{
			public int Compare( object x, object y )
			{
				if( x.Equals( y ) )
					return 0;

				var left = (Pair)x;
				var leftType = left.memberInfo.DeclaringType;
				var right = (Pair)y;
				var rightType = right.memberInfo.DeclaringType;

				return leftType.Equals( rightType )
					? ( left.index < right.index ? -1 : 1 )
					: ( rightType.IsSubclassOf( leftType ) ? -1 : 1 );
			}
		}
	}
}