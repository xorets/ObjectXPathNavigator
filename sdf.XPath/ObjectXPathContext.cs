using System;
using System.Xml;
using System.Xml.XPath;

namespace sdf.XPath
{
	/// <summary>
	/// Represents with method which will be called to handler 
	/// <see cref="ObjectXPathContext.NodePolicyGet"/> event.
	/// </summary>
	public delegate void NodePolicyGetEventHandler( object sender, NodePolicyGetEventArgs e );

	/// <summary>
	/// Represent loop detection event
	/// </summary>
	public delegate void LoopDetectedEventHandler( object sender, LoopDetectionEventArgs e );

	/// <summary>
	/// Defines a context that is shared by all object nodes related to 
	/// the same object navigation tree. 
	/// </summary>
	public class ObjectXPathContext
	{
		/// <summary>
		/// Standard Xml Schema Instance namespace.
		/// </summary>
		internal const string Xsi = "http://www.w3.org/2001/XMLSchema-instance";

		private XmlNameTable _nameTable;
		private XmlNamespaceManager _namespaceManager;
		private NodePolicyFactory _nodePolicyFactory;
		private ConverterFactory _converterFactory;
		private bool _detectLoops;

		/// <summary>
		/// Creates a new <see cref="ObjectXPathContext"/> instance.
		/// </summary>
		/// <remarks>New instance will use new namespace manager instance.</remarks>
		public ObjectXPathContext()
			: this( (XmlNamespaceManager)null )
		{
		}

		/// <summary>
		/// Creates a new <see cref="ObjectXPathContext"/> instance with given 
		/// namespace manager.
		/// </summary>
		/// <param name="nsmgr">A namespace manager that will be used to
		/// resolve elements' namespace to a prefix.</param>
		public ObjectXPathContext( XmlNamespaceManager nsmgr )
		{
			_nodePolicyFactory = new NodePolicyFactory( this );
			_converterFactory = new ConverterFactory();

			if( NamespaceManager != null )
			{
				_namespaceManager = nsmgr;
				_nameTable = NamespaceManager.NameTable;
			}
			else
			{
				_nameTable = new NameTable();
				_namespaceManager = new XmlNamespaceManager( _nameTable );
			}
			_namespaceManager.AddNamespace( "xsi", Xsi );
			_detectLoops = false;
		}

		/// <summary>
		/// Creates a copy of given context. Contexts shares the same name table,
		/// namespace manager, and factories.
		/// </summary>
		/// <param name="other">An instance of <see cref="ObjectXPathContext"/> to 
		/// copy.</param>
		private ObjectXPathContext( ObjectXPathContext other ) :
			this( (XmlNamespaceManager)null )
		{
			_nameTable = other.NameTable;
			_namespaceManager = other.NamespaceManager;
			_nodePolicyFactory = other._nodePolicyFactory;
			_converterFactory = other._converterFactory;
		}

		/// <summary>
		/// Creates the <see cref="ObjectXPathNavigator"/> rooted at given object.
		/// </summary>
		/// <param name="obj">Object which will be a root of navigation tree.</param>
		/// <returns>New instance of <see cref="ObjectXPathNavigator"/> positioned
		/// at the node, corresponding to the given object.</returns>
		public XPathNavigator CreateNavigator( object obj )
		{
			return new ObjectXPathNavigator( obj, this );
		}

		/// <summary>
		/// Gets a name table, this context works with.
		/// </summary>
		/// <value>An instance of <see cref="XmlNameTable"/> that stores all string
		/// values used during tree traversal.</value>
		public XmlNameTable NameTable
		{
			get { return _nameTable; }
		}

		/// <summary>
		/// Gets a namespace manager that is used to map elements' namespace to a 
		/// prefix.
		/// </summary>
		/// <value>An instance of <see cref="XmlNamespaceManager"/>.</value>
		/// <remarks>Tree nodes (objects and their members) are marked with 
		/// attributes from <see cref="System.Xml.Serialization"/> namespace that
		/// difine only fully qualified namespace but do not define prefix. If you 
		/// want your namespaces to have certain prefixes, appropriate mapping should
		/// be added to this namespace manager.</remarks>
		public XmlNamespaceManager NamespaceManager
		{
			get { return _namespaceManager; }
		}

