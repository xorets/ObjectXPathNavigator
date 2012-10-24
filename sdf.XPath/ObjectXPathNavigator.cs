using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using sdf.XPath.NodePolicy;

namespace sdf.XPath
{
	/// <summary>
	/// Implements XPathNavigator to navigate a graph of generic objects.
	/// </summary>
	/// <remarks>
	/// Note that object's properties and contents of some collections will be resolved
	/// only once during tree traversal. This means that if navigator has visited
	/// some object then it will remember the state of this object as it was on the 
	/// moment of first visit. Any change to the object's state will be seen only to 
	/// another navigator, either created after this change was made, or which simply 
	/// was not visiting this object yet.
	/// </remarks>
	public class ObjectXPathNavigator : XPathNavigator
	{
		private ObjectXPathContext _context;
		private string _lang;
		private Node _root;
		private Node _node;
		private XPathNavigator _childNav;
		private int _childNavDepth;

		private Stack _navigationStack;

		internal ObjectXPathNavigator( object obj, ObjectXPathContext context )
		{
			_context = context;

			_root = new Node( _context, new RootNodePolicy( obj ) );
			_node = _root;

			if( _context.DetectLoops )
			{
				_navigationStack = new Stack();
				_navigationStack.Push( _root ); // Push dummy root object
			}

			_lang = _context.NameTable.Add( "" );
		}

		/// <summary>
		/// Creates a new navigator positioned at the same node as other navigator.
		/// </summary>
		/// <param name="other">Navigator to be copied</param>
		private ObjectXPathNavigator( ObjectXPathNavigator other )
		{
			if( other._context.DetectLoops )
				_navigationStack = new Stack();
			MoveTo( other );
		}

		/// <summary>
		/// Selects a single object from the current node.
		/// </summary>
		/// <param name="xpath">Selection expression.</param>
		/// <returns>Returns the first object found by the expression or <see langword="null"/>.</returns>
		public object SelectObject( string xpath )
		{
			var i = Select( xpath );
			return i.MoveNext() ? ( (ObjectXPathNavigator)i.Current ).Object : null;
		}

		/// <summary>
		/// Selects a group of objects from the current node.
		/// </summary>
		/// <param name="xpath">Selection expression.</param>
		/// <param name="returnItemType">Type of array elements to be returned.</param>
		/// <returns>Returns an array with all the objects found
		/// by the expression.</returns>
		public Array SelectObjects( string xpath, Type returnItemType )
		{
			if( null == returnItemType )
				throw new ArgumentNullException( "returnItemType" );

			var result = new ArrayList();
			var i = Select( xpath );
			while( i.MoveNext() )
				result.Add( ( (ObjectXPathNavigator)i.Current ).Object );
			return result.ToArray( returnItemType );
		}

		/// <summary>
		/// Selects a group of objects from the current node.
		/// </summary>
		/// <param name="xpath">Selection expression.</param>
		/// <returns>Returns an array with all the objects found
		/// by the expression.</returns>
		public object[] SelectObjects( string xpath )
		{
			return (object[])SelectObjects( xpath, typeof( object ) );
		}

