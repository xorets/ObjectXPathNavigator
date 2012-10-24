using System;
using System.Xml.XPath;

namespace sdf.XPath
{
	/// <summary>
	/// Class representing a node of object tree. 
	/// </summary>
	/// <remarks>
	/// All nodes has the same properites but could have different
	/// <see cref="INodePolicy">node policies</see>.<br/>
	/// Almost each node represents an object, but there could be three different 
	/// states: 
	/// <list type="bullet">
	/// <item>Target object is know for the node;</item>
	/// <item>Target object is not known yet, but node knows parent object and has
	/// <see cref="MemberInfo"/> describing field or property, which must be used
	/// to get a target object;</item>
	/// <item>Node has tried to resolve target object, but got an exception.</item>
	/// </list>
	/// In very special cases there could be a node, which neither has and object, nor
	/// knows how to get it. An example of such node is text node inside of child 
	/// element.<br/>
	/// User code usually communicates with nodes only from custom node policies.
	/// </remarks>
	public class Node
	{
		private ObjectXPathContext _context;
		private Node _parent;
		private int _index;

		private INodePolicy _nodePolicy;

		private string _name; // Empty if null
		private string _namespace;  // Empty if null
		private XPathNodeType _nodeType; // Empty if XPathNodeType.All

		private object _targetObject; // Empty if _nodeState == NodeState.ObjectNotKnown
		private NodeState _nodeState;
		private TypeInfo _targetObjectType; // Empty if null

		private MemberInfo _memberInfo;
		private Exception _accessException;

		private bool _isTransparent; // Empty if !_knowTransparent
		private bool _knowTransparent;

		private string _value; // Empty if null
		private int _attributesCount; // Empty if -1
		private int _elementsCount; // Empty if -1

		private Node[] _cache;
		private static readonly Node[] EmptyCache = new Node[0];
		private static readonly Node NullCacheEntry = new Node( null, null );

		/// <summary>
		/// Creates a new <see cref="Node"/> instance.
		/// </summary>
		/// <param name="context">Context this node belongs to.</param>
		/// <param name="policy">The node policy.</param>
		public Node( ObjectXPathContext context, INodePolicy policy )
		{
			_context = context;
			_nodePolicy = policy;
			_nodeType = XPathNodeType.All;
			_nodeState = NodeState.ObjectNotKnown;
			_attributesCount = -1;
			_elementsCount = -1;
		}

		/// <summary>
		/// Gets the <see cref="ObjectXPathContext"/> this node belongs to.
		/// </summary>
		/// <value>A context this node belongs to.</value>
		public ObjectXPathContext Context
		{
			get { return _context; }
		}

		/// <summary>
		/// Gets or sets the parent node.
		/// </summary>
		/// <value>The parent of this node.</value>
		public Node Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}

		/// <summary>
		/// Gets or sets the index of this node in the parent node.
		/// </summary>
		/// <value>Zero based index of this node in its parent node.</value>
		/// <remarks>Attributes and child elements are counted differently, so
		/// in one parent node could be an attribute and a child element with the
		/// same index.</remarks>
		public int Index
		{
			get { return _index; }
			set { _index = value; }
		}

		/// <summary>
		/// Gets or sets the node policy.
		/// </summary>
		/// <value>The node policy which handles with node.</value>
		public INodePolicy Policy
		{
			get { return _nodePolicy; }
			set { _nodePolicy = value; }
		}

		/// <summary>
		/// Gets the current state of the node.
		/// </summary>
		/// <value>The current state of the node.</value>
		public NodeState State
		{
			get { return _nodeState; }
		}

		/// <summary>
		/// Gets or sets the target object.
		/// </summary>
		/// <value>The target object represented by this node.</value>
		public object Object
		{
			get
			{
				ResolveObject();
				return _targetObject;
			}
			set
			{
				_targetObject = value;
				_targetObjectType = null;
				_nodeState = NodeState.ObjectKnown;
				CheckAndSwitchPolicy();
			}
		}