		/// <summary>
		/// Gets a cache of types descriptors.
		/// </summary>
		/// <value>An instance of <see cref="TypeInfoCache"/> object providing
		/// access to types descriptors (<see cref="TypeInfo"/>).</value>
		public TypeInfoCache TypeInfoCache
		{
			get { return TypeInfoCache.Instance; }
		}

		/// <summary>
		/// Gets a converter factory which is used during the conversion of values of
		/// complexly typed class members to string representation.
		/// </summary>
		/// <value>An instance of <see cref="ConverterFactory"/> class.</value>
		/// <remarks>
		/// The converter factory will automatically create converters for types marked
		/// with <see cref="ConverterAttribute"/> attribute. You can also add
		/// a custom converter for a specific type. 
		/// </remarks>
		public ConverterFactory ConverterFactory
		{
			get { return _converterFactory; }
		}

		/// <summary>
		/// If set to true ObjectXPath navigator will detect loops and raise <see cref="LoopDetected"/>LoopDetected events
		/// </summary>
		public bool DetectLoops
		{
			get { return _detectLoops; }
			set { _detectLoops = value; }
		}

		/// <summary>
		/// Occurs during getting of node policy for an object. Event is fired after
		/// a policy was selected by standard method, so event consumers could
		/// specify other policy.
		/// </summary>
		public event NodePolicyGetEventHandler NodePolicyGet;

		/// <summary>
		/// Occurs when loop inside object hierarchy was detected.
		/// </summary>
		public event LoopDetectedEventHandler LoopDetected;

		/// <summary>
		/// Creates a node policy for given object instance. 
		/// </summary>
		/// <param name="obj">An object to create node policy for.</param>
		/// <returns>An object implementing <see cref="INodePolicy"/> interface
		/// which should be used to map this object to the tree node.</returns>
		public INodePolicy GetNodePolicy( object obj )
		{
			var policy = _nodePolicyFactory.GetPolicy( obj.GetType() );
			if( NodePolicyGet != null )
			{
				var e = new NodePolicyGetEventArgs( obj, policy );
				NodePolicyGet( this, e );
				if( e.Policy != null )
					policy = e.Policy;
			}
			return policy;
		}

		/// <summary>
		/// Creates the node policy object of the given type.
		/// </summary>
		/// <param name="policyType">Type of policy object to be created.</param>
		/// <returns>An instance of the given type.</returns>
		/// <remarks>The object returned could be either new instance, created 
		/// especially for
		/// this request, or instance shared by other nodes. This behavior is
		/// solely determined by static GetPolicy method of given policy type.</remarks>
		public INodePolicy CreateNodePolicy( Type policyType )
		{
			return _nodePolicyFactory.CreatePolicy( policyType );
		}

		/// <summary>
		/// Registers the node policy for specific type of objects.
		/// </summary>
		/// <param name="forType">Objects of which type are handled by given policy type.</param>
		/// <param name="policyType">Policy type.</param>
		/// <remarks>If you register a policy for base type or interface then
		/// this policy will be used also for all derived types or types that 
		/// implement this interface. <br/>
		/// If both base type and interface has policies
		/// registered, then policy of base type will be used.<br/>
		/// If several implemented interfaces has policies registered, then it's
		/// not possible to predict which interface's policy will be taken.</remarks>
		public void RegisterNodePolicy( Type forType, Type policyType )
		{
			_nodePolicyFactory.RegisterPolicy( forType, policyType );
		}

		/// <summary>
		/// Called by ObjectXPathNavigator when it detects a loop inside object hierarchy
		/// </summary>
		/// <param name="e"></param>
		internal void OnLoopDetected( LoopDetectionEventArgs e )
		{
			if( LoopDetected != null )
				LoopDetected( this, e );
		}
	}
}