		/// <summary>
		/// Gets a <see ref="XmlNamespaceManager"/> used to resolve prefixes for
		/// namespaces.
		/// </summary>
		/// <value>A namespace manager that shows which prefixes we wish to use for
		/// object namespaces.</value>
		public XmlNamespaceManager NamespaceManager
		{
			get { return _context.NamespaceManager; }
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.BaseURI" /> for details.
		/// </summary>
		public override string BaseURI
		{
			get
			{
#if DEBUG
				Trace( "BaseURI" );
#endif
				// Invoke child navigator
				return _childNav != null ? _context.NameTable.Add( _childNav.BaseURI ) : _node.Namespace;
			}
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.Clone" /> for details.
		/// </summary>
		public override XPathNavigator Clone()
		{
#if DEBUG
			Trace( "Clone" );
#endif
			return new ObjectXPathNavigator( this );
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.GetAttribute(string, string)" /> for details.
		/// </summary>
		public override string GetAttribute( string localName, string namespaceURI )
		{
#if DEBUG
			Trace( "GetAttribute" );
#endif
			// Invoke child navigator
			if( _childNav != null )
				return _childNav.GetAttribute( localName, namespaceURI );

			var clone = Clone();
			return clone.MoveToAttribute( localName, namespaceURI ) ? clone.Value : string.Empty;
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.GetNamespace(string)" /> for details.
		/// </summary>
		public override string GetNamespace( string name )
		{
#if DEBUG
			Trace( "GetNamespace" );
#endif
			// Invoke child navigator
			if( _childNav != null )
				return _context.NameTable.Add( _childNav.GetNamespace( name ) );

			return _context.NamespaceManager.LookupNamespace( name ) ?? string.Empty;
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.HasAttributes" /> for details.
		/// </summary>
		public override bool HasAttributes
		{
			get
			{
#if DEBUG
				Trace( "HasAttributes" );
#endif
				// Invoke child navigator
				if( _childNav != null )
					return _childNav.HasAttributes;

				return _node.AttributesCount != 0 && Clone().MoveToFirstAttribute();
			}
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.HasChildren" /> for details.
		/// </summary>
		public override bool HasChildren
		{
			get
			{
#if DEBUG
				Trace( "HasChildren" );
#endif
				// Invoke child navigator
				if( _childNav != null )
					return _childNav.HasChildren;

				// Check that not all of them are transparent
				return _node.ChildrenCount != 0 && Clone().MoveToFirstChild();
			}
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.IsEmptyElement" /> for details.
		/// </summary>
		public override bool IsEmptyElement
		{
			get
			{
#if DEBUG
				Trace( "IsEmptyElement" );
#endif
				// Invoke child navigator
				return _childNav != null ? _childNav.IsEmptyElement : _node.ChildrenCount == 0;
			}
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.IsSamePosition" /> for details.
		/// </summary>
		/// <returns>Returns <see langword="true"/> if this navigator is positioned
		/// at the same node as other navigator, and <see langword="false"/>
		/// otherwise.</returns>
		public override bool IsSamePosition( XPathNavigator other )
		{
#if DEBUG
			Trace( () => string.Format( "IsSamePosition( N#{0} )", other.GetHashCode() ) );
#endif
			var x = other as ObjectXPathNavigator;
			if( x == null )
				return false;

			if( _context != x._context )
				return false;

			// Compare child navigators
			if( _childNav != null )
				return x._childNav != null && _childNav.IsSamePosition( x._childNav );
			if( x._childNav != null )
				// In case if our navigator is null and other is not.
				return false;

			return _node == x._node;
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.LocalName" /> for details.
		/// </summary>
		public override string LocalName
		{
			get
			{
#if DEBUG
				Trace( "LocalName" );
#endif
				// Invoke child navigator
				return _childNav != null ? _context.NameTable.Add( _childNav.LocalName ) : _node.Name;
			}
		}

		/// <summary>
		/// Gets a fully qualified name of the node.
		/// </summary>
		/// <value>Fully qualified name of the node.</value>
		/// <remarks><see cref="NamespaceManager"/> will be used to resolve a prefix
		/// for node's namespace. If no prefix could be found, then name without a
		/// prefix will be returned.</remarks>
		public override string Name
		{
			get
			{
#if DEBUG
				Trace( "Name" );
#endif
				string ns = NamespaceURI;

				if( ns != null && ns.Length == 0 )
					return LocalName;
				string name = LocalName;
				string prefix = Prefix;
				if( prefix != null && prefix.Length == 0 )
					return name;
				return string.Format( "{0}:{1}", prefix, name );
			}
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.NamespaceURI" /> for details.
		/// </summary>
		public override string NamespaceURI
		{
			get
			{
#if DEBUG
				Trace( "NamespaceURI" );
#endif
				// Invoke child navigator
				return _childNav != null ? _context.NameTable.Add( _childNav.NamespaceURI ) : _node.Namespace;
			}
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.NameTable" /> for details.
		/// </summary>
		public override XmlNameTable NameTable
		{
			get
			{
#if DEBUG
				Trace( "NameTable" );
#endif
				return _context.NameTable;
			}
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.MoveTo" /> for details.
		/// </summary>
		public override bool MoveTo( XPathNavigator other )
		{
			var otherNav = other as ObjectXPathNavigator;
			if( otherNav == null )
				return false;

			_context = otherNav._context;
			_root = otherNav._root;
			_node = otherNav._node;
			if( otherNav._childNav != null )
			{
				_childNav = otherNav._childNav.Clone();
				_childNavDepth = otherNav._childNavDepth;
			}
			else
				_childNav = null;
#if DEBUG
			Trace( () => string.Format( "MoveTo( N#{0} )", other.GetHashCode() ) );
#endif
			if( _context.DetectLoops )
				_navigationStack = (Stack)otherNav._navigationStack.Clone();

			return true;
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.MoveToAttribute" /> for details.
		/// </summary>
		public override bool MoveToAttribute( string localName, string namespaceURI )
		{
#if DEBUG
			Trace( "MoveToAttribute" );
#endif
			// Invoke child navigator
			if( _childNav != null )
			{
				bool res = _childNav.MoveToAttribute( localName, namespaceURI );
				if( res )
					_childNavDepth++;
				return res;
				// TODO: Check what will happen if we are positioned at the navigable root of the transparent node. 
			}

			if( _node.NodeType != XPathNodeType.Element )
				return false;

			int i = _node.FindAttribute( localName, namespaceURI );
			if( i >= 0 )
			{
				var newnode = _node.GetAttribute( i );
				if( newnode != null && !newnode.IsTransparent )
				{
					_node = newnode;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.MoveToFirst" /> for details.
		/// </summary>
		public override bool MoveToFirst()
		{
#if DEBUG
			Trace( "MoveToFirst" );
#endif
			// Invoke child navigator
			if( _childNav != null )
			{
				if( !_node.IsTransparent || _childNavDepth > 0 )
					// Navigable node could be transparent, so MoveToFirst could 
					// position us outside the child navigator.
					return _childNav.MoveToFirst();
				_childNav = null;
			}

			if( _node.NodeType == XPathNodeType.Attribute )
				return false;

			bool res = MoveToParent();
			if( !res )
				// There is no parent, hence no sibling
				return false;

			return MoveToFirstChild();
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.MoveToFirstAttribute" /> for details.
		/// </summary>
		public override bool MoveToFirstAttribute()
		{
#if DEBUG
			Trace( "MoveToFirstAttribute" );
#endif
			// Invoke child navigator
			if( _childNav != null )
			{
				bool res = _childNav.MoveToFirstAttribute();
				if( res )
					_childNavDepth++;
				return res;
			}

			if( _node.AttributesCount == 0 )
				return false;

			for( int i = 0; i < _node.AttributesCount; i++ )
			{
				var newnode = _node.GetAttribute( i );
				if( newnode != null && !newnode.IsTransparent )
				{
					_node = newnode;
					if( _context.DetectLoops )
					{
						_navigationStack.Push( _node.Object );
						Trace( () => string.Format( "Move2.1stAttr Pushed {0} {1}", _node.Object, GetNavigationStack() ) );
					}
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.MoveToFirstChild" /> for details.
		/// </summary>
		public override bool MoveToFirstChild()
		{
#if DEBUG
			Trace( "MoveToFirstChild" );
#endif
			// Invoke child navigator
			if( _childNav != null )
			{
				bool res = _childNav.MoveToFirstChild();
				if( res )
					_childNavDepth++;
				return res;
			}

			// Check if we are trying to descend into IXPathNavigable object
			if( IsNodeXPathNavigable( _node ) )
				return MoveIntoNavigableNode( _node );

			var newnode = GetNonTransparentChild( _node, true );

			while( newnode != null )
			{
				if( !newnode.IsTransparent )
				{
					_node = newnode;
					if( _context.DetectLoops )
					{
						Trace( () => string.Format( "MoveTo1stChild Pushing {0} {1} TRUE", _node.Object, GetNavigationStack() ) );
						_navigationStack.Push( newnode.Object );
					}
					return true;
				}
				// It's transparent but navigable node.
				// Try to move into this navigable node.
				if( MoveIntoNavigableNode( newnode ) )
				{
					_node = newnode;
					if( _context.DetectLoops )
					{
						Trace( () => string.Format( "T: MoveTo1stChild Pushing {0} {1} TRUE", _node.Object, GetNavigationStack() ) );
						_navigationStack.Push( newnode.Object );
					}
					return true;
				}
				// If failure, get its sibling
				newnode = GetNonTransparentSibling( newnode, true );
				continue;
			}
			return false;
		}

		/// <summary>
		/// Stack content in text format
		/// </summary>
		public string GetNavigationStack()
		{
			if( _navigationStack == null )
				return "No stack";

			if( _navigationStack.Count == 0 )
				return "{}";

			var sl = new List<string>();
			foreach( var node in _navigationStack )
				sl.Add( node != null ? node.ToString() : "NULL" );
			return "{" + string.Join( ",", sl.ToArray() ) + "}";
		}

		/// <summary>
		/// Path of current node
		/// </summary>
		public string GetCurrentNodePath()
		{
			string res = string.Empty;
			var nav = Clone();
			do
			{
				res = ( nav.Prefix != string.Empty ? nav.Prefix + ":" : "" )
					+ ( nav.NodeType == XPathNodeType.Attribute ? "@" : "" )
					+ nav.LocalName
					+ ( nav.NodeType == XPathNodeType.Text ? "\"" + nav.Value + "\"" : "" )
					+ ( res != string.Empty ? "/" : "" )
					+ res;
			} while( nav.MoveToParent() );
			return res;
		}

		private bool MoveIntoNavigableNode( Node node )
		{
			var navigable = node.Object as IXPathNavigable;
			if( navigable != null )
			{
				_childNav = navigable.CreateNavigator();
				_childNavDepth = 0;
				if( _childNav.NodeType == XPathNodeType.Root || ( node.Member != null && node.Member.SkipNavigableRoot != null ) )
				{
					bool res = _childNav.MoveToFirstChild();
					if( !res )
					{
						// The type of node is Root and we can't descend to a child
						_childNav = null;
						return false;
					}
					_childNav.MoveToFirst();
				}

				// Descended succesfully
				return true;
			}
			return false;
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.MoveToFirstNamespace(XPathNamespaceScope)" /> for details.
		/// </summary>
		public override bool MoveToFirstNamespace( XPathNamespaceScope namespaceScope )
		{
#if DEBUG
			Trace( "MoveToFirstNamespace" );
#endif
			return false;
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.MoveToId" /> for details.
		/// </summary>
		/// <remarks>Not supported.</remarks>
		public override bool MoveToId( string id )
		{
#if DEBUG
			Trace( "MoveToId" );
#endif
			return false;
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.MoveToNamespace(string)" /> for details.
		/// </summary>
		public override bool MoveToNamespace( string name )
		{
#if DEBUG
			Trace( "MoveToNamespace" );
#endif
			return false;
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.MoveToNext()" /> for details.
		/// </summary>
		public override bool MoveToNext()
		{
#if DEBUG
			Trace( "MoveToNext" );
#endif
			// Invoke child navigator
			if( _childNav != null )
			{
				bool res = _childNav.MoveToNext();
				if( res )
					return true;
				if( !_node.IsTransparent || _childNavDepth > 0 )
					return false;
			}


			var newnode = GetNonTransparentSibling( _node, true );
			while( newnode != null )
			{
				if( !newnode.IsTransparent )
				{
					_node = newnode;
					_childNav = null;

					if( _context.DetectLoops )
					{
						Trace( () => string.Format( "Move2Next Popping/Pushing {0} {1}", _node.Object, GetNavigationStack() ) );
						if( _navigationStack.Count > 0 )
							_navigationStack.Pop();
						else
							Trace( () => string.Format( "{0} MoveToNext : stack is empty", Object ) );
						_navigationStack.Push( newnode.Object );
					}
					return true;
				}
				// It's transparent but navigable node.
				// Try to move into this navigable node.
				if( MoveIntoNavigableNode( newnode ) )
				{
					_node = newnode;

					if( _context.DetectLoops )
					{
						Trace( () => string.Format( "Move2Next Popping/Pushing {0} {1}", _node.Object, GetNavigationStack() ) );

						if( _navigationStack.Count > 0 )
							_navigationStack.Pop();
						else
							Trace( () => string.Format( "{0} MoveToNext : stack is empty", Object ) );
						_navigationStack.Push( newnode.Object );
					}
					return true;
				}
				// If failure, get its sibling
				newnode = GetNonTransparentSibling( newnode, true );
				continue;
			}
			return false;
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.MoveToPrevious" /> for details.
		/// </summary>
		public override bool MoveToPrevious()
		{
#if DEBUG
			Trace( "MoveToPrevious" );
#endif
			// Invoke child navigator
			if( _childNav != null )
			{
				bool res = _childNav.MoveToPrevious();
				if( res )
					return true;
				if( !_node.IsTransparent || _childNavDepth > 0 )
					return false;
			}

			var newnode = GetNonTransparentSibling( _node, false );
			while( newnode != null )
			{
				if( !newnode.IsTransparent )
				{
					_node = newnode;
					_childNav = null;

					if( _context.DetectLoops )
					{
						Trace( () => string.Format( "Move2Prev Pushing {0} {1}", _node.Object, GetNavigationStack() ) );
						if( _navigationStack.Count > 0 )
							_navigationStack.Pop();
						else
							Trace( () => string.Format( "{0} MoveToPrevious : stack empty", Object ) );
						_navigationStack.Push( newnode.Object );
					}
					return true;
				}
				// It's transparent but navigable node.
				// Try to move into this navigable node.
				if( MoveIntoNavigableNode( newnode ) )
				{
					_node = newnode;
					// Go to the last sibling of the child navigator's node
					while( _childNav.MoveToNext() )
						;

					if( _context.DetectLoops )
					{
						Trace( () => string.Format( "Move2Prev {0} {1}", _node.Object, GetNavigationStack() ) );
						if( _navigationStack.Count > 0 )
							_navigationStack.Pop();
						else
							Trace( () => string.Format( "{0} MoveToPrevious : stack is empty", Object ) );
						_navigationStack.Push( newnode.Object );
					}
					return true;
				}
				// If failure, get its sibling
				newnode = GetNonTransparentSibling( newnode, false );
				continue;
			}
			return false;
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.MoveToNextAttribute" /> for details.
		/// </summary>
		public override bool MoveToNextAttribute()
		{
#if DEBUG
			Trace( "MoveToNextAttribute" );
#endif
			// Invoke child navigator
			if( _childNav != null )
				return _childNav.MoveToNextAttribute();

			if( _node.NodeType != XPathNodeType.Attribute )
				return false;

			for( int i = _node.Index + 1; i < _node.Parent.AttributesCount; i++ )
			{
				var newnode = _node.Parent.GetAttribute( i );
				if( newnode != null && !newnode.IsTransparent )
				{
					_node = newnode;
					if( _context.DetectLoops )
					{
						Trace( () => string.Format( "Move2NextAttr popping {0}", GetNavigationStack() ) );
						if( _navigationStack.Count > 0 )
							_navigationStack.Pop();
						else
							Trace( () => string.Format( "MoveToNextAttribute: Stack is empty {0}", GetNavigationStack() ) );

						_navigationStack.Push( newnode.Object );
					}
					return true;
				}
			}
			Trace( () => string.Format( "~Move2NextAttr" ) );
			return false;
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.MoveToNextNamespace( XPathNamespaceScope )" /> for details.
		/// </summary>
		public override bool MoveToNextNamespace( XPathNamespaceScope namespaceScope )
		{
#if DEBUG
			Trace( "MoveToNextNamespace" );
#endif
			// Invoke child navigator
			return _childNav != null && _childNav.MoveToNextNamespace( namespaceScope );
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.MoveToParent" /> for details.
		/// </summary>
		public override bool MoveToParent()
		{
#if DEBUG
			Trace( "MoveToParent" );
			Trace( GetNavigationStack );
#endif
			// Invoke child navigator
			if( _childNav != null )
			{
				Trace( () => String.Format( "Trace: ChildNav" ) );
				if( _childNavDepth == 0 )
				{
					// We reached root node of the child navigator
					_childNav = null;
					if( !_node.IsTransparent )
						// Child navigator cleared, we are at its non-transparent container
						return true;
				}
				else
				{
					// Descend one level
					_childNav.MoveToParent();
					_childNavDepth--;
					return true;
				}
			}

			var parent = GetNonTransparentParent( _node );

			if( _context.DetectLoops )
				if( parent != null )
				{
					Trace( () => String.Format( "Move2Parent popping {0}", GetNavigationStack() ) );
					if( _navigationStack.Count > 0 )
						_navigationStack.Pop();
					else
						Trace( () => String.Format( "MoveToParent: Stack is empty" ) );
				}

			if( parent != null )
			{
				_node = parent;
				return true;
			}

			return false;
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.MoveToRoot" /> for details.
		/// </summary>
		public override void MoveToRoot()
		{
#if DEBUG
			Trace( "MoveToRoot" );
#endif
			_node = _root;
			_childNav = null;

			if( _context.DetectLoops )
			{
				_navigationStack.Clear();
				Trace( () => string.Format( "Move2Root {0} {1}", _node.Object, GetNavigationStack() ) );
			}
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.NodeType" /> for details.
		/// </summary>
		public override XPathNodeType NodeType
		{
			get
			{
#if DEBUG
				Trace( "NodeType" );
#endif
				// Invoke child navigator
				return _childNav != null ? _childNav.NodeType : _node.NodeType;
			}
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathItem.Value" /> for details.
		/// </summary>
		public override string Value
		{
			get
			{
#if DEBUG
				Trace( "Value" );
#endif
				// Invoke child navigator
				return _childNav != null ? _childNav.Value : _node.Value;
			}
		}

		/// <summary>
		/// The current object.
		/// </summary>
		public object Object
		{
			get
			{
#if DEBUG
				Trace( "Node" );
#endif
				return _childNav == null ? _node.Object : ( _childNav is IHasXmlNode ? ( (IHasXmlNode)_childNav ).GetNode() : null );
			}
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.XmlLang" /> for details.
		/// </summary>
		public override string XmlLang
		{
			get
			{
#if DEBUG
				Trace( "XmlLang" );
#endif
				// Invoke child navigator
				if( _childNav != null )
					return _childNav.XmlLang;

				var clone = Clone();
				string xmlns = NameTable.Get( "http://www.w3.org/XML/1998/namespace" );
				do
					if( clone.MoveToAttribute( "lang", xmlns ) )
						return clone.Value;
				while( clone.MoveToParent() );

				return string.Empty;
			}
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.Prefix" /> for details.
		/// </summary>
		/// <remarks>Prefix is beeng looked up in the <see cref="NamespaceManager"/>.</remarks>
		public override string Prefix
		{
			get
			{
#if DEBUG
				Trace( "Prefix" );
#endif
				string prefix = _context.NamespaceManager.LookupPrefix( NamespaceURI );

				// Invoke child navigator
				if( _childNav != null && prefix == null )
					prefix = _context.NameTable.Add( _childNav.Prefix );

				return prefix ?? string.Empty;
			}
		}

		/// <summary>
		/// See <see cref="System.Xml.XPath.XPathNavigator.ComparePosition" /> for details.
		/// </summary>
		public override XmlNodeOrder ComparePosition( XPathNavigator nav )
		{
			var other = nav as ObjectXPathNavigator;
			if( other == null )
				return XmlNodeOrder.Unknown;

			if( _childNav == null && other._childNav == null )
			{
				if( _node == other._node )
					return XmlNodeOrder.Same;

				var parent = GetNonTransparentParent( _node );
				var otherParent = GetNonTransparentParent( other._node );
				if( parent == otherParent )
				{
					if( _node.NodeType == XPathNodeType.Attribute )
					{
						if( other._node.NodeType == XPathNodeType.Attribute )
							return _node.Index < other._node.Index ? XmlNodeOrder.Before : XmlNodeOrder.After;
						return XmlNodeOrder.Before;
					}
				}
			}
			return base.ComparePosition( nav );
		}

		private static bool IsNodeXPathNavigable( Node node )
		{
			if( node.ObjectType == null )
				return false;
			return node.ObjectType.IsXPathNavigable;
		}

		private static Node GetNonTransparentParent( Node node )
		{
			var parent = node.Parent;
			while( parent != null && parent.IsTransparent )
				parent = parent.Parent;
			return parent;
		}

		private Node GetNonTransparentChild( Node current, bool goForward )
		{
			if( current.ChildrenCount == 0 )
				return null;

			Node newNode;
			bool inStack = false;
			int index = goForward ? 0 : current.ChildrenCount - 1;
			do
			{
				newNode = current.GetChild( index );

				if( newNode == null && current.IsTransparent )
					return GetNonTransparentSibling( current, goForward );

				// If we have to detect loops.
				if( _context.DetectLoops )
					if( newNode != null && newNode.Object != null && _navigationStack.Contains( newNode.Object ) )
					{
						var evt = new LoopDetectionEventArgs( this, newNode );
						_context.OnLoopDetected( evt );

						if( !evt.IgnoreLoop )
						{
							inStack = true;
							Trace( () => String.Format( "GetNonTransparentChild {0} already in stack", newNode.Object ) );
							if( goForward )
								index++;
							else
								index--;

							continue;
						}
					}
					else
						inStack = false;

				current = newNode;
				if( current != null )
					index = goForward ? 0 : current.ChildrenCount - 1;

			} while( inStack || ( current != null && current.IsTransparent && !IsNodeXPathNavigable( current ) ) );
			// Break if current is null, or non trasparent or navigable

			return current;
		}

		private Node GetNonTransparentSibling( Node current, bool goForward )
		{
			if( current.NodeType == XPathNodeType.Attribute )
				return null;

			Node newnode;
			var parent = current.Parent;

			if( parent == null )
				return null;

			Object old = null;
			if( _context.DetectLoops )
				if( _navigationStack.Count > 0 )
					old = _navigationStack.Pop();
			do
			{
				newnode = goForward ? parent.GetChild( current.Index + 1 ) : parent.GetChild( current.Index - 1 );

				// While there are no more nodes in this transparent parent, try to 
				// ascend up the hierarchy and take node next to this transparent node.
				while( newnode == null && parent.IsTransparent )
				{
					newnode = parent;
					parent = parent.Parent;
					newnode = goForward ? parent.GetChild( newnode.Index + 1 ) : parent.GetChild( newnode.Index - 1 );
				}

				// If node is found but it is transparent and not navigable
				if( newnode != null && newnode.IsTransparent && !IsNodeXPathNavigable( newnode ) )
				{
					current = newnode;
					newnode = GetNonTransparentChild( current, goForward );
					if( newnode != null )
						break;
					continue;
				}

				// If we have to detect loops
				if( _context.DetectLoops )
					if( newnode != null && newnode.Object != null && _navigationStack.Contains( newnode.Object ) )
					{
						var evt = new LoopDetectionEventArgs( this, newnode );
						_context.OnLoopDetected( evt );

						if( !evt.IgnoreLoop )
						{
							current = newnode;
							continue;
						}
					}
				break;
			} while( true );

			if( old != null )
				_navigationStack.Push( old );

			return newnode;
		}

#if DEBUG
		private void Trace( string method )
		{
			return;
		}
#endif

		private void Trace( Func<string> message )
		{
#if DEBUG
			Trace( message() );
#endif
		}
	}
}