		/// <summary>
		/// Resolves a target object if it is not known.
		/// </summary>
		/// <remarks>If a target object is not known yet then this method tries
		/// to resolve it by getting the value of the corresponding 
		/// <see cref="MemberInfo"/>. In this case state of the node will be changed
		/// either to <see cref="NodeState.ObjectKnown"/> or 
		/// <see cref="NodeState.Exception"/>. After the change the 
		/// <see cref="INodePolicy.GetNewPolicy"/> method will be called to 
		/// change the policy of the node, if current policy will request for it.
		/// </remarks>
		public void ResolveObject()
		{
			if( State == NodeState.ObjectNotKnown )
			{
				// Resolve target object
				try
				{
					Object = _memberInfo.GetValue( Parent.Object );
				}
				catch( Exception e )
				{
					AccessException = e;
				}
				CheckAndSwitchPolicy();
			}
		}

		private void CheckAndSwitchPolicy()
		{
			if( _nodePolicy != null )
			{
				var newPolicy = _nodePolicy.GetNewPolicy( this );
				if( newPolicy != null )
					_nodePolicy = newPolicy;
			}
		}

		/// <summary>
		/// Gets or sets a type descriptor of target object.
		/// </summary>
		/// <value>A <see cref="TypeInfo"/> object describing target type of the node.</value>
		public TypeInfo ObjectType
		{
			get
			{
				if( _targetObjectType == null )
					if( Object != null )
						_targetObjectType = _context.TypeInfoCache.GetTypeInfo( _targetObject );
				return _targetObjectType;
			}
			set
			{
				_targetObjectType = value;
			}
		}

