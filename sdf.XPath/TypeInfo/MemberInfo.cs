using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using sdf.XPath.NodePolicy;
using R = System.Reflection;

namespace sdf.XPath
{
	/// <summary>
	/// Provides the abstract base class for a class members descriptors.
	/// </summary>
	public abstract class MemberInfo : NodeInfo
	{
		private XPathNodeType _nodeType;
		private int _index;
		private bool _isTransparent;
		internal XmlSchemaForm Form;

		/// <summary>
		/// Creates a new <see cref="MemberInfo"/> instance.
		/// </summary>
		protected MemberInfo( )
		{
			_nodeName = string.Empty;
			_nodeType = XPathNodeType.Element;
			Form = XmlSchemaForm.None;
		}

		/// <summary>
		/// Gets a XPath node type of the member.
		/// </summary>
		/// <value>A value of <see cref="XPathNodeType"/> enumeration.</value>
		/// <remarks>Generally, only Element and Attribute types are expeced.</remarks>
		public XPathNodeType NodeType
		{
			get { return _nodeType; }
			set { _nodeType = value; }
		}

		/// <summary>
		/// Gets a name of the node for this member.
		/// </summary>
		/// <value>A name which will be used when mapping this member to XML tree.</value>
		public string Name
		{
			get { return _nodeName; }
		}

		/// <summary>
		/// Gets a type of this member.
		/// </summary>
		/// <value>A type of the member.</value>
		/// <example>
		/// <code>
		/// class Dog 
		/// {
		///   public string Name;
		/// }
		/// </code>
		/// The member info for a Name field will have a type of <see cref="String"/>.
		/// </example>
		public virtual Type Type
		{
			get { return typeof( object ); }
		}

		/// <summary>
		/// Gets or sets an index of the member in owning class.
		/// </summary>
		/// <value>Zero-based index of the member in its owner class.</value>
		public int Index
		{
			get { return _index; }
			set { _index = value; }
		}

		/// <summary>
		/// Gets or sets a transparency flag for the member.
		/// </summary>
		/// <value><see langword="true"/> if this member is transparent,
		/// <see langword="false"/> otherwise.</value>
		/// <remarks>
		/// Member could be marked as transparent using the 
		/// <see cref="System.Xml.Serialization.XmlAnyElementAttribute"/>. For more 
		/// information on transparency see remarks on <see cref="Node.IsTransparent"/>
		/// property.
		/// </remarks>
		public bool IsTransparent
		{
			get { return _isTransparent; }
			set { _isTransparent = value; }
		}

		/// <summary>
		/// Gets or sets the type of value converter for this member.
		/// </summary>
		/// <value>A type of converter which must be used to convert a value of
		/// this member to the string form or vice versa. </value>
		/// <remarks>
		/// If it is equal to <see langword="null"/> then default conversion rules 
		/// should be employed.
		/// </remarks>
		public new Type ConverterType
		{
			get { return base.ConverterType; }
			set { base.ConverterType = value; }
		}

		/// <summary>
		/// When overridden in derived class gets a value of the member in given instance.
		/// </summary>
		/// <param name="instance">The object instance to get member value for.</param>
		/// <returns>Returns a value of corresponding member of given object.</returns>
		public abstract object GetValue( object instance );
		internal void ProcessDecorations()
		{
			if( XmlAttribute != null )
			{
				if( !string.IsNullOrEmpty( XmlAttribute.AttributeName ) )
					_nodeName = XmlAttribute.AttributeName;
				_namespace = XmlAttribute.Namespace;
				Form = XmlAttribute.Form;
				_nodeType = XPathNodeType.Attribute;
			}
			else if( XmlElement != null )
			{
				if( !string.IsNullOrEmpty( XmlElement.ElementName ) )
					_nodeName = XmlElement.ElementName;
				_namespace = XmlElement.Namespace;
				Form = XmlElement.Form;
				_nodeType = XPathNodeType.Element;
				if( typeof( IList ).IsAssignableFrom( Type ) )
					_isTransparent = true;
			}
			else if( XmlAnyElement != null )
			{
				if( !string.IsNullOrEmpty( XmlAnyElement.Name ) )
					_nodeName = XmlAnyElement.Name;
				_namespace = XmlAnyElement.Namespace;
				Form = XmlSchemaForm.Qualified;

				_nodeType = XPathNodeType.Element;
				_isTransparent = true;

				if( typeof( XmlNode[] ).IsAssignableFrom( Type ) ) 
					Add( new NodePolicyAttribute( typeof( AnyElementXmlNodeArrayNodePolicy )));
			}
			else if( XmlText != null )
				_nodeType = XPathNodeType.Text;

			if( Converter != null )
				base.ConverterType = Converter.ConverterType;

			if( Transparent != null )
				_isTransparent = Transparent.IsTransparent;
		}
	}

	/// <summary>
	/// Implementation of MemberInfo for properties.
	/// </summary>
	internal class PropertyInfo : MemberInfo
	{
		private readonly R.PropertyInfo _pi;
		private readonly Func<object, object> _getValueDelegate;

		public PropertyInfo( R.PropertyInfo pi  )
		{
			_pi = pi;
			_getValueDelegate = GeneratePropertyGetter( _pi.GetGetMethod() );
		}

		public override object GetValue( object instance )
		{
			try
			{
				return _getValueDelegate( instance );
			}
			catch( TargetInvocationException e )
			{
				throw e.InnerException;
			}
		}

		public override Type Type
		{
			get { return _pi.PropertyType; }
		}

		private static Func<object, object> GeneratePropertyGetter( MethodInfo invokeMethod )
		{
			var instanceParam = Expression.Parameter( typeof( object ), "instance" );
			var asTargetType = Expression.TypeAs( instanceParam, invokeMethod.DeclaringType );
			var methodCall = Expression.Call( asTargetType, invokeMethod, null );
			var asObj = Expression.TypeAs( methodCall, typeof( object ) );

			return Expression.Lambda<Func<object, object>>( asObj, instanceParam ).Compile();
		}
	}

	/// <summary>
	/// Implementation of MemberInfo for fields.
	/// </summary>
	internal class FieldInfo : MemberInfo
	{
		private readonly R.FieldInfo _fi;
		private readonly Func<object, object> _getValueDelegate;

		public FieldInfo( R.FieldInfo fi )
		{
			_fi = fi;
			_getValueDelegate = GenerateFieldGetter( _fi );
		}

		public override object GetValue( object instance )
		{
			return _getValueDelegate( instance );
		}

		public override Type Type
		{
			get { return _fi.FieldType; }
		}

		private static Func<object, object> GenerateFieldGetter( R.FieldInfo field )
		{
			var instanceParam = Expression.Parameter( typeof( object ), "instance" );
			var asTargetType = Expression.TypeAs( instanceParam, field.DeclaringType );
			var getField = Expression.Field( asTargetType, field );
			var asObj = Expression.TypeAs( getField, typeof( object ) );

			return Expression.Lambda<Func<object, object>>( asObj, instanceParam ).Compile();
		}
	}
}