		/// <summary>
		/// Gets or sets the access exception.
		/// </summary>
		/// <value>An exception which were raised while resolving the object, 
		/// or <see langword="null"/> if there were no exception.</value>
		public Exception AccessException
		{
			get { return _accessException; }
			set
			{
				_accessException = value;
				_nodeState = NodeState.Exception;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="MemberInfo"/> for this node.
		/// </summary>
		/// <value>A descriptor of the field or property, corresponding to this 
		/// node, or <see langword="null"/> if this was initialized with a 
		/// reference to target object.</value>
		public MemberInfo Member
		{
			get { return _memberInfo; }
			set { _memberInfo = value; }
		}

		/// <summary>
		/// Gets or sets the name of the node.
		/// </summary>
		/// <value>Name of the node in the resulting XML tree.</value>
		public string Name
		{
			get
			{
				if( _name == null )
					if( _nodePolicy != null )
					{
						_name = _nodePolicy.GetName( this );
						_name = _name == null ? string.Empty : _context.NameTable.Add( _name );
					}
					else
						return string.Empty;
				return _name;
			}
			set { _name = _context.NameTable.Add( value ); }
		}

		/// <summary>
		/// Gets or sets the namespace of the node.
		/// </summary>
		/// <value>Fully qualified namespace of the node in the resulting
		/// XML tree. Could be <see langword="null"/> if node has no namespace.</value>
		public string Namespace
		{
			get
			{
				if( _namespace == null )
					if( _nodePolicy != null )
					{
						_namespace = _nodePolicy.GetNamespace( this );
						_namespace = _namespace == null ? string.Empty : _context.NameTable.Add( _namespace );
					}
					else
						return string.Empty;
				return _namespace;
			}
			set { _namespace = _context.NameTable.Add( value ); }
		}

		/// <summary>
		/// Gets or sets the value of the node.
		/// </summary>
		/// <value>Value of the element of attribute.</value>
		public string Value
		{
			get
			{
				if( _value == null )
					if( _nodePolicy != null )
						_value = _nodePolicy.GetValue( this );
					else
						return string.Empty;
				return _value;
			}
			set { _value = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is transparent.
		/// </summary>
		/// <value>
		/// 	<see langword="true"/> if this instance is transparent; 
		/// 	otherwise, <see langword="false"/>.
		/// </value>
		/// <remarks>Transparency allows for making a node which will be skipped
		/// during the tree traversal. If transparent node has children, then those
		/// children will appear as children of parent of this transparent node.</remarks>
		public bool IsTransparent
		{
			get
			{
				if( !_knowTransparent )
					if( _nodePolicy != null )
					{
						_isTransparent = _nodePolicy.GetIsTransparent( this );
						_knowTransparent = true;
					}
					else
						return false;
				return _isTransparent;
			}
			set
			{
				_isTransparent = value;
				_knowTransparent = true;
			}
		}

		/// <summary>
		/// Gets or sets the node type.
		/// </summary>
		/// <value>Type of the node.</value>
		/// <remarks>
		/// Allowed types are:
		/// <list type="bullet">
		/// <item><see cref="XPathNodeType.Attribute"/></item>
		/// <item><see cref="XPathNodeType.Element"/></item>
		/// <item><see cref="XPathNodeType.Text"/></item>
		/// <item><see cref="XPathNodeType.Comment"/></item>
		/// <item><see cref="XPathNodeType.Root"/></item>
		/// </list>
		/// </remarks>
		public XPathNodeType NodeType
		{
			get
			{
				if( _nodeType == XPathNodeType.All )
					if( _nodePolicy != null )
						_nodeType = _nodePolicy.GetNodeType( this );
					else
						return XPathNodeType.Element;
				return _nodeType;
			}
			set { _nodeType = value; }
		}

		/// <summary>
		/// Gets the count of the attributes.
		/// </summary>
		/// <value>Count of the attributes this node has; <c>0</c> if it hasn't.</value>
		public int AttributesCount
		{
			get
			{
				if( _attributesCount == -1 )
					if( NodeType != XPathNodeType.Element )
						_attributesCount = 0;
					else if( _nodePolicy != null )
						_attributesCount = _nodePolicy.GetAttributesCount( this );
					else
						return 0;
				return _attributesCount;
			}
		}

		/// <summary>
		/// See interface description for details
		/// </summary>
		public Node GetAttribute( int index )
		{
			if( NodeType != XPathNodeType.Element )
				return null;

			if( index < 0 || index >= AttributesCount )
				return null;

			var res = AttributeFromCache( index );
			if( res == null )
			{
				// We don't know this attribute yet
				if( _nodePolicy != null )
				{
					res = _nodePolicy.GetAttribute( this, index );
					if( res != null )
					{
						res.Parent = this;
						res.Index = index;
						CacheAttribute( index, res );
					}
				}
			}
			else if( res == NullCacheEntry )
				// We know that this attribute is null
				res = null;
			return res;
		}

		/// <summary>
		/// See interface description for details
		/// </summary>
		public int FindAttribute( string name, string ns )
		{
			if( NodeType != XPathNodeType.Element )
				return -1;

			if( _nodePolicy != null )
				return _nodePolicy.FindAttribute( this, name, ns );
			return -1;
		}

		/// <summary>
		/// Gets the count of the node's children.
		/// </summary>
		/// <value>Count of the child elements, text nodes and comments this node has; 
		/// <c>0</c> if it hasn't.</value>
		public int ChildrenCount
		{
			get
			{
				if( _elementsCount == -1 )
					if( NodeType != XPathNodeType.Element && NodeType != XPathNodeType.Root )
						_elementsCount = 0;
					else if( _nodePolicy != null )
						_elementsCount = _nodePolicy.GetChildrenCount( this );
					else
						return 0;
				return _elementsCount;
			}
		}

		/// <summary>
		/// Gets the child element with given index.
		/// </summary>
		/// <param name="index">Zero-based index of the child node.</param>
		/// <returns>A child node (element, text node or comment) with given index;
		/// <see langword="null"/> if there is no child node with such index.</returns>
		public Node GetChild( int index )
		{
			if( NodeType != XPathNodeType.Element && NodeType != XPathNodeType.Root )
				return null;

			if( index < 0 || index >= ChildrenCount )
				return null;

			var res = ChildFromCache( index );

			if( res == null )
			{
				// We don't know this element yet
				if( _nodePolicy != null )
				{
					res = _nodePolicy.GetChild( this, index );
					if( res != null )
					{
						res.Parent = this;
						res.Index = index;
						CacheChild( index, res );
					}
				}
			}
			else if( res == NullCacheEntry )
				// We know that this element is null
				res = null;

			return res;
		}

		/// <summary>
		/// Adds the child to this node.
		/// </summary>
		/// <param name="child">Child node to add.</param>
		/// <remarks>This method should be used with caution, because it could
		/// cause interference with child nodes returned by the node policy.</remarks>
		public void AddChild( Node child )
		{
			int index = ChildrenCount;
			CacheChild( index, child );
			child.Parent = this;
			child.Index = index;
			_elementsCount = ChildrenCount + 1;
		}

		/* Cache related methods */

		/// <summary>
		/// Clears the cache of child nodes and attributes.
		/// </summary>
		/// <remarks>After the cache will be cleared, node will requery its node
		/// policy if request for child node or attribute will be issued.</remarks>
		public void ResetCache()
		{
			_cache = null;
		}

		/// <summary>
		/// Minimal size of elements cache to allocate if we don't know total number
		/// of child elements at the creation time.
		/// </summary>
		private readonly int MinimalElementsCacheSize = 5;

		private void CheckCache( int elemIndex )
		{
			int elCount = ChildrenCount;
			if( _cache == null )
			{
				int size = AttributesCount;
				if( elCount == int.MaxValue || elCount <= elemIndex )
					if( elemIndex >= MinimalElementsCacheSize )
						size += elemIndex + 1;
					else
						// Initial elements cache size for unknown number of children
						size += MinimalElementsCacheSize;
				else
					size += elCount;

				_cache = size != 0 ? new Node[size] : EmptyCache;
			}
			else if( elCount == int.MaxValue || elCount <= elemIndex )
			{
				// Check if cache has to be expanded
				int size = _cache.Length;
				int elemSize = size - AttributesCount;
				if( elemIndex >= elemSize )
				{
					var newCache = new Node[size + elemSize];
					Array.Copy( _cache, 0, newCache, 0, size );
				}
			}
		}

		/// <summary>
		/// Caches child element.
		/// </summary>
		/// <param name="i">Zero-based index of child element.</param>
		/// <param name="child">Child element to add.</param>
		private void CacheChild( int i, Node child )
		{
			CheckCache( i );
			_cache[AttributesCount + i] = ( child ?? NullCacheEntry );
		}

		/// <summary>
		/// Caches attribute.
		/// </summary>
		/// <param name="i">Zero-based index of attribute.</param>
		/// <param name="child">Attribute to add..</param>
		private void CacheAttribute( int i, Node child )
		{
			CheckCache( 0 );
			_cache[i] = ( child ?? NullCacheEntry );
		}

		/// <summary>
		/// Gets element from cache.
		/// </summary>
		/// <param name="i">Index of previously cached element.</param>
		/// <returns>Returns previously cached with this index child element.</returns>
		private Node ChildFromCache( int i )
		{
			CheckCache( i );
			return _cache[AttributesCount + i];
		}

		/// <summary>
		/// Gets attribute from cache.
		/// </summary>
		/// <param name="i">Index of previously cached attribute.</param>
		/// <returns>Returns previously cached with this index child element.</returns>
		private Node AttributeFromCache( int i )
		{
			CheckCache( 0 );
			return _cache[i];
		}

	}

	/// <summary>
	/// Enumerates possible states of a node object.
	/// </summary>
	public enum NodeState
	{
		/// <summary>
		/// Target object is not known yet.
		/// </summary>
		ObjectNotKnown = 0,
		/// <summary>
		/// Object is known, either because it was resolved, or it was given 
		/// explicitely.
		/// </summary>
		ObjectKnown,
		/// <summary>
		/// There were an exception during the resolution of the target object.
		/// </summary>
		Exception
	